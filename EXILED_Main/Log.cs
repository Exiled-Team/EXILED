using System.Reflection;

namespace EXILED
{
	public static class Log
	{
		internal static bool debug;
		public static void Info(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[INFO] [{assembly.GetName().Name}] {message}");
		}

		//Used to send DEBUG level messages to the game console. Server must have EXILED_Debug enabled (in the configuration).
		public static void Debug(string message)
		{
			if (!debug)
				return;
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[DEBUG] [{assembly.GetName().Name}] {message} LOGTYPE02");
		}

		public static void Warn(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[WARN] [{assembly.GetName().Name}] {message} LOGTYPE14");
		}

		//Used to send ERROR level messages to the game console. This should be used to send errors only. It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
		public static void Error(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}] {message} LOGTYPE-8");
		}
	}
}