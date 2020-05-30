using System;
using System.IO;
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
                HandleSafely(action.GetType().FullName, handler.Method, handler.Target,  args);
            }
        }

        /// <summary>
        ///     Executes <see cref="MethodInfo"/> safely.
        /// </summary>
        /// <param name="instance">
        ///     Instance of the delegate method object,
        ///     null is used in the case of a static method.
        /// </param>
        private static void HandleSafely(string eventName, MethodInfo action, object instance, params object[] args)
        {
            try
            {
                action.Invoke(instance, args);
            }
            catch (Exception ex)
            {
                // instance is null for static methods, we shouldn't use it
                Log.Error($"Method '{action.Name}' of the class '{action.ReflectedType.FullName}' caused an error when processing the event '{eventName}': {ex.InnerException} {ex.Message}");
                Log.Error(ex.StackTrace);
            }
        }
    }
}
