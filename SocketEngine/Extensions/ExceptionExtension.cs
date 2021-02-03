using System;

namespace SocketEngine.Extensions
{
    internal static class ExceptionExtension
    {
        public static void ExceptionIfTrue(bool value, string message = "Exception thrown")
        {
            if (value)
                throw new Exception(message);
        }

        public static void ExceptionIfFalse(bool value, string message = "Exception thrown")
        {
            if (!value)
                throw new Exception(message);
        }

        public static void ExceptionIfNull<T>(T value, string message = "Exception thrown") where T : class
        {
            if (value == null)
                throw new Exception(message);
        }

        public static void ExceptionIfNoneNull<T>(T value, string message = "Exception thrown") where T : class
        {
            if (value != null)
                throw new Exception(message);
        }

        public static void ArgumentExceptionIfTrue(bool value, string parameterName, string message = "")
        {
            if (value)
            {
                throw string.IsNullOrEmpty(message)
                    ? new ArgumentException(parameterName)
                    : new ArgumentException(message, parameterName);
            }
        }

        public static void ArgumentExceptionIfFalse(bool value, string parameterName, string message = "")
        {
            if (!value)
            {
                throw string.IsNullOrEmpty(message)
                    ? new ArgumentException(parameterName)
                    : new ArgumentException(message, parameterName);
            }
        }

        public static void ArgumentNullExceptionIfNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void ArgumentExceptionIsNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException($"{parameterName} is null or empty");
        }

        public static void ArgumentExceptionIfLessThanOrEqualsToZero(int value, string parameterName)
        {
            ArgumentExceptionIfTrue(value <= 0, parameterName: parameterName, message: "Parameter cannot be less than or equal to zero.");
        }

    }
}
