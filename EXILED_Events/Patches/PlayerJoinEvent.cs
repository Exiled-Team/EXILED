using System;
using GameCore;
using Harmony;

namespace EXILED.Patches
{
	public class PlayerJoinEvent
	{
    [HarmonyPatch(typeof(CentralAuthInterface), "Ok")]
    public static bool Prefix(CentralAuthInterface __instance, string steamId, string nickname, string ban, string steamban, string server, bool bypass, bool DNT)
    {
      try
      {
        ServerConsole.AddLog("Accepted authentication token of user " + steamId + " with global ban status " + ban +
                             " signed by " + server + " server.");
        __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
          "Accepted your authentication token (your steam id " + steamId + ") with global ban status " + ban +
          " signed by " + server + " server.", "green");
        ServerRoles component = __instance._s.GetComponent<ServerRoles>();
        if (ConfigFile.ServerConfig.GetBool("disable_ban_bypass", false))
          bypass = false;
        if (DNT)
          component.SetDoNotTrack();
        BanDetails key = BanHandler.QueryBan(steamId, (string) null).Key;
        if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && key != null)
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
            "You are banned from this server. Reason: " + key.Reason, "red");
          ServerConsole.AddLog("Player kicked due to local SteamID ban.");
          ServerConsole.Disconnect(__instance._s.connectionToClient,
            "You are banned from this server. Reason: " + key.Reason);
        }
        else if ((!bypass || !ServerStatic.GetPermissionsHandler().IsVerified) && !WhiteList.IsWhitelisted(steamId))
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient, "You are not on the whitelist!", "red");
          ServerConsole.AddLog("Player kicked due to whitelist enabled.");
          ServerConsole.Disconnect(__instance._s.connectionToClient, "You are not on the whitelist for this server.");
        }
        else if ((ConfigFile.ServerConfig.GetBool("use_vac", true) || ServerStatic.PermissionsHandler.IsVerified) &&
                 steamban != "0")
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
            "You have been globally banned: " + steamban + ".", "red");
          ServerConsole.AddLog("Player kicked due to active global ban (" + steamban + ").");
          ServerConsole.Disconnect(__instance._s.connectionToClient,
            "You have been globally banned: " + steamban + ".");
        }
        else if ((ConfigFile.ServerConfig.GetBool("global_bans_cheating", true) ||
                  ServerStatic.PermissionsHandler.IsVerified) && ban == "1")
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
            "You have been globally banned for cheating.", "red");
          ServerConsole.AddLog("Player kicked due to global ban for cheating.");
          ServerConsole.Disconnect(__instance._s.connectionToClient, "You have been globally banned for cheating.");
        }
        else if ((ConfigFile.ServerConfig.GetBool("global_bans_exploiting", true) ||
                  ServerStatic.PermissionsHandler.IsVerified) && ban == "2")
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
            "You have been globally banned for exploiting.", "red");
          ServerConsole.AddLog("Player kicked due to global ban for exploiting.");
          ServerConsole.Disconnect(__instance._s.connectionToClient, "You have been globally banned for exploiting.");
        }
        else if ((ConfigFile.ServerConfig.GetBool("global_bans_griefing", true) ||
                  ServerStatic.PermissionsHandler.IsVerified) && ban == "5")
        {
          __instance._s.TargetConsolePrint(__instance._s.connectionToClient,
            "You have been globally banned for griefing.", "red");
          ServerConsole.AddLog("Player kicked due to global ban for griefing.");
          ServerConsole.Disconnect(__instance._s.connectionToClient, "You have been globally banned for griefing.");
        }
        else
        {
          if (!MuteHandler.QueryPersistantMute(steamId) &&
              (!ConfigFile.ServerConfig.GetBool("global_mutes_voicechat", true) &&
               !ServerStatic.PermissionsHandler.IsVerified || !(ban == "3")) &&
              (!MuteHandler.QueryPersistantMute("ICOM-" + steamId) &&
               (ConfigFile.ServerConfig.GetBool("global_mutes_intercom", true) ||
                ServerStatic.PermissionsHandler.IsVerified)))
          {
            int num = ban == "4" ? 1 : 0;
          }

          Events.InvokePlayerJoin(Plugin.GetPlayer(component.gameObject), ref ban, ref steamban, DNT);
          component.BypassStaff |= bypass;
          if (component.BypassStaff)
            component.GetComponent<CharacterClassManager>().TargetConsolePrint(__instance._s.connectionToClient,
              "You have the ban bypass flag, so you can't be banned from this server.", "cyan");
          component.StartServerChallenge(0);

        }
      }
      catch (Exception e)
      {
        Plugin.Error($"PlayerJoin Event error: {e}");
        return true;
      }

      return false;
    }
  }
}