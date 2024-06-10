namespace UnitTests;

using Domain;

public class ErrorTests
{
    [Fact]
    public void ErrorWithEmptyCodeAndNonEmptyDescriptionIsCreatedCorrectly()
    {
        // Arrange
        string code = string.Empty;
        string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void ErrorWithEmptyCodeAndNullDescriptionIsCreatedCorrectly()
    {
        // Arrange
        string code = string.Empty;
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void ErrorWithNonEmptyCodeAndNonEmptyDescriptionIsCreatedCorrectly()
    {
        // Arrange
        string code = "Test Code";
        string description = "Test Description";

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Equal(description, error.Description);
    }

    [Fact]
    public void ErrorWithNonEmptyCodeAndNullDescriptionIsCreatedCorrectly()
    {
        // Arrange
        string code = "Test Code";
        string? description = null;

        // Act
        Error error = new(code, description);

        // Assert
        Assert.Equal(code, error.Code);
        Assert.Null(error.Description);
    }

    [Fact]
    public void ErrorWithNoneHasEmptyCodeAndDescription()
    {
        // Arrange
        string expectedCode = string.Empty;
        string expectedDescription = string.Empty;

        // Act
        Error error = Error.None;

        // Assert
        Assert.Equal(expectedCode, error.Code);
        Assert.Equal(expectedDescription, error.Description);
    }
}
