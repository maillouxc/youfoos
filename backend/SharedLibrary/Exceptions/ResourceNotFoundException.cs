using System;

namespace YouFoos.Api.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when an operation depends on a certain resource existing cannot locate the resource.
    /// 
    /// For example, when an attempt is made to register for a tournament that doesn't exist.
    /// </summary>
    public class ResourceNotFoundException : InvalidOperationException
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ResourceNotFoundException() : base() {}

        /// <summary>
        /// Constructor which accepts an error message.
        /// </summary>
        public ResourceNotFoundException(string message) : base(message) {}

        /// <summary>
        /// Constructor which accepts an error message and an inner exception.
        /// </summary>
        public ResourceNotFoundException(string message, Exception inner) : base(message, inner) {}
    }
}
