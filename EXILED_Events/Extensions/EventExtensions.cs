using System;
using System.Diagnostics;
using System.Reflection;

namespace EXILED.Extensions
{
    public static class EventExtensions
    {
        // end of call stack inner exceptions
        const string endOfInnerExceptionStack = "--- End of inner exception stack trace ---";
        readonly static string endOfInnerExcetionFinalTemplate = $"{Environment.NewLine}   ---> {{0}}{Environment.NewLine}{{1}}";

        /// <summary>
        ///     Executes all delegate listeners safely.
        /// </summary>
        public static void InvokeSafely(this MulticastDelegate action, params object[] args)
        {
            // Just return, this will allow you not to use null-conditional operator at each event
            if (action == null)
                return;

            var eventName = action.GetType().FullName;
            foreach (var handler in action.GetInvocationList())
            {
                HandleSafely(eventName, handler, args);
            }
        }

        /// <summary>
        ///     Executes <see cref="MethodInfo"/> safely.
        /// </summary>
        private static void HandleSafely(string eventName, Delegate @delegate, params object[] args)
        {
            try
            { @delegate.DynamicInvoke(args); }
            catch (Exception ex)
            {
                Log.Error($"Method '{@delegate.Method.Name}' of the class '{@delegate.Method.ReflectedType.FullName}' caused an error when processing the event '{eventName}': {GetExceptionMessage(ex)}");
                // skip five frames
                // 1. System.Reflection.MonoMethod.Invoke
                // 2. System.Reflection.MethodBase.Invoke
                // 3. System.Delegate.DynamicInvokeImpl
                // 4. System.MulticastDelegate.DynamicInvokeImpl
                // 5. Delegate.DynamicInvoke
                var result = GetStackTrace(ex, 5, allowRecursion: false);
                // skip two frames below
                // 1. System.Reflection.MonoMethod.Invoke
                // 2. System.Reflection.MonoMethod.InternalInvoke
                var second = GetStackTrace(ex.InnerException, 2, false);
                if (second != null)
                    // change the order of exceptions according to the call stack
                    result = second + string.Format(endOfInnerExcetionFinalTemplate, result, endOfInnerExceptionStack);
                // insert a new line for a better view of the stack
                Log.Error(Environment.NewLine + result);
            }
        }

        // Just logs an exception without a call stack
        // Literally a copy of `Exception.ToString()` from MSReference/Mono
        private static string GetExceptionMessage(Exception ex)
        {
            var s = $"{ex.GetType()}: {ex.Message}";
            if (ex.InnerException != null)
            {
                // just show the end of the inner exception
                const string endOfInnerException = "--- End of inner exception ---";
                s += string.Format(endOfInnerExcetionFinalTemplate, GetExceptionMessage(ex.InnerException), endOfInnerException);
            }
            return s;
        }

        readonly static FieldInfo stackTrace_frames = typeof(StackTrace).GetField("frames", BindingFlags.NonPublic | BindingFlags.Instance);
        // skips multiple frames and returns the source stack
        private static void StackTraceSkipFrames(StackTrace source, uint frameCount, bool above = true)
        {
            if (frameCount == 0)
                return;

            var sourceArray = (StackFrame[])stackTrace_frames.GetValue(source);
            var finalSize = sourceArray.Length - frameCount;
            if (finalSize > 0)
            {
                // I think the best option would not be to rewrite the parser,
                // but simply change the array that contains the frames
                if (above) Array.Reverse(sourceArray);
                Array.Resize(ref sourceArray, Convert.ToInt32(finalSize));
                stackTrace_frames.SetValue(source, sourceArray);
                if (above) Array.Reverse(sourceArray);
            }
        }

        // gets stacktrace from exception
        private static string GetStackTrace(Exception ex, uint skipFrames, bool above = true, bool allowRecursion = true)
        {
            if (ex == null)
                return null;

            var stacktrace = new StackTrace(ex);
            StackTraceSkipFrames(stacktrace, skipFrames, above);
            var result = stacktrace.ToString();
            if (allowRecursion && ex.InnerException != null)
                result += string.Format(endOfInnerExcetionFinalTemplate, GetStackTrace(ex.InnerException, 0), endOfInnerExceptionStack);
            return result;
        }
    }
}
