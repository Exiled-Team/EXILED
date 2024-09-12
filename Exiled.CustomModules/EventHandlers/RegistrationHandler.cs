// -----------------------------------------------------------------------
// <copyright file="RegistrationHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.EventHandlers
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.CustomItems;

    /// <summary>
    /// Handles the all the module's registration.
    /// </summary>
    internal class RegistrationHandler
    {
        private readonly Config config;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationHandler"/> class.
        /// </summary>
        /// <param name="inConfig">The configuration instance.</param>
        internal RegistrationHandler(Config inConfig) => config = inConfig;

        /// <summary>
        /// Fired when a module gets enabled.
        /// </summary>
        /// <param name="moduleInfo">The module which is being enabled.</param>
        internal void OnModuleEnabled(ModuleInfo moduleInfo)
        {
            if (moduleInfo.Type.IsGenericType && moduleInfo.Type.BaseType != typeof(CustomModule))
                return;

            Log.InfoWithContext($"Module '{moduleInfo.Name}' has been deployed.", Log.CONTEXT_DEPLOYMENT);

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomRoles.Name && config.UseDefaultRoleAssigner)
            {
                StaticActor.Get<RoleAssigner>();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomTeams.Name && config.UseDefaultRespawnManager)
            {
                StaticActor.Get<RespawnManager>();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomGameModes.Name)
            {
                World.Get();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomAbilities.Name)
            {
                TrackerBase.Get();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomItems.Name)
            {
                GlobalPatchProcessor.PatchAll("exiled.customitems.patch", nameof(CustomItem));
                TrackerBase.Get();
            }
        }

        /// <summary>
        /// Fired when a module gets disabled.
        /// </summary>
        /// <param name="moduleInfo">The module which is being disabled.</param>
        internal void OnModuleDisabled(ModuleInfo moduleInfo)
        {
            Log.InfoWithContext($"Module '{moduleInfo.Name}' has been disabled.", Log.CONTEXT_DEPLOYMENT);

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomRoles.Name && config.UseDefaultRoleAssigner)
            {
                StaticActor.Get<RoleAssigner>()?.Destroy();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomTeams.Name && config.UseDefaultRespawnManager)
            {
                StaticActor.Get<RespawnManager>()?.Destroy();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomGameModes.Name)
            {
                World.Get()?.Destroy();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomAbilities.Name)
            {
                TrackerBase.Get()?.Destroy();
                return;
            }

            if (moduleInfo.ModuleType.Name == UUModuleType.CustomItems.Name)
            {
                GlobalPatchProcessor.UnpatchAll("exiled.customitems.unpatch", nameof(CustomItem));
                TrackerBase.Get()?.Destroy();
                return;
            }
        }
    }
}