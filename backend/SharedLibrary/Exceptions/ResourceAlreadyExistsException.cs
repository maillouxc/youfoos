using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when trying to create something but it is discovered that it already exists.
    /// 
    /// For example, an operation that creates a user may throw this exception if the user already exists.
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
