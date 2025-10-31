using System;

namespace Genealogy.Domain;

/// <summary>
/// Represents errors that occur during genealogy-related operations.
/// </summary>
/// <remarks>This exception is typically thrown when an error specific to genealogy processing occurs. It can be
/// used to differentiate genealogy-related errors from other exceptions in the application.</remarks>
public class GenealogyException : Exception
{
  public GenealogyException()
  {
  }

  public GenealogyException(string message)
      : base(message)
  {
  }

  public GenealogyException(string message, Exception inner)
      : base(message, inner)
  {
  }
}
