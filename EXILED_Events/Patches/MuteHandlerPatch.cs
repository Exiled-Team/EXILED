using GameCore;
using Harmony;
using System.IO;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(MuteHandler), nameof(MuteHandler.Reload))]
	class MuteHandlerPatch
	{
		public static bool Prefix()
		{
			MuteHandler._path = ConfigSharing.Paths[1] + "mutes.txt";
			ServerConsole.AddLog("Loading saved mutes...");
			try
			{
				MuteHandler.Mutes.Clear();
				using (StreamReader streamReader = new StreamReader(MuteHandler._path))
				{
					string text;
					while ((text = streamReader.ReadLine()) != null)
					{
						if (!string.IsNullOrWhiteSpace(text))
						{
							MuteHandler.Mutes.Add(text.Trim());
						}
					}
				}
			}
			catch
			{
				ServerConsole.AddLog("Can't load the mute file!");
			}
			return false;
		}
	}
}