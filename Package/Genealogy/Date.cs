using Genealogy.DateImplementation;

namespace Genealogy
{
  public class Date
  {
    /// <summary>
    /// Parses the specified formal date string representing a GEDCOM date of any kind.
    /// </summary>
    /// <param name="date">The formal date string to parse.</param>
    /// <returns>A new instance defining interface <see cref="IDate"/></returns>
    /// <exception cref="GenealogyException">
    /// Throw if the formal date string is null or empty.
    /// </exception>
    public static IDate Parse(string date)
    {
      if (string.IsNullOrEmpty(date))
      {
        throw new GenealogyException("Invalid Date");
      }

      return new SimpleDate(date);
    }
  }
}
