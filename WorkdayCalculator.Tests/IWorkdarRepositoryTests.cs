using FluentAssertions;
using WorkdayCalendar.Repositories;
using Xunit;

namespace WorkdayCalculator.Tests;

public class WorkdayRepositoryTests
{
    [Fact]
    public void SetWorkdayStartAndStop_ShouldSetCorrectTimes()
    {
        // Arrange
        var repository = new WorkdayRepository();
        var expectedStart = new TimeSpan(8, 0, 0);
        var expectedEnd = new TimeSpan(17, 0, 0);

        // Act
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);

        // Assert
        repository.WorkdayStart.Should().Be(expectedStart);
        repository.WorkdayEnd.Should().Be(expectedEnd);
    }

    [Fact]
    public void SetHoliday_ShouldAddHoliday()
    {
        // Arrange
        var repository = new WorkdayRepository();
        var holiday = new DateTime(2023, 12, 25);

        // Act
        repository.SetHoliday(holiday);

        // Assert
        repository.Holidays.Should().Contain(holiday);
    }

    [Fact]
    public void SetRecurringHoliday_ShouldAddRecurringHoliday()
    {
        // Arrange
        var repository = new WorkdayRepository();

        // Act
        repository.SetRecurringHoliday(12, 25);

        // Assert
        repository.RecurringHolidays.Should().Contain((12, 25));
    }

    [Theory]
    [InlineData("2023-04-01 09:00", 1, "2023-04-03 09:00")]
    [InlineData("2023-04-01 09:00", -1, "2023-03-31 09:00")]
    [InlineData("2023-04-01 09:00", 0.5, "2023-04-01 13:30")]
    public void GetWorkdayIncrement_ShouldReturnCorrectDate(string startDate, decimal increment, string expectedDate)
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        var startDateTime = DateTime.Parse(startDate);

        // Act
        var result = repository.GetWorkdayIncrement(startDateTime, increment);

        // Assert
        result.Should().Be(DateTime.Parse(expectedDate));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldSkipWeekendsAndHolidays()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        repository.SetHoliday(new DateTime(2023, 4, 14));
        var startDate = new DateTime(2023, 4, 13, 9, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, 1);

        // Assert
        result.Should().Be(new DateTime(2023, 4, 17, 9, 0, 0));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldAccountForRecurringHolidays()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        repository.SetRecurringHoliday(12, 25);
        var startDate = new DateTime(2023, 12, 24, 10, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, 1);

        // Assert
        result.Should().Be(new DateTime(2023, 12, 26, 10, 0, 0));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldHandleNegativeFractionalDays()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        var startDate = new DateTime(2023, 4, 2, 10, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, -1.5m);

        // Assert
        result.Should().Be(new DateTime(2023, 3, 31, 14, 30, 0));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldHandleStartOutsideBusinessHours()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        var startDate = new DateTime(2023, 4, 1, 7, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, 1);

        // Assert
        result.Should().Be(new DateTime(2023, 4, 3, 8, 0, 0));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldHandleEndOutsideBusinessHours()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        var startDate = new DateTime(2023, 4, 1, 18, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, 1);

        // Assert
        result.Should().Be(new DateTime(2023, 4, 3, 8, 0, 0));
    }

    [Fact]
    public void GetWorkdayIncrement_ShouldHandleFractionalDaysOverWeekend()
    {
        // Arrange
        var repository = new WorkdayRepository();
        repository.SetWorkdayStartAndStop(8, 0, 17, 0);
        var startDate = new DateTime(2023, 4, 7, 12, 0, 0);

        // Act
        var result = repository.GetWorkdayIncrement(startDate, 1.5m);

        // Assert
        result.Should().Be(new DateTime(2023, 4, 10, 16, 30, 0));
    }
}
