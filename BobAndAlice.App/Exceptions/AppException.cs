using System;

namespace BobAndAlice.App.Exceptions
{
    public class AppException : Exception
    {
        public AppException() { }

        public AppException(string message) : base(message) { }

        public AppException(string message, Exception exception) : base(message, exception) { }
    }
}
