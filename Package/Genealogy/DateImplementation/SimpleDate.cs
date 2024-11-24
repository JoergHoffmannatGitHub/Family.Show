using System;
using System.Globalization;

namespace Genealogy.DateImplementation
{
  internal class SimpleDate : IDate
  {
    private string _date;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleDate"/> class.
    /// </summary>
    /// <param name="date">The formal date string that describes a simple GEDCOM date.</param>
    internal SimpleDate(string date)
    {
      ParseDate(date);
    }

    public string ToGedcom()
    {
      return _date;
    }

    private void ParseDate(string date)
    {
      // There is a minimum length of 4 characters
      if (date.Length < 4)
      {
        throw new GenealogyException("Invalid Date: Must have at least YYYY");
      }

      _date = date;
    }
  }
}
