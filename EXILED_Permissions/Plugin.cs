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
                Directory.CreateDirectory(pluginDir);
            if (!File.Exists(Path.Combine(pluginDir, "permissions.yml")))
                File.WriteAllText(Path.Combine(pluginDir, "permissions.yml"), Encoding.UTF8.GetString(Resources.permissions));
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
            foreach(KeyValuePair<string, Group> gr in permissionsconfig.groups)
            {
                if (gr.Value.Default)
                    return gr.Value;
            }
            return null;
        }

        public static bool CheckPermission(ReferenceHub hub, string permission)
        {
            UserGroup userGroup = ServerStatic.GetPermissionsHandler().GetUserGroup(hub.characterClassManager.UserId);
            Group group = null;
            if (userGroup != null)
            {
                string groupName = ServerStatic.GetPermissionsHandler()._groups.FirstOrDefault(g => g.Value == hub.serverRoles.Group).Key;
                permissionsconfig.groups.TryGetValue(groupName, out group);
            }
            else
            {
                group = GetDefaultGroup();
            }
            if (group != null)
            {
                if (permission.Contains("."))
                {
                    if (group.permissions.Contains(permission.Split('.')[0] + ".*"))
                        return true;
                }
                if (group.permissions.Contains(permission) || group.permissions.Contains("*"))
                    return true;
            }
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
