using System;
using EXILED.Shared.Helpers;
using Harmony;
using Console = GameCore.Console;

namespace EXILED.Events.Patches
{
    [HarmonyPatch(typeof(Console), "TypeCommand", new Type[] { typeof(string), typeof(CommandSender) })]
    class ServerCommandPatch
    {
        public static bool Prefix(ref string cmd)
        {
            try
            {
                bool allow = true;
                CommandSender sender = Console._ccs;
                Events.Events.InvokeCommand(ref cmd, ref sender, ref allow);
                return allow;
            }
            catch (Exception e)
            {
                LogHelper.Error($"Server Command event error: {e}");
                return true;
            }

        }
    }
}
