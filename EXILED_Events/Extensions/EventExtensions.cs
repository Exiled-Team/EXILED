using System;
using System.Reflection;

namespace EXILED.Extensions
{
    public static class EventExtensions
    {
        /// <summary>
        ///     Executes all delegate listeners safely.
        /// </summary>
        public static void InvokeSafely(this MulticastDelegate action, params object[] args)
        {
            // Just return, this will allow you not to use null-conditional operator at each event
            if (action == null)
                return;

            foreach (var handler in action.GetInvocationList())
            {
                HandleSafely(handler.Method, args);
            }
        }

        /// <summary>
        ///     Executes <see cref="MethodInfo"/> safely.
        /// </summary>
        private static void HandleSafely(MethodInfo action, params object[] args)
        {
            try
            {
                action.InvokeSafely(null, args);
            }
            catch (Exception ex)
            {
                Log.Error($"{action} Caused an error when processing the event: {ex.Message}");
                Log.Error(ex.StackTrace);
            }
        }
    }
}
