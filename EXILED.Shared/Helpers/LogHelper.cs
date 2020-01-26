using System.Reflection;

namespace EXILED.Shared.Helpers
{
    public static class LogHelper //I don't like this, but i'm not rewriting everything.
    {

        //Used to send INFO level messages to the game console.
        public static void Info(string message)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ServerConsole.AddLog($"[INFO] [{assembly.GetName().Name}]: {message}");
        }

        //Used to send DEBUG level messages to the game console. Server must have EXILED_Debug enabled.
        public static void Debug(string message)
        {
#if DEBUG
            Assembly assembly = Assembly.GetCallingAssembly();
            ServerConsole.AddLog($"[DEBUG] [{assembly.GetName().Name}]: {message}");
#endif
        }

        //Used to send ERROR level messages to the game console. This should be used to send errors only. It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
        public static void Error(string message)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}]: {message}");
        }
    }
}
