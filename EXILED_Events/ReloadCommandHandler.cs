namespace EXILED
{
	public static class ReloadCommandHandler
	{
		//Do not use this as plugin command handler, this is only meant to handle the EXILED reload command, handle commands similarly from within your plugin.
		public static void CommandHandler(ref string query, ref CommandSender sender, ref bool allow)
		{
			if (query == "reload")
			{
				allow = false;
				PluginManager.ReloadPlugins();
			}
		}
	}
}