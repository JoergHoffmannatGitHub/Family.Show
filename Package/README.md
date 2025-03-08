# Genealogy Date
This is a implementation of [Genealogy Dates](?) - non-normalized durations.
It also includes some handy utility functions for manupulating ranges, durations, and recurring dates.

# NuGet Package Information (future)


| Package ID | Link |
|------------|------|
| Gedcomx.Date | http://www.nuget.org/packages/?/ |

See [the section on using these libraries](../README.md#Use).

To determine the latest version visit the NuGet link listed above.

# Exceptions
Every error thrown is an instance of `GenealogyException`, which is a *runtime* exception.
Genealogy Date was designed to give you and/or the end user as much information as possible.
For instance, when parsing fails, the message in the exception will tell you exactly what failed and where.

# Date Types
Each date implements the interface `IDate`.

````csharp
bool result = Date.TryParse("BET 1982 AND 1984", out IDate date);
// returns true if the date string was parsed successfully

date.ToGedcom();
// returns BET 1982 AND 1984

````

**Simple**

The most basic of dates.
Stores simply the given string. This class is used temporyrly to store the date string until full implementation.

````csharp
SimpleDate simple = new SimpleDate("something wrong");

x = simple.ToGedcom();
// returns something wrong
````

**Exact**

The most basic of dates.
For example, `1000`, `JAN 1823`, `6 JUL 1946`, and `1946-07-06T00:00:00` are all examples of simple dates.

````csharp
DateExact exact = new("1835");

x = exact.Year;
// returns 1835

x = exact.Month;
// returns 0

simple.ToGedcom();
// returns 1835
````

**Range**

"BEF ", "AFT ", "BET "

**Period**

"FROM ", "TO "

**Approximated**

"ABT ", "CAL ", "EST "

**Duration**

A special sub-date of sorts, a duration represents the amount of time that has passed from a given starting date.
You can get the duration from a range or recurring

````csharp
GedcomxDateDuration duration = new GedcomxDateRecurring("P1Y35D");

duration.Years;
// returns 1

duration.Months;
// returns null

duration.Days;
// returns 35

duration.ToFormalString();
// returns P1Y35D
````

# Utility functions
All of these functions are static under the `GedcomxDateUtil` class

**parse(date)**
Parse a formal date string into the appropriate date type

````csharp
GedcomxDate date = GedcomxDateUtil.Parse("A+1900");
// date is an instance of GedcomxDateApproximate

GedcomxDate date = GedcomxDateUtil.Parse("R3/+1900/P1Y");
// date is an instance of GedcomxDateRecurring

GedcomxDate date = GedcomxDateUtil.Parse("Bogus");
// throws an instance of GedcomxDateException
````

**GetDuration(startDate, endDate)**
Get the duration between two simple dates. Throws an exception if start >= end.

````csharp
GedcomxDateDuration duration = GedcomxDateUtil.GetDuration(new GedcomxDateSimple("+1000"),new GedcomxDateSimple("+1100"));
// returns a duration of P100Y
````

**AddDuration(startDate, duration)**
Add a duration to a simple starting date and returns the resulting SimpleDate.

````csharp
GedcomxDateSimple date = GedcomxDateUtil.AddDuration(new GedcomxDateSimple("+1000"),new GedcomxDateDuration("P1Y3D"));
// returns a date of +1001-01-04
````


**MultiplyDuration(duration, multiplier)**
Multiply a duration by an integer value

````csharp
GedcomxDateduration duration = GedcomxDateUtil.MultiplyDuration(new GedcomxDateDuration("P1Y3D"),4);
// returns a date of P4Y12D
````

**DaysInMonth(month, year)**
Returns the number of days in the given month accounting for the year (leap year or not).

````csharp
int days;
days = DateTime.DaysInMonth(2003, 2);
// returns 28, non leap year
days = DateTime.DaysInMonth(2004, 2); 
// returns 29, leap year
days = DateTime.DaysInMonth(1900, 2);
// returns 28, non leap year
days = DateTime.DaysInMonth(2000, 2);
// returns 29, leap year
````
