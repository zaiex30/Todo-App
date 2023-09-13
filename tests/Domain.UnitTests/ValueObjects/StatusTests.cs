using FluentAssertions;
using NUnit.Framework;
using Todo_App.Domain.Exceptions;
using Todo_App.Domain.ValueObjects;

namespace Todo_App.Domain.UnitTests.ValueObjects;
public class StatusTests
{
    [Test]
    public void ShouldReturnCorrectForDeletionStatus()
    {
        var deletionStatus = "Yes";

        var status = Status.From(deletionStatus);

        status.ForDeletionStatus.Should().Be(deletionStatus);
    }

    [Test]
    public void ToStringReturnsCode()
    {
        var status = Status.No;

        status.ToString().Should().Be(status.ForDeletionStatus);
    }

    [Test]
    public void ShouldPerformImplicitConversionToForDeletionStatusString()
    {
        string status = Status.Yes;

        status.Should().Be("Yes");
    }

    [Test]
    public void ShouldPerformExplicitConversionGivenSupportedStatus()
    {
        var status = (Status)"Yes";

        status.Should().Be(Status.Yes);
    }

    [Test]
    public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
    {
        FluentActions.Invoking(() => Colour.From("Archived"))
            .Should().Throw<UnsupportedColourException>();
    }
}
