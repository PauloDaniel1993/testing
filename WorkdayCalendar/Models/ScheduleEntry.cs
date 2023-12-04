namespace WorkdayCalendar.Models;

public class ScheduleEntry
{
    public Time WorkdayStart { get; set; }
    public Time WorkdayStop { get; set; }
    public List<Holiday> RecurringHolidays { get; set; }
    public List<Holiday> Holidays { get; set; }
    public DateTime StartDate { get; set; }
    public decimal Increment { get; set; }
}

public class Time
{
    public int Hours { get; set; }
    public int Minutes { get; set; }
}

public class Holiday
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int Day { get; set; }
}
