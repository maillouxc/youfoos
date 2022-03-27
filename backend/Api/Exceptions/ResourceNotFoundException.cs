using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.Api.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when an operation depends on a certain resource existing cannot locate the resource.
    /// 
    /// For example, when an attempt is made to register for a tournament that doesn't exist.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ResourceNotFoundException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ResourceNotFoundException(string message) : base(message) {}
    }
}
