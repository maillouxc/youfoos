using System;

namespace YouFoos.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when an operation depends on a certain resource existing cannot locate the resource.
    /// 
    /// Calling code should try to prevent this exception by ensuring that the resource exists before attempting to retrieve it.
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
