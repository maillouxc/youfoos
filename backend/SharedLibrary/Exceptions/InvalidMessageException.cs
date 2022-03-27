using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.SharedLibrary.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class InvalidMessageTypeException : Exception
    {
        public InvalidMessageTypeException(string message) : base(message) {}
    }
}
