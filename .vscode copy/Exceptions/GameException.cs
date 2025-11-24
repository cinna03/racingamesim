using System;

namespace TimeBasedRacingGame.Exceptions
{
    /// <summary>
    /// Custom exception for game-related errors and invalid operations
    /// </summary>
    public class GameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the GameException class
        /// </summary>
        public GameException() : base() { }

        /// <summary>
        /// Initializes a new instance of the GameException class with a specified error message
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public GameException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the GameException class with a specified error message and inner exception
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="innerException">The exception that is the cause of the current exception</param>
        public GameException(string message, Exception innerException) : base(message, innerException) { }
    }
}

