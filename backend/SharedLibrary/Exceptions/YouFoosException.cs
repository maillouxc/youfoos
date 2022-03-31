using System;

namespace YouFoos.Exceptions
{
    /// <summary>
    /// Represents a business logic error in the YouFoos system.
    /// </summary>
    public class YouFoosException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public YouFoosException() : base() {}

        /// <summary>
        /// Constructor which accepts an error message.
        /// </summary>
        public YouFoosException(string message) : base(message) {}

        /// <summary>
        /// Constructor which accepts an error message and an inner exception.
        /// </summary>
        public YouFoosException(string message, Exception inner) : base(message, inner) {}
    }
}
