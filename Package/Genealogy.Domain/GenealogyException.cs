using System;

namespace Genealogy.Domain;

public class GenealogyException : Exception
{
  /// <summary>
  /// Initializes a new instance of the <see cref="GenealogyException"/> class.
  /// message.
  /// </summary>
  /// <param name="message">The message that describes the error.</param>
  public GenealogyException(string message)
      : base(message)
  {
  }
}
