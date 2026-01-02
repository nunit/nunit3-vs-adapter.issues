#!/usr/bin/env python3
"""
Script to build all issue projects and report compilation failures.

Usage:
    python check-builds.py                    # Build all issues
    python check-builds.py failed-builds.json # Build only projects in JSON file
"""

import argparse
import json
import os
import subprocess
import sys
from pathlib import Path
from collections import defaultdict
from typing import List, Tuple, Optional, Dict, Any

# Set UTF-8 encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')


def find_project_files(issue_dir: Path) -> List[Path]:
    """Find all .csproj and .sln files in an issue directory."""
    project_files = []
    
    # Find .sln files first (prefer solution files)
    sln_files = list(issue_dir.rglob("*.sln"))
    if sln_files:
        project_files.extend(sln_files)
    else:
        # If no .sln, find .csproj files
        csproj_files = list(issue_dir.rglob("*.csproj"))
        project_files.extend(csproj_files)
    
    return project_files


def build_project(project_path: Path, timeout: int = 300) -> Tuple[bool, str, str]:
    """
    Build a project using dotnet build.
    Returns: (success, stdout, stderr)
    """
    try:
        result = subprocess.run(
            ["dotnet", "build", str(project_path), "--no-restore"],
            cwd=project_path.parent,
            capture_output=True,
            text=True,
            timeout=timeout
        )
        
        return result.returncode == 0, result.stdout, result.stderr
    except subprocess.TimeoutExpired:
        return False, "", f"Build timed out after {timeout} seconds"
    except Exception as e:
        return False, "", str(e)


def load_json_projects(json_path: Path) -> Dict[str, List[str]]:
    """
    Load project paths from JSON file.
    Returns: Dictionary mapping issue names to list of project paths
    """
    try:
        with open(json_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
        
        projects_by_issue = defaultdict(list)
        for item in data.get('failed_builds', []):
            issue = item.get('issue', '')
            project = item.get('project', '')
            if issue and project:
                projects_by_issue[issue].append(project)
        
        return dict(projects_by_issue)
    except Exception as e:
        print(f"Error loading JSON file {json_path}: {e}", file=sys.stderr)
        sys.exit(1)


def main():
    """Main function to check all issue builds."""
    parser = argparse.ArgumentParser(
        description='Build issue projects and report compilation failures.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python check-builds.py                    # Build all issues
  python check-builds.py failed-builds.json # Build only projects in JSON file
        """
    )
    parser.add_argument(
        'json_file',
        nargs='?',
        type=str,
        help='Optional JSON file containing list of projects to build (e.g., failed-builds.json)'
    )
    args = parser.parse_args()
    
    repo_root = Path(__file__).parent
    
    # Load JSON file if provided
    json_projects = None
    if args.json_file:
        json_path = Path(args.json_file)
        if not json_path.is_absolute():
            json_path = repo_root / json_path
        if not json_path.exists():
            print(f"Error: JSON file not found: {json_path}", file=sys.stderr)
            sys.exit(1)
        json_projects = load_json_projects(json_path)
        print(f"Loaded {sum(len(projs) for projs in json_projects.values())} projects from {args.json_file}")
        print("=" * 80)
    else:
        # Find all issue directories
        issue_dirs = sorted([d for d in repo_root.iterdir() 
                            if d.is_dir() and d.name.startswith("Issue")])
        
        if not issue_dirs:
            print("No issue directories found.")
            return 1
        
        print(f"Found {len(issue_dirs)} issue directories")
        print("=" * 80)
    
    results = {
        "success": [],
        "failed": [],
        "no_project": [],
        "skipped": []
    }
    
    # Track failures by issue
    failures = defaultdict(list)
    
    # Create error output directory if using JSON file
    error_output_dir = None
    if json_projects and args.json_file:
        json_path = Path(args.json_file)
        if not json_path.is_absolute():
            json_path = repo_root / json_path
        # Create error output directory next to the JSON file
        error_output_dir = json_path.parent / f"{json_path.stem}_errors"
        error_output_dir.mkdir(exist_ok=True)
        print(f"Error messages will be saved to: {error_output_dir}")
    
    if json_projects:
        # Build only projects from JSON file
        for issue_name, project_paths in sorted(json_projects.items()):
            issue_dir = repo_root / issue_name
            if not issue_dir.exists():
                print(f"\n[{issue_name}] ISSUE DIRECTORY NOT FOUND")
                results["no_project"].append(issue_name)
                if error_output_dir:
                    error_file = error_output_dir / f"{issue_name}_not_found.txt"
                    with open(error_file, 'w', encoding='utf-8') as f:
                        f.write(f"Issue directory not found: {issue_name}\n")
                continue
            
            print(f"\n[{issue_name}]", end=" ", flush=True)
            
            issue_success = True
            for project_path_str in project_paths:
                # Convert Windows path separators if needed
                project_path = repo_root / project_path_str.replace('\\', os.sep)
                
                if not project_path.exists():
                    print(f"\n  Project not found: {project_path_str}")
                    issue_success = False
                    error_msg = f"Project file not found: {project_path}"
                    failures[issue_name].append({
                        "project": project_path_str,
                        "stdout": "",
                        "stderr": error_msg
                    })
                    if error_output_dir:
                        error_file = error_output_dir / f"{issue_name}_{Path(project_path_str).stem}_not_found.txt"
                        with open(error_file, 'w', encoding='utf-8') as f:
                            f.write(f"Project: {project_path_str}\n")
                            f.write(f"Error: {error_msg}\n")
                    continue
                
                rel_path = project_path.relative_to(repo_root)
                print(f"\n  Building: {rel_path}", end=" ", flush=True)
                
                success, stdout, stderr = build_project(project_path)
                
                if success:
                    print("[OK] SUCCESS")
                else:
                    print("[FAIL] FAILED")
                    issue_success = False
                    failures[issue_name].append({
                        "project": str(rel_path),
                        "stdout": stdout,
                        "stderr": stderr
                    })
                    
                    # Save error message to file
                    if error_output_dir:
                        # Create a safe filename from the project path
                        safe_project_name = str(rel_path).replace('\\', '_').replace('/', '_').replace(':', '_')
                        error_file = error_output_dir / f"{issue_name}_{safe_project_name}_error.txt"
                        with open(error_file, 'w', encoding='utf-8') as f:
                            f.write(f"Project: {rel_path}\n")
                            f.write(f"Issue: {issue_name}\n")
                            f.write("=" * 80 + "\n\n")
                            if stdout:
                                f.write("STDOUT:\n")
                                f.write("-" * 80 + "\n")
                                f.write(stdout)
                                f.write("\n\n")
                            if stderr:
                                f.write("STDERR:\n")
                                f.write("-" * 80 + "\n")
                                f.write(stderr)
                                f.write("\n\n")
            
            if issue_success:
                results["success"].append(issue_name)
                print("  → All builds succeeded")
            else:
                results["failed"].append(issue_name)
    else:
        # Build all issues (original behavior)
        for issue_dir in issue_dirs:
            issue_name = issue_dir.name
            print(f"\n[{issue_name}]", end=" ", flush=True)
            
            # Check for ignore markers
            ignore_files = ["ignore", "ignore.md", "explicit", "explicit.md", 
                           "wip", "wip.md", "gui", "gui.md", "closedasnotplanned", 
                           "closedasnotplanned.md"]
            has_ignore = any((issue_dir / f).exists() for f in ignore_files)
            
            if has_ignore:
                print("SKIPPED (has ignore marker)")
                results["skipped"].append(issue_name)
                continue
            
            # Find project files
            project_files = find_project_files(issue_dir)
            
            if not project_files:
                print("NO PROJECT FILES")
                results["no_project"].append(issue_name)
                continue
            
            # Build each project file
            issue_success = True
            for project_file in project_files:
                rel_path = project_file.relative_to(repo_root)
                print(f"\n  Building: {rel_path}", end=" ", flush=True)
                
                success, stdout, stderr = build_project(project_file)
                
                if success:
                    print("[OK] SUCCESS")
                else:
                    print("[FAIL] FAILED")
                    issue_success = False
                    failures[issue_name].append({
                        "project": str(rel_path),
                        "stdout": stdout,
                        "stderr": stderr
                    })
            
            if issue_success:
                results["success"].append(issue_name)
                print("  → All builds succeeded")
            else:
                results["failed"].append(issue_name)
    
    # Print summary
    print("\n" + "=" * 80)
    print("BUILD SUMMARY")
    print("=" * 80)
    total_checked = (len(results['success']) + len(results['failed']) + 
                    len(results['no_project']) + len(results['skipped']))
    print(f"Total issues checked: {total_checked}")
    print(f"  [OK] Successful builds: {len(results['success'])}")
    print(f"  [FAIL] Failed builds: {len(results['failed'])}")
    print(f"  [N/A] No project files: {len(results['no_project'])}")
    if not json_projects:
        print(f"  [SKIP] Skipped (ignore markers): {len(results['skipped'])}")
    
    # Print failed builds details
    if results["failed"]:
        print("\n" + "=" * 80)
        print("FAILED BUILDS")
        print("=" * 80)
        
        for issue_name in sorted(results["failed"]):
            print(f"\n[{issue_name}]")
            for failure in failures[issue_name]:
                print(f"  Project: {failure['project']}")
                if failure['stderr']:
                    # Extract error summary from stderr
                    error_lines = failure['stderr'].split('\n')
                    error_summary = [line for line in error_lines 
                                   if 'error' in line.lower() or 'failed' in line.lower()]
                    if error_summary:
                        print("  Errors:")
                        for line in error_summary[:5]:  # Show first 5 error lines
                            print(f"    {line}")
                    else:
                        print(f"  Stderr: {failure['stderr'][:200]}...")
                print()
    
    # Print issues with no project files (if any)
    if results["no_project"]:
        print("\n" + "=" * 80)
        print("ISSUES WITH NO PROJECT FILES")
        print("=" * 80)
        for issue_name in sorted(results["no_project"]):
            print(f"  {issue_name}")
    
    return 0 if not results["failed"] else 1


if __name__ == "__main__":
    try:
        exit_code = main()
        sys.exit(exit_code)
    except KeyboardInterrupt:
        print("\n\nBuild check interrupted by user.")
        sys.exit(130)
    except Exception as e:
        print(f"\n\nUnexpected error: {e}", file=sys.stderr)
        import traceback
        traceback.print_exc()
        sys.exit(1)

