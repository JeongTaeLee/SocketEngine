using System;
using System.Threading.Tasks;
using SocketEngine.Logging;

namespace SocketEngine.Extensions
{
    public static class AsyncExtensions
    {
        public static Task AsyncRun(this ILoggerProvider loggerProvider, Action task)
        {
            return AsyncRun(loggerProvider, task, TaskCreationOptions.None);
        }

        public static Task AsyncRun(this ILoggerProvider loggerProvider, Action task, TaskCreationOptions taskOption, Action<Exception> exceptionHandler = null)
        {
            return Task.Run(task).ContinueWith((t) =>
            {
                if (exceptionHandler != null)
                    exceptionHandler.Invoke(t.Exception);
                else
                {
                    if (loggerProvider.logger.IsErrorEnabled)
                    {
                        for (var i = 0; i < t.Exception.InnerExceptions.Count; i++)
                        {
                            loggerProvider.logger.Error(t.Exception.InnerExceptions[i]);
                        }
                    }
                }
            }, TaskContinuationOptions.OnlyOnFaulted); // 예외 발생일 경우에만 ContinueWith 호출
        }
    }
}
