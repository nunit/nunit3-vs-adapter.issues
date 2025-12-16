# Instructions for Copilot in this repository

This apply to all files under the Tools directory, and not to the issue folders.

- Use the specifications from the root README.md for all tools developments.

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


## Testing

- Write unit tests for all new functionality using NUnit
- Use the NUnit constraint syntax for all tests
- Keep class based tests focused on a single class or component
- Use NSubstitue for mocking
- Create component tests, seperate from unit tests, for testing interactions between multiple classes or components, and for loading test data from files
- Create component tests using available metadata files, copy them to the test project and load them from there
- Create integration tests that reads some of the Issue projects, e.g. one per type that needs to be tests, where type is framework, netfx, .net (core), and where different

## Build and test

- Always build and run all tests locally after changes.
