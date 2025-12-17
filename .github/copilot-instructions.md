# Instructions for Copilot in this repository

## ⚠️ CRITICAL: Read BEFORE any code changes ⚠️

**STOP and verify these requirements before writing ANY code:**
- [ ] Will this change need unit tests? (Answer: YES for all new functionality)
- [ ] Am I using ONLY NUnit constraint syntax? (No FluentAssertions, no Shouldly)
- [ ] Have I checked existing code follows these rules? (If not, flag it)

This apply to all files under the Tools directory, and not to the issue folders.

- Use the specifications from the root README.md for all tools developments.
- Don't assume, ask if in doubt.  Assumptions is the mother of all evil.

## Coding

- Use dotnet 10 for all tools, with C# 14 syntax
- Break methods with more than 25 lines into smaller methods
- Use async/await for I/O operations
- Follow standard .NET naming conventions
- Include XML documentation comments for all public methods and classes

- Use dependency injection where appropriate
- Handle exceptions gracefully and log errors using the built-in logging framework
- Ensure code is formatted using `dotnet format` before committing
- Use meaningful variable and method names that convey intent
- For argument parsing, use System.CommandLine version 2.0.0 or later
- All classes that have functions should also have interfaces.


## Testing - NON-NEGOTIABLE REQUIREMENTS

### ✅ MUST DO:
- Write unit tests for ALL new functionality using NUnit
- Use NUnit constraint syntax: `Assert.That(actual, Is.EqualTo(expected))`
- Keep class based tests focused on a single class or component
- Use NSubstitute for mocking interfaces
- Build AND run tests after EACH significant change

### ❌ NEVER USE:
- FluentAssertions (.Should() syntax) - **REMOVE if found in codebase**
- MSTest, XUnit, Moq, Shouldly
- Any assertion library other than NUnit

### When to write each test type:
- **Unit tests**: For every new class, method, or function
- **Component tests**: For interactions between multiple classes, file I/O operations, loading test data from files
- **Integration tests**: For end-to-end scenarios using actual Issue projects (e.g. one per type: framework, netfx, .net (core))

### Component test guidelines:
- Separate from unit tests
- Use available metadata files, copy them to the test project and load them from there

## Build and test

- Always build and run all tests locally after changes.
