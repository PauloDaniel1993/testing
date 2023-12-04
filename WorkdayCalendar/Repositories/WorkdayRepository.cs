namespace WorkdayCalendar.Repositories;

public class WorkdayRepository : IWorkdayRepository
{
    private TimeSpan workdayStart;
    private TimeSpan workdayEnd;
    private HashSet<DateTime> holidays = new HashSet<DateTime>();
    private HashSet<(int Month, int Day)> recurringHolidays = new HashSet<(int Month, int Day)>();

    //In normal day to day code I would not add this read only properties, in this case I am adding it only to showcase in tests for this test
    public TimeSpan WorkdayStart => workdayStart;
    public TimeSpan WorkdayEnd => workdayEnd;
    public HashSet<DateTime> Holidays => holidays;
    public HashSet<(int Month, int Day)> RecurringHolidays => recurringHolidays;


    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        workdayStart = new TimeSpan(startHours, startMinutes, 0);
        workdayEnd = new TimeSpan(stopHours, stopMinutes, 0);
    }

    public void SetHoliday(DateTime date)
    {
        holidays.Add(date.Date); // Only consider the date part
    }

    public void SetRecurringHoliday(int month, int day)
    {
        recurringHolidays.Add((month, day));
    }

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        var sign = Math.Sign(incrementInWorkdays);
        var wholeDays = (int)Math.Floor(Math.Abs(incrementInWorkdays));
        var fractionalDay = Math.Abs(incrementInWorkdays) - wholeDays;

        var currentTime = startDate.TimeOfDay;
        var currentDate = startDate.Date;

        // Adjust start time if outside business hours
        if (currentTime < workdayStart)
        {
            currentTime = workdayStart;
        }
        else if (currentTime > workdayEnd)
        {
            currentDate = currentDate.AddDays(sign);
            currentTime = workdayStart;
        }

        // Add whole days
        while (wholeDays > 0)
        {
            currentDate = currentDate.AddDays(sign);
            if (IsWorkday(currentDate))
            {
                wholeDays--;
            }
        }

        // Add fractional day using ticks
        var totalWorkdayTicks = (workdayEnd - workdayStart).Ticks;
        var additionalTicks = totalWorkdayTicks * (double)fractionalDay * sign;
        var finalTime = currentTime + TimeSpan.FromTicks((long)additionalTicks);

        // Adjust for crossing end of workday
        while (finalTime >= workdayEnd || finalTime < workdayStart)
        {
            if (finalTime >= workdayEnd)
            {
                currentDate = currentDate.AddDays(sign);
                finalTime = finalTime - (workdayEnd - workdayStart);
            }
            else if (finalTime < workdayStart)
            {
                currentDate = currentDate.AddDays(sign * -1);
                finalTime = workdayEnd - (workdayStart - finalTime);
            }

            currentDate = AdjustForHoliday(currentDate, sign);
        }

        // Ensure the final time is within the working hours
        if (finalTime > workdayEnd)
        {
            finalTime = workdayEnd;
        }
        else if (finalTime < workdayStart)
        {
            finalTime = workdayStart;
        }

        return currentDate + finalTime;
    }

    private DateTime AdjustForHoliday(DateTime date, int direction)
    {
        while (!IsWorkday(date))
        {
            date = date.AddDays(direction);
        }
        return date;
    }

    private bool IsWorkday(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday &&
               !holidays.Contains(date) &&
               !recurringHolidays.Contains((date.Month, date.Day));
    }
}

public interface IWorkdayRepository
{
    void SetHoliday(DateTime date);
    void SetRecurringHoliday(int month, int day);
    void SetWorkdayStartAndStop(int startHours, int startMinutes,
        int stopHours, int stopMinutes);
    DateTime GetWorkdayIncrement(DateTime startDate, decimal
        incrementInWorkdays);
}

public record WorkTimeHours(int StartWorkHour, int StartWorkMinute, int EndWorkHour, int EndWorkMinute);
