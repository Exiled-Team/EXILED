// -----------------------------------------------------------------------
// <copyright file="Command.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions.Events
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using Exiled.Permissions.Extensions;
    using Exiled.Permissions.Features;

    /// <summary>
    /// Handles player commands.
    /// </summary>
    public class Command
    {
        /// <inheritdoc cref="Server.OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs)"/>
        public void OnSendingRemoteAdminCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            if (ev.Name.Contains("REQUEST_DATA PLAYER_LIST SILENT"))
                return;

            switch (ev.Name)
            {
                case "ep":
                    ev.IsAllowed = false;

                    if (!ev.Sender.CheckPermission("ep.use"))
                    {
                        ev.Sender.RemoteAdminMessage("No permission.");
                        return;
                    }

                    if (ev.Arguments.Count == 0)
                    {
                        ev.Sender.RemoteAdminMessage("Commands:");
                        ev.Sender.RemoteAdminMessage("- EP RELOAD - Reload permissions.");
                        ev.Sender.RemoteAdminMessage("- EP ADDGROUP <NAME> - Add group.");
                        ev.Sender.RemoteAdminMessage("- EP REMOVEGROUP <NAME> - Remove group.");
                        ev.Sender.RemoteAdminMessage("- EP GROUP <NAME> ADD/REMOVE <PERMISSION> - Add/remove permission from group.");
                        return;
                    }
                    else if (ev.Arguments.Count > 0)
                    {
                        switch (ev.Arguments[1].ToLower())
                        {
                            case "reload":
                                if (!ev.Sender.CheckPermission("ep.reload"))
                                {
                                    ev.Sender.RemoteAdminMessage("No permission.");
                                    return;
                                }

                                Extensions.Permissions.Reload();

                                ev.Sender.RemoteAdminMessage("Plugin reloaded.");
                                return;

                            case "addgroup":
                                if (!ev.Sender.CheckPermission("ep.addgroup"))
                                {
                                    ev.Sender.RemoteAdminMessage("No permission.");
                                    return;
                                }

                                if (ev.Arguments.Count > 1)
                                {
                                    Permissions.Reload();

                                    if (Permissions.Groups.ContainsKey(ev.Arguments[1]))
                                    {
                                        ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " already exists.");
                                        return;
                                    }

                                    Permissions.Groups.Add(ev.Arguments[1], new Group());
                                    Permissions.Save();

                                    ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " has been added.");
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage("EP ADDGROUP <NAME>");
                                }

                                return;

                            case "removegroup":
                                if (!ev.Sender.CheckPermission("ep.removegroup"))
                                {
                                    ev.Sender.RemoteAdminMessage("No permission.");
                                    return;
                                }

                                if (ev.Arguments.Count > 1)
                                {
                                    Permissions.Reload();

                                    if (!Permissions.Groups.ContainsKey(ev.Arguments[1]))
                                    {
                                        ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " does not exist.");
                                        return;
                                    }

                                    Permissions.Groups.Remove(ev.Arguments[1]);
                                    Permissions.Save();
                                    ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " removed.");
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage("EP REMOVEGROUP <NAME>");
                                }

                                return;

                            case "group":
                                if (!ev.Sender.CheckPermission("ep.group"))
                                {
                                    ev.Sender.RemoteAdminMessage("No permission.");
                                    return;
                                }

                                if (ev.Arguments.Count > 2)
                                {
                                    switch (ev.Arguments[2].ToLower())
                                    {
                                        case "add":
                                            if (!ev.Sender.CheckPermission("ep.addpermission"))
                                            {
                                                ev.Sender.RemoteAdminMessage("No permission.");
                                                return;
                                            }

                                            if (ev.Arguments.Count == 3)
                                            {
                                                ev.Sender.RemoteAdminMessage("EP GROUP <NAME> ADD <PERMISSION>");
                                                return;
                                            }

                                            Permissions.Reload();

                                            if (!Permissions.Groups.ContainsKey(ev.Arguments[1]))
                                            {
                                                ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " does not exist.");
                                                return;
                                            }

                                            Permissions.Groups.TryGetValue(ev.Arguments[1], out Group val);

                                            if (!val.Permissions.Contains(ev.Arguments[3]))
                                                val.Permissions.Add(ev.Arguments[3]);

                                            Permissions.Save();

                                            ev.Sender.RemoteAdminMessage("Permission " + ev.Arguments[3] + " for group " + ev.Arguments[1] + " added.");
                                            return;

                                        case "remove":
                                            if (!ev.Sender.CheckPermission("ep.removepermission"))
                                            {
                                                ev.Sender.RemoteAdminMessage("No permission.");
                                                return;
                                            }

                                            if (ev.Arguments.Count == 3)
                                            {
                                                ev.Sender.RemoteAdminMessage("EP GROUP <NAME> REMOVE <PERMISSION>");
                                                return;
                                            }

                                            Permissions.Reload();

                                            if (!Permissions.Groups.ContainsKey(ev.Arguments[1]))
                                            {
                                                ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " does not exist.");
                                                return;
                                            }

                                            Permissions.Groups.TryGetValue(ev.Arguments[1], out Group group);

                                            if (group.Permissions.Contains(ev.Arguments[3]))
                                                group.Permissions.Remove(ev.Arguments[3]);

                                            Permissions.Save();

                                            ev.Sender.RemoteAdminMessage("Permission " + ev.Arguments[3] + " for group " + ev.Arguments[1] + " removed.");
                                            return;
                                    }
                                }
                                else if (ev.Arguments.Count > 1)
                                {
                                    if (!Permissions.Groups.ContainsKey(ev.Arguments[1]))
                                    {
                                        ev.Sender.RemoteAdminMessage("Group " + ev.Arguments[1] + " does not exist.");
                                        return;
                                    }

                                    Permissions.Groups.TryGetValue(ev.Arguments[1], out Group group);

                                    ev.Sender.RemoteAdminMessage("Group: " + ev.Arguments[1]);
                                    ev.Sender.RemoteAdminMessage("Default: " + group.IsDefault);

                                    if (group.Inheritance.Count != 0)
                                    {
                                        ev.Sender.RemoteAdminMessage("Inheritance: ");
                                        foreach (string inheritance in group.Inheritance)
                                        {
                                            ev.Sender.RemoteAdminMessage("- " + inheritance);
                                        }
                                    }

                                    if (group.Inheritance.Count != 0)
                                    {
                                        ev.Sender.RemoteAdminMessage("Permissions: ");
                                        foreach (string permission in group.Permissions)
                                        {
                                            ev.Sender.RemoteAdminMessage("- " + permission);
                                        }
                                    }
                                }
                                else
                                {
                                    ev.Sender.RemoteAdminMessage("EP GROUP <NAME>");
                                    ev.Sender.RemoteAdminMessage("EP GROUP <NAME> ADD/REMOVE <PERMISSION>");
                                }

                                return;

                            default:
                                ev.Sender.RemoteAdminMessage("Commands:");
                                ev.Sender.RemoteAdminMessage("- EP RELOAD - Reload permissions.");
                                ev.Sender.RemoteAdminMessage("- EP ADDGROUP <NAME> - Add group.");
                                ev.Sender.RemoteAdminMessage("- EP REMOVEGROUP <NAME> - Remove group.");
                                ev.Sender.RemoteAdminMessage("- EP GROUP <NAME> - Info about group.");
                                ev.Sender.RemoteAdminMessage("- EP GROUP <NAME> ADD/REMOVE <PERMISSION> - Add/remove permission from group.");
                                return;
                        }
                    }

                    break;
            }
        }
    }
}
