using EXILED_Permissions.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EXILED
{
	public class PermissionPlugin : Plugin
	{
		public static YML permissionsconfig;
		public static string pluginDir;
		public override void OnEnable()
		{
			string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			pluginDir = Path.Combine(appData, "Plugins", "Exiled Permissions");
			if (!Directory.Exists(pluginDir))
			{
				Log.Warn("Permissions directory missing, creating.");
				Directory.CreateDirectory(pluginDir);
			}

			if (!File.Exists(Path.Combine(pluginDir, "permissions.yml")))
			{
				Log.Warn("Permissions file missing, creating.");
				File.WriteAllText(Path.Combine(pluginDir, "permissions.yml"),
					Encoding.UTF8.GetString(Resources.permissions));
			}

			ReloadPermissions();
			Events.RemoteAdminCommandEvent += EventHandlers.RemoteAdminCommandEvent;
		}

		public static void ReloadPermissions()
		{
			string yml = File.ReadAllText(Path.Combine(pluginDir, "permissions.yml"));
			var deserializer = new DeserializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
			permissionsconfig = deserializer.Deserialize<YML>(yml);
		}

		public static void SavePermissions()
		{
			var serializer = new SerializerBuilder()
				.WithNamingConvention(CamelCaseNamingConvention.Instance)
				.Build();
			File.WriteAllText(Path.Combine(pluginDir, "permissions.yml"), serializer.Serialize(permissionsconfig));
		}

		public static Group GetDefaultGroup()
		{
			foreach (KeyValuePair<string, Group> gr in permissionsconfig.groups)
			{
				if (gr.Value.Default)
					return gr.Value;
			}
			return null;
		}

		public static bool CheckPermission(ReferenceHub hub, string permission)
		{
			if (hub == null)
			{
				Log.Error("Reference hub was null, unable to check permissions.");
				return false;
			}

			Log.Debug($"Hub: {hub.nicknameSync.MyNick} ID: {hub.characterClassManager.UserId}");
			if (string.IsNullOrEmpty(permission))
			{
				Log.Error("Permission checked was null.");
				return false;
			}

			Log.Debug($"Permission string: {permission}");
			UserGroup userGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(hub.characterClassManager.UserId);
			Group group = null;
			if (userGroup != null)
			{
				Log.Debug($"UserGroup: {userGroup.BadgeText}");
				string groupName = ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == hub.serverRoles.Group).Key;
				Log.Debug($"GroupName: {groupName}");
				if (permissionsconfig == null)
				{
					Log.Error("Permissions config is null.");
					return false;
				}

				if (!permissionsconfig.groups.Any())
				{
					Log.Error("No permissionconfig groups.");
					return false;
				}

				if (!permissionsconfig.groups.TryGetValue(groupName, out group))
				{
					Log.Error("Could not get permission value.");
					return false;
				}
				Log.Debug($"Got group.");
			}
			else
			{
				Log.Debug("user group is null, getting default..");
				group = GetDefaultGroup();
			}

			if (group != null)
			{
				Log.Debug("Group is not null!");
				if (permission.Contains("."))
				{
					Log.Debug("Group contains perm seperator");
					if (group.permissions.Any(s => s == ".*"))
					{
						Log.Debug("All perms granted for all nodes.");
						return true;
					}
					if (group.permissions.Contains(permission.Split('.')[0] + ".*"))
					{
						Log.Debug("Check 1: True, returning.");
						return true;
					}
				}

				if (group.permissions.Contains(permission) || group.permissions.Contains("*"))
				{
					Log.Debug("Check 2: True, returning.");
					return true;
				}
			}
			else
			{
				Log.Debug("Group is null, returning false.");
				return false;
			}
			Log.Debug("No permissions found.");
			return false;
		}

		public class YML
		{
			public Dictionary<string, Group> groups { get; set; } = new Dictionary<string, Group>();
		}

		public class Group
		{
			[YamlMember(Alias = "default")]
			public bool Default { get; set; } = false;
			public List<string> inheritance { get; set; } = new List<string>();
			public List<string> permissions { get; set; } = new List<string>();
		}

		public override void OnDisable() { }
		public override void OnReload() { }
		public override string getName { get; }
	}
}
