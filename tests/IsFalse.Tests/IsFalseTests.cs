namespace IsFalse.Tests;

public class IsFalseTests
{
    [Fact]
    public void IsFalseWhenTrueShouldReturnFalse()
    {
        // Arrange
        var value = true;

        // Act
        var result = value.IsFalse();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsFalseWhenFalseShouldReturnTrue()
    {
        // Arrange
        var value = false;

        // Act
        var result = value.IsFalse();

        // Assert
        result.Should().BeTrue();
    }
}
