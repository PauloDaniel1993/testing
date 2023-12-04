# To run this test

Simply run the `WorkdayCalculator.Presentation` project in an IDE or using the command `dotnet run`.

When prompted, past your json in the terminal, in one line. For example:

`{"WorkdayStart":{"Hours":8,"Minutes":0},"WorkdayStop":{"Hours":16,"Minutes":0},"RecurringHolidays":[{"Month":5,"Day":17}],"Holidays":[{"Year":2004,"Month":5,"Day":27}],"StartDate":"24-05-2004 18:03","Increment":-6.7470217}`

# To run the unit tests

Simply run it in one IDE or run `dotnet test` in terminal. The tesdts are located in WorkdayCalculator.Tests

# Important

Sadly I could not handle all the cases with the desired precision. The  case `{"WorkdayStart":{"Hours":8,"Minutes":
0},"WorkdayStop":{"Hours":16,"Minutes":0},"RecurringHolidays":[{"Mont
h":
5,"Day":17}],"Holidays":[{"Year":2004,"Month":5,"Day":
27}],"StartDate":"24-05-2004 18:03","Increment":-6.7470217}` is returning 13-05-2004 10:01 instead of 13-05-2004 10:02. I could not find the reason.
