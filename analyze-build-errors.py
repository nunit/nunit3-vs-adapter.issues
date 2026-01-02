#!/usr/bin/env python3
"""
Script to analyze build errors from failed-builds.json and categorize them.

Usage:
    python analyze-build-errors.py [failed-builds.json] [failed-builds_errors]
"""

import argparse
import json
import re
import sys
from collections import defaultdict, Counter
from pathlib import Path
from typing import Dict, List, Tuple, Optional

# Set UTF-8 encoding for Windows console
if sys.platform == 'win32':
    import codecs
    sys.stdout = codecs.getwriter('utf-8')(sys.stdout.buffer, 'strict')
    sys.stderr = codecs.getwriter('utf-8')(sys.stderr.buffer, 'strict')


def categorize_error(error_line: str) -> str:
    """
    Categorize an error line into a category.
    Returns the error category.
    """
    error_line_lower = error_line.lower()
    
    # NUnit Analyzer errors
    if 'nunit' in error_line_lower and ('analyzer' in error_line_lower or 'nunit10' in error_line_lower or 'nunit11' in error_line_lower):
        # Extract error code if present
        match = re.search(r'(NUnit\d{4})', error_line)
        if match:
            return f"NUnit Analyzer ({match.group(1)})"
        return "NUnit Analyzer"
    
    # C# compiler errors (CS####)
    if re.search(r'error CS\d{4}', error_line, re.IGNORECASE):
        match = re.search(r'error (CS\d{4})', error_line, re.IGNORECASE)
        if match:
            return f"C# Compiler ({match.group(1)})"
        return "C# Compiler"
    
    # Missing reference/assembly errors
    if any(term in error_line_lower for term in ['could not find', 'missing reference', 'assembly reference', 'nuget package']):
        return "Missing Reference"
    
    # Type/namespace errors
    if any(term in error_line_lower for term in ['does not exist', 'could not be found', 'namespace', 'type']):
        if 'namespace' in error_line_lower or 'type' in error_line_lower:
            return "Type/Namespace Not Found"
    
    # Syntax errors
    if any(term in error_line_lower for term in ['syntax error', 'unexpected', 'expected']):
        return "Syntax Error"
    
    # Version/target framework errors
    if any(term in error_line_lower for term in ['target framework', 'netstandard', 'netcoreapp', 'netframework']):
        return "Framework/Target Error"
    
    # Build/project errors
    if any(term in error_line_lower for term in ['project file', 'build failed', 'msbuild']):
        return "Build/Project Error"
    
    # Generic error patterns
    if 'error' in error_line_lower:
        return "Other Error"
    
    return "Unknown"


def extract_errors_from_file(error_file: Path, max_errors: int = 5) -> List[Tuple[str, str]]:
    """
    Extract error lines from an error file.
    Returns list of (error_category, error_line) tuples.
    """
    errors = []
    
    if not error_file.exists():
        return errors
    
    try:
        with open(error_file, 'r', encoding='utf-8', errors='ignore') as f:
            content = f.read()
            
        # Look for error lines - typically start with file path and contain "error"
        # Pattern: path(line,col): error ...
        error_pattern = r'[^\n]*(?:error|Error|ERROR)[^\n]*'
        error_lines = re.findall(error_pattern, content)
        
        # Limit to max_errors
        for error_line in error_lines[:max_errors]:
            error_line = error_line.strip()
            if error_line and 'error' in error_line.lower():
                category = categorize_error(error_line)
                errors.append((category, error_line))
    
    except Exception as e:
        errors.append(("File Read Error", f"Could not read file: {e}"))
    
    return errors


def analyze_errors(json_file: Path, errors_dir: Path) -> Dict:
    """
    Analyze all error files and return categorized results.
    """
    # Load JSON file
    try:
        with open(json_file, 'r', encoding='utf-8') as f:
            data = json.load(f)
    except Exception as e:
        print(f"Error loading JSON file: {e}", file=sys.stderr)
        sys.exit(1)
    
    # Statistics
    issue_stats = []
    category_counter = Counter()
    total_errors = 0
    
    # Process each failed build
    for item in data.get('failed_builds', []):
        issue_name = item.get('issue', '')
        project_path = item.get('project', '')
        
        if not issue_name or not project_path:
            continue
        
        # Create error file name
        safe_project_name = project_path.replace('\\', '_').replace('/', '_').replace(':', '_')
        error_file = errors_dir / f"{issue_name}_{safe_project_name}_error.txt"
        
        # Extract errors
        errors = extract_errors_from_file(error_file, max_errors=5)
        
        if errors:
            # Count errors by category for this issue
            issue_categories = Counter([cat for cat, _ in errors])
            
            issue_stats.append({
                'issue': issue_name,
                'project': project_path,
                'error_count': len(errors),
                'categories': dict(issue_categories),
                'errors': errors[:5]  # Keep first 5 for display
            })
            
            # Update global category counter
            for category, _ in errors:
                category_counter[category] += 1
            
            total_errors += len(errors)
        else:
            # No errors found in file (might be missing or empty)
            issue_stats.append({
                'issue': issue_name,
                'project': project_path,
                'error_count': 0,
                'categories': {},
                'errors': []
            })
    
    return {
        'issues': issue_stats,
        'category_summary': dict(category_counter),
        'total_errors': total_errors,
        'total_issues': len(issue_stats)
    }


def print_summary_table(results: Dict):
    """
    Print a formatted summary table of errors.
    """
    print("=" * 100)
    print("BUILD ERROR ANALYSIS SUMMARY")
    print("=" * 100)
    print(f"\nTotal Issues Analyzed: {results['total_issues']}")
    print(f"Total Errors Found: {results['total_errors']}")
    
    # Category summary
    print("\n" + "-" * 100)
    print("ERROR CATEGORIES (Summary)")
    print("-" * 100)
    print(f"{'Category':<40} {'Count':<10}")
    print("-" * 100)
    
    for category, count in sorted(results['category_summary'].items(), key=lambda x: x[1], reverse=True):
        print(f"{category:<40} {count:<10}")
    
    # Detailed issue breakdown
    print("\n" + "=" * 100)
    print("ISSUE-BY-ISSUE BREAKDOWN")
    print("=" * 100)
    
    for issue_data in results['issues']:
        issue_name = issue_data['issue']
        project = issue_data['project']
        error_count = issue_data['error_count']
        categories = issue_data['categories']
        
        print(f"\n[{issue_name}]")
        print(f"  Project: {project}")
        print(f"  Total Errors: {error_count}")
        
        if categories:
            print(f"  Error Categories:")
            for cat, count in sorted(categories.items(), key=lambda x: x[1], reverse=True):
                print(f"    - {cat}: {count}")
            
            # Show sample errors (first 3)
            if issue_data['errors']:
                print(f"  Sample Errors (showing first {min(3, len(issue_data['errors']))}):")
                for i, (category, error_line) in enumerate(issue_data['errors'][:3], 1):
                    # Truncate long error lines
                    display_error = error_line[:120] + "..." if len(error_line) > 120 else error_line
                    print(f"    {i}. [{category}] {display_error}")
                
                if len(issue_data['errors']) > 3:
                    print(f"    ... and {len(issue_data['errors']) - 3} more error(s)")
        else:
            print(f"  No errors found in error file (file may be missing or empty)")


def main():
    """Main function."""
    parser = argparse.ArgumentParser(
        description='Analyze build errors from failed-builds.json and categorize them.',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python analyze-build-errors.py
  python analyze-build-errors.py failed-builds.json failed-builds_errors
        """
    )
    parser.add_argument(
        'json_file',
        nargs='?',
        type=str,
        default='failed-builds.json',
        help='JSON file containing failed builds (default: failed-builds.json)'
    )
    parser.add_argument(
        'errors_dir',
        nargs='?',
        type=str,
        default='failed-builds_errors',
        help='Directory containing error files (default: failed-builds_errors)'
    )
    args = parser.parse_args()
    
    repo_root = Path(__file__).parent
    
    # Resolve paths
    json_file = Path(args.json_file)
    if not json_file.is_absolute():
        json_file = repo_root / json_file
    
    errors_dir = Path(args.errors_dir)
    if not errors_dir.is_absolute():
        errors_dir = repo_root / errors_dir
    
    if not json_file.exists():
        print(f"Error: JSON file not found: {json_file}", file=sys.stderr)
        sys.exit(1)
    
    if not errors_dir.exists():
        print(f"Error: Errors directory not found: {errors_dir}", file=sys.stderr)
        print(f"Hint: Run 'python check-builds.py {args.json_file}' first to generate error files.", file=sys.stderr)
        sys.exit(1)
    
    # Analyze errors
    print(f"Analyzing errors from: {json_file}")
    print(f"Reading error files from: {errors_dir}")
    print()
    
    results = analyze_errors(json_file, errors_dir)
    
    # Print summary
    print_summary_table(results)


if __name__ == "__main__":
    try:
        main()
    except KeyboardInterrupt:
        print("\n\nAnalysis interrupted by user.")
        sys.exit(130)
    except Exception as e:
        print(f"\n\nUnexpected error: {e}", file=sys.stderr)
        import traceback
        traceback.print_exc()
        sys.exit(1)

