using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.SharedLibrary.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class YouFoosException : Exception
    {
        public YouFoosException(string message) : base(message) {}
    }
}
