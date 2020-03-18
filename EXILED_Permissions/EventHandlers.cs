using EXILED.Extensions;

namespace EXILED
{
	public class EventHandlers
	{
		public static void RemoteAdminCommandEvent(ref RACommandEvent ev)
		{
			if (ev.Command.Contains("REQUEST_DATA PLAYER_LIST SILENT"))
				return;
			string[] args = ev.Command.Split(' ');
			ReferenceHub sender = ev.Sender.SenderId == "SERVER CONSOLE" || ev.Sender.SenderId == "GAME CONSOLE" ? Player.GetPlayer(PlayerManager.localPlayer) : Player.GetPlayer(ev.Sender.SenderId);

			switch (args[0].ToLower())
			{
				case "ep":
					ev.Allow = false;
					if (!sender.CheckPermission("ep.use"))
					{
						ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
						return;
					}
					if (args.Length == 1)
					{
						ev.Sender.RaReply("ExiledPermissions#Commands:", true, true, string.Empty);
						ev.Sender.RaReply("ExiledPermissions# - EP RELOAD - Reload permissions.", true, true, string.Empty);
						ev.Sender.RaReply("ExiledPermissions# - EP ADDGROUP <NAME> - Add group.", true, true, string.Empty);
						ev.Sender.RaReply("ExiledPermissions# - EP REMOVEGROUP <NAME> - Remove group.", true, true, string.Empty);
						ev.Sender.RaReply("ExiledPermissions# - EP GROUP <NAME> ADD/REMOVE <PERMISSION> - Add/remove permission from group.", true, true, string.Empty);
						return;
					}
					else if (args.Length > 1)
					{
						switch (args[1].ToLower())
						{
							case "reload":
								if (!sender.CheckPermission("ep.reload"))
								{
									ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
									return;
								}
								PermissionPlugin.ReloadPermissions();
								ev.Sender.RaReply("ExiledPermissions#Plugin reloaded.", true, true, string.Empty);
								return;
							case "addgroup":
								if (!sender.CheckPermission("ep.addgroup"))
								{
									ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
									return;
								}
								if (args.Length > 2)
								{
									PermissionPlugin.ReloadPermissions();
									if (PermissionPlugin.permissionsconfig.groups.ContainsKey(args[2]))
									{
										ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " already exists.", true, true, string.Empty);
										return;
									}
									PermissionPlugin.permissionsconfig.groups.Add(args[2], new PermissionPlugin.Group());
									PermissionPlugin.SavePermissions();
									ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " has been added.", true, true, string.Empty);
								}
								else
								{
									ev.Sender.RaReply("ExiledPermissions#EP ADDGROUP <NAME>", true, true, string.Empty);
								}
								return;
							case "removegroup":
								if (!sender.CheckPermission("ep.removegroup"))
								{
									ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
									return;
								}
								if (args.Length > 2)
								{
									PermissionPlugin.ReloadPermissions();
									if (!PermissionPlugin.permissionsconfig.groups.ContainsKey(args[2]))
									{
										ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " does not exist.", true, true, string.Empty);
										return;
									}
									PermissionPlugin.permissionsconfig.groups.Remove(args[2]);
									PermissionPlugin.SavePermissions();
									ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " removed.", true, true, string.Empty);
								}
								else
								{
									ev.Sender.RaReply("ExiledPermissions#EP REMOVEGROUP <NAME>", true, true, string.Empty);
								}
								return;
							case "group":
								if (!sender.CheckPermission("ep.group"))
								{
									ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
									return;
								}
								if (args.Length > 3)
								{
									switch (args[3].ToLower())
									{
										case "add":
											if (!sender.CheckPermission("ep.addpermission"))
											{
												ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
												return;
											}
											if (args.Length == 4)
											{
												ev.Sender.RaReply("ExiledPermissions#EP GROUP <NAME> ADD <PERMISSION>", true, true, string.Empty);
												return;
											}
											PermissionPlugin.ReloadPermissions();
											if (!PermissionPlugin.permissionsconfig.groups.ContainsKey(args[2]))
											{
												ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " does not exist.", true, true, string.Empty);
												return;
											}
											PermissionPlugin.permissionsconfig.groups.TryGetValue(args[2], out PermissionPlugin.Group val);
											if (!val.permissions.Contains(args[4]))
												val.permissions.Add(args[4]);
											PermissionPlugin.SavePermissions();
											ev.Sender.RaReply("ExiledPermissions#Permission " + args[4] + " for group " + args[2] + " added.", true, true, string.Empty);
											return;
										case "remove":
											if (!sender.CheckPermission("ep.removepermission"))
											{
												ev.Sender.RaReply("ExiledPermissions#No permission.", true, true, string.Empty);
												return;
											}
											if (args.Length == 4)
											{
												ev.Sender.RaReply("ExiledPermissions#EP GROUP <NAME> REMOVE <PERMISSION>", true, true, string.Empty);
												return;
											}
											PermissionPlugin.ReloadPermissions();
											if (!PermissionPlugin.permissionsconfig.groups.ContainsKey(args[2]))
											{
												ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " does not exist.", true, true, string.Empty);
												return;
											}
											PermissionPlugin.permissionsconfig.groups.TryGetValue(args[2], out PermissionPlugin.Group val2);
											if (val2.permissions.Contains(args[4]))
												val2.permissions.Remove(args[4]);
											PermissionPlugin.SavePermissions();
											ev.Sender.RaReply("ExiledPermissions#Permission " + args[4] + " for group " + args[2] + " removed.", true, true, string.Empty);
											return;
									}
								}
								else if (args.Length > 2)
								{
									if (!PermissionPlugin.permissionsconfig.groups.ContainsKey(args[2]))
									{
										ev.Sender.RaReply("ExiledPermissions#Group " + args[2] + " does not exist.", true, true, string.Empty);
										return;
									}
									PermissionPlugin.permissionsconfig.groups.TryGetValue(args[2], out PermissionPlugin.Group val2);
									ev.Sender.RaReply("ExiledPermissions#Group: " + args[2], true, true, string.Empty);
									ev.Sender.RaReply("ExiledPermissions# Default: " + val2.Default, true, true, string.Empty);
									if (val2.inheritance.Count != 0)
									{
										ev.Sender.RaReply("ExiledPermissions# Inheritance: ", true, true, string.Empty);
										foreach (string per in val2.inheritance)
										{
											ev.Sender.RaReply("ExiledPermissions# - " + per, true, true, string.Empty);
										}
									}
									if (val2.permissions.Count != 0)
									{
										ev.Sender.RaReply("ExiledPermissions# Permissions: ", true, true, string.Empty);
										foreach (string per in val2.permissions)
										{
											ev.Sender.RaReply("ExiledPermissions# - " + per, true, true, string.Empty);
										}
									}
								}
								else
								{
									ev.Sender.RaReply("ExiledPermissions#EP GROUP <NAME>", true, true, string.Empty);
									ev.Sender.RaReply("ExiledPermissions#EP GROUP <NAME> ADD/REMOVE <PERMISSION>", true, true, string.Empty);
								}
								return;
							default:
								ev.Sender.RaReply("ExiledPermissions#Commands:", true, true, string.Empty);
								ev.Sender.RaReply("ExiledPermissions# - EP RELOAD - Reload permissions.", true, true, string.Empty);
								ev.Sender.RaReply("ExiledPermissions# - EP ADDGROUP <NAME> - Add group.", true, true, string.Empty);
								ev.Sender.RaReply("ExiledPermissions# - EP REMOVEGROUP <NAME> - Remove group.", true, true, string.Empty);
								ev.Sender.RaReply("ExiledPermissions# - EP GROUP <NAME> - Info about group.", true, true, string.Empty);
								ev.Sender.RaReply("ExiledPermissions# - EP GROUP <NAME> ADD/REMOVE <PERMISSION> - Add/remove permission from group.", true, true, string.Empty);
								return;
						}
					}
					break;
			}
		}
	}
}
