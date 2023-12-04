// See https://aka.ms/new-console-template for more information

using WorkdayCalendar.Models;
using WorkdayCalendar.Repositories;
using System.Text.Json;
using WorkdayCalendar.Utils;

Console.WriteLine("Enter your configuration in one line: ");

var json = Console.ReadLine();

var options = new JsonSerializerOptions
{
    Converters = { new CustomDateTimeConverter("dd-MM-yyyy HH:mm") }
};

var configuration = JsonSerializer.Deserialize<ScheduleEntry>(json!, options);

var repo = new WorkdayRepository();

repo.SetWorkdayStartAndStop(
    configuration!.WorkdayStart.Hours,
    configuration.WorkdayStart.Minutes,
    configuration.WorkdayStop.Hours,
    configuration.WorkdayStop.Minutes);

configuration.Holidays.ForEach(
    holiday =>
        repo.SetHoliday(
            new DateTime(
                holiday.Year,
                holiday.Month,
                holiday.Day
            )));

configuration.RecurringHolidays.ForEach(
    rh =>
        repo.SetRecurringHoliday(
            rh.Month,
            rh.Day
        ));

var result = repo.GetWorkdayIncrement(configuration.StartDate, configuration.Increment);

var formattedStartDate = configuration.StartDate.ToString("dd-MM-yyyy HH:mm");
var formattedResult = result.ToString("dd-MM-yyyy HH:mm");

Console.WriteLine($"{formattedStartDate} with an addition of {configuration.Increment} work days is {formattedResult}");


