using System;

namespace YouFoos.Exceptions
{
    /// <summary>
    /// Exception that should be thrown when an operation that tries to create a resource fails because it already exists.
    /// 
    /// Calling code should try to prevent this exception by ensuring that the resource does not exist before attempting to create it.
    /// </summary>
    public class ResourceAlreadyExistsException : InvalidOperationException
    {
        /// <summary>
        /// Empty constructor.
        /// </summary>
        public ResourceAlreadyExistsException() : base() {}

        /// <summary>
        /// Constructor which accepts an error message.
        /// </summary>
        public ResourceAlreadyExistsException(string message) : base(message) {}

        /// <summary>
        /// Constructor which accepts an error message and an inner exception.
        /// </summary>
        public ResourceAlreadyExistsException(string message, Exception inner) : base(message, inner) {}
    }
}
