// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether SCP-173 can be blocked or not by the tutorial.
        /// </summary>
        public static bool CanTutorialBlockScp173 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SCP-096 can be triggered or not by the tutorial.
        /// </summary>
        public static bool CanTutorialTriggerScp096 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the warhead can be disabled or not.
        /// </summary>
        public static bool IsWarheadLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the name tracking is enabled or not.
        /// </summary>
        public static bool IsNameTrackingEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the inventory should be dropped before being set as spectator, through commands or plugins.
        /// </summary>
        public static bool ShouldDropInventory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the blood can be spawned or not.
        /// </summary>
        public static bool CanSpawnBlood { get; set; }

        /// <summary>
        /// Gets or sets the anti-fly treshold.
        /// </summary>
        public static int AntiFlyThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the anti fly is enabled or not.
        /// </summary>
        public static bool IsAntyFlyEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the teams have to be respawned at random or not.
        /// </summary>
        public static bool IsRespawnRandom { get; set; }

        /// <summary>
        /// Gets a value indicating whether bans should be logged or not.
        /// </summary>
        public static bool ShouldLogBans { get; private set; }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public string Prefix => "exiled_events_";

        /// <inheritdoc/>
        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled", true);
            CanTutorialBlockScp173 = PluginManager.YamlConfig.GetBool($"{Prefix}tutorial_block_scp173", true);
            CanTutorialTriggerScp096 = PluginManager.YamlConfig.GetBool($"{Prefix}tutorial_trigger_scp096", true);
            IsWarheadLocked = PluginManager.YamlConfig.GetBool($"{Prefix}warhead_locked");
            IsNameTrackingEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}name_tracking_enabled", true);
            ShouldDropInventory = PluginManager.YamlConfig.GetBool($"{Prefix}drop_inventory", true);
            CanSpawnBlood = PluginManager.YamlConfig.GetBool($"{Prefix}spawn_blood", true);
            AntiFlyThreshold = PluginManager.YamlConfig.GetInt($"{Prefix}antifly_threshold", 5);
            IsAntyFlyEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}antifly_enabled", true);
            IsRespawnRandom = PluginManager.YamlConfig.GetBool($"{Prefix}random_respawns");
            ShouldLogBans = PluginManager.YamlConfig.GetBool($"{Prefix}log_bans", true);
        }
    }
}
