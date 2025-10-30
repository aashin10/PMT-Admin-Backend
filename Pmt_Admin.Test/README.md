# PMT Admin - Unit Tests

This directory contains comprehensive unit tests for the PMT Admin Backend application.

## Test Structure

```
Pmt_Admin.Test/
??? Handlers/
?   ??? Projects/
?   ?   ??? GetAllProjectsQueryHandlerTests.cs
?   ?   ??? GetProjectByIdQueryHandlerTests.cs
?   ?   ??? DeleteProjectCommandHandlerTests.cs
?   ??? ProjectStatus/
?       ??? GetAllProjectStatusQueryHandlerTests.cs
```

## Test Coverage

### Project Handlers

#### `GetAllProjectsQueryHandlerTests`
Tests for retrieving all projects with complete details.

**Test Cases:**
- ? `Handle_WhenProjectsExist_ReturnsSuccessWithProjects` - Verifies successful retrieval with all nested data (teams, members, sprints, custom fields)
- ? `Handle_WhenNoProjectsExist_ReturnsSuccessWithEmptyList` - Verifies empty list response when no projects exist
- ? `Handle_WhenProjectsHaveNullCollections_ReturnsProjectsWithEmptyCollections` - Tests null-safety for collections
- ? `Handle_WhenMultipleProjects_ReturnsAllProjects` - Verifies handling of multiple projects

**Key Assertions:**
- Project count verification
- Team size calculation
- Sprint count accuracy
- Custom fields (additional information) mapping
- Teams and team members data structure
- Delivery unit code inclusion
- Total project count in response message

#### `GetProjectByIdQueryHandlerTests`
Tests for retrieving a single project by ID.

**Test Cases:**
- ? `Handle_WhenProjectExists_ReturnsSuccessWithProject` - Verifies complete project retrieval
- ? `Handle_WhenProjectDoesNotExist_ReturnsNotFound` - Tests 404 response for missing projects
- ? `Handle_WhenProjectHasNullCollections_ReturnsProjectWithEmptyCollections` - Tests null-safety
- ? `Handle_WhenProjectHasCompleteData_MapsAllFieldsCorrectly` - Comprehensive field mapping verification

**Key Assertions:**
- All project fields (name, key, description, customer info)
- Navigation properties (ProjectManager, Status, DeliveryUnit)
- Collections (ProjectMembers, Sprints, CustomFields, Teams)
- Delivery unit code mapping
- Team size and sprint count calculations
- Custom field DTOs
- Team member DTOs with role and team assignment

#### `DeleteProjectCommandHandlerTests`
Tests for soft-deleting projects.

**Test Cases:**
- ? `Handle_WhenProjectExists_ReturnsSuccess` - Verifies successful soft deletion
- ? `Handle_WhenProjectDoesNotExist_ReturnsNotFound` - Tests 404 response
- ? `Handle_WhenRepositoryThrowsException_PropagatesException` - Exception handling verification

### Project Status Handlers

#### `GetAllProjectStatusQueryHandlerTests`
Tests for retrieving all project statuses.

**Test Cases:**
- ? `Handle_WhenStatusesExist_ReturnsSuccessWithStatuses` - Verifies status retrieval with AutoMapper
- ? `Handle_WhenNoStatusesExist_ReturnsSuccessWithEmptyList` - Empty list handling
- ? `Handle_WhenStatusesIsNull_ReturnsSuccessWithEmptyList` - Null handling
- ? `Handle_WhenSingleStatus_ReturnsSingleStatus` - Single item verification
- ? `Handle_WhenMapperReturnsNull_ReturnsSuccessWithEmptyList` - AutoMapper null handling
- ? `Handle_WhenRepositoryThrowsException_PropagatesException` - Exception propagation

## Technologies Used

- **xUnit** - Testing framework
- **Moq** - Mocking framework for dependencies
- **FluentAssertions** - Fluent assertion library for readable tests
- **AutoMapper** - Object mapping (where applicable)

## Running the Tests

### Visual Studio
1. Open Test Explorer (Test ? Test Explorer)
2. Click "Run All" to execute all tests
3. View results in the Test Explorer window

### Command Line
```bash
dotnet test
```

### With Coverage
```bash
dotnet test /p:CollectCoverage=true
```

## Test Patterns

### Arrange-Act-Assert (AAA)
All tests follow the AAA pattern:
```csharp
// Arrange - Set up test data and mocks
var projectId = Guid.NewGuid();
_repositoryMock.Setup(x => x.GetById(projectId)).ReturnsAsync(project);

// Act - Execute the method being tested
var result = await _handler.Handle(query, CancellationToken.None);

// Assert - Verify the results
result.Status.Should().Be(200);
result.Data.Should().NotBeNull();
```

### Mocking Strategy
- Repository interfaces are mocked using Moq
- All external dependencies are isolated
- Tests verify method calls using `Verify()`

### Assertions
- FluentAssertions provides readable assertions
- Verifies both success and failure scenarios
- Checks all relevant properties in response objects

## Key Features Tested

### Projects API
- ? Complete project retrieval with nested entities
- ? Team size calculation from ProjectMembers
- ? Sprint count from Sprints collection
- ? Custom fields (additional information) mapping
- ? Teams list with ID and name
- ? Team members with role, email, and team assignment
- ? Delivery unit code inclusion
- ? Total project count in response
- ? Null-safety for optional collections
- ? Project soft deletion
- ? 404 handling for missing projects

### Project Status API
- ? Status list retrieval with AutoMapper
- ? Empty result handling
- ? Single and multiple status scenarios
- ? Exception handling

## Future Enhancements

Potential areas for additional test coverage:
- Integration tests with real database
- Create/Update project command handlers
- Project validation logic
- Performance tests for large datasets
- Edge cases for data limits
- Concurrency scenarios

## Contributing

When adding new tests:
1. Follow the AAA pattern
2. Use descriptive test names (e.g., `Handle_WhenCondition_ExpectedBehavior`)
3. Include both success and failure scenarios
4. Test edge cases and null handling
5. Verify all mock interactions
6. Use FluentAssertions for readability

## Notes

- All tests use in-memory data (no database required)
- Repository methods are mocked for isolation
- Tests are independent and can run in any order
- Build must pass before running tests
- Sprint IDs are Guid type (not int)
- ProjectStatus is fully qualified to avoid namespace conflicts
