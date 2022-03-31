using System;

namespace YouFoos.Exceptions
{
    /// <summary>
    /// Thrown when the YouFoos system receives an invalid RabbitMQ message type given the current state of the system.
    /// </summary>
    public class InvalidMessageTypeException : InvalidOperationException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public InvalidMessageTypeException() : base() {}

        /// <summary>
        /// Constructor which accepts an error message.
        /// </summary>
        public InvalidMessageTypeException(string message) : base(message) {}

        /// <summary>
        /// Constructor which accepts an error message and an inner exception.
        /// </summary>
        public InvalidMessageTypeException(string message, Exception inner) : base(message, inner) {}
    }
}
