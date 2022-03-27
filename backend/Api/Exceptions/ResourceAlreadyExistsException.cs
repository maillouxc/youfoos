using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when trying to create something but it is discovered that it already exists.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ResourceAlreadyExistsException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceAlreadyExistsException(string message) : base(message) {}
    }
}
