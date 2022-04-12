// -----------------------------------------------------------------------
// <copyright file="CustomRoleBlueprint.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;

    using UnityEngine;

    /// <summary>
    /// The base <see cref="CustomRoleBlueprint"/> serializable class.
    /// </summary>
    public abstract class CustomRoleBlueprint
    {
        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="CustomRoleBlueprint"/> containing all the registered custom blueprints.
        /// </summary>
        public static HashSet<CustomRoleBlueprint> Registered { get; } = new();

        /// <summary>
        /// Gets or sets the component of the blueprint.
        /// </summary>
        public abstract Type Component { get; set; }

        /// <summary>
        /// Gets or sets the id of the blueprint.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> to spawn this blueprint as.
        /// </summary>
        public abstract RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the name of this blueprint.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this blueprint.
        /// </summary>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets the CustomInfo of this blueprint.
        /// </summary>
        public abstract string CustomInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the custom blueprint is shared.
        /// </summary>
        public virtual bool IsShared { get; set; }

        /// <summary>
        /// Gets or sets the player's max health.
        /// </summary>
        public virtual int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the player's health on spawning.
        /// </summary>
        public virtual float StartingHealth { get; set; }

        /// <summary>
        /// Gets or sets the player's max artificial health.
        /// </summary>
        public virtual float MaxArtificialHealth { get; set; }

        /// <summary>
        /// Gets or sets the player's artificial health on spawning.
        /// </summary>
        public virtual float StartingArtificialHealth { get; set; }

        /// <summary>
        /// Gets or sets a list of the blueprints custom abilities.
        /// </summary>
        public virtual List<CustomAbility> CustomAbilities { get; set; } = new();

        /// <summary>
        /// Gets or sets the starting inventory for the blueprint.
        /// </summary>
        public virtual Dictionary<string, ushort> Inventory { get; set; } = new();

        /// <summary>
        /// Gets or sets the possible spawn locations for this blueprint.
        /// </summary>
        public virtual SpawnProperties SpawnProperties { get; set; } = new();

        /// <summary>
        /// Gets or sets a value indicating whether players keep their current inventory when gaining this blueprint.
        /// </summary>
        public virtual bool KeepInventoryOnSpawn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether players die when this blueprint is removed.
        /// </summary>
        public virtual bool KillOwnerOnDestroy { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating the <see cref="Player"/>'s size.
        /// </summary>
        public virtual Vector3 Scale { get; set; } = Vector3.one;

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of the fake appearance applied when the player is spawned.
        /// </summary>
        public virtual RoleType FakeAppearance { get; set; } = RoleType.None;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="FakeAppearance"/> should be kept after the player changes blueprint.
        /// </summary>
        public virtual bool KeepFakeAppearanceOnChangingRole { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player's <see cref="PlayerInfoArea"/> should be hidden.
        /// </summary>
        public virtual bool HideInfoArea { get; set; } = true;

        /// <summary>
        /// Gets or sets the <see cref="Broadcast"/> to be displayed to the player on spawning.
        /// </summary>
        public virtual Broadcast DisplayBroadcast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can escape.
        /// </summary>
        public virtual bool CanEscape { get; set; } = true;

        /// <summary>
        /// Gets or sets the overridden escape blueprint.
        /// </summary>
        public virtual RoleType OverrideDefaultEscapeRole { get; set; } = RoleType.None;

        /// <summary>
        /// Gets or sets a value indicating whether the player should affect Scp173's movement.
        /// </summary>
        public virtual bool IgnoreScp173 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player should ignore Scp096 vision information.
        /// </summary>
        public virtual bool IgnoreScp096 { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all the doors that can be bypassed.
        /// </summary>
        public virtual DoorType[] BypassableDoors { get; set; } = new DoorType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the blueprints which can be used along with the custom blueprint.
        /// </summary>
        /// <remarks>
        /// The player who's changing blueprint will keep the custom blueprint if the new blueprint is listed.
        /// <br>Leave empty if you don't want to allow any blueprint other than the default one specified in the config</br>.
        /// </remarks>
        public virtual RoleType[] AllowedRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the blueprints that can damage the player.
        /// </summary>
        /// Using these values along with <see cref="DamageableByRoles"/> is not allowed.
        /// <br>The computational priority is lower than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual RoleType[] DamageableByRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the blueprints that can't damage the player.
        /// </summary>
        /// Using these values along with <see cref="NotDamageableByRoles"/> is not allowed.
        /// <br>The computational priority is lower than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual RoleType[] NotDamageableByRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the teams that can damage the player.
        /// </summary>
        /// Using these values along with <see cref="NotDamageableByTeams"/> is not allowed.
        /// <br>The computational priority is higher than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual Team[] DamageableByTeams { get; set; } = new Team[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the teams that can't damage the player.
        /// </summary>
        /// <remarks>
        /// Using these values along with <see cref="DamageableByTeams"/> is not allowed.
        /// <br>The computational priority is higher than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        /// </remarks>
        public virtual Team[] NotDamageableByTeams { get; set; } = new Team[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the blueprints the player can damage.
        /// </summary>
        /// Using these values along with <see cref="DamageableByRoles"/> is not allowed.
        /// <br>The computational priority is lower than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual RoleType[] DamageableRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="RoleType"/>[] containing all the blueprints the player can't damage.
        /// </summary>
        /// Using these values along with <see cref="NotDamageableByRoles"/> is not allowed.
        /// <br>The computational priority is lower than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual RoleType[] NotDamageableRoles { get; set; } = new RoleType[] { };

        /// <summary>
        /// Gets or sets a <see cref="Team"/>[] containing all the teams the player can damage.
        /// </summary>
        /// Using these values along with <see cref="NotDamageableByTeams"/> is not allowed.
        /// <br>The computational priority is higher than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        public virtual Team[] DamageableTeams { get; set; } = new Team[] { };

        /// <summary>
        /// Gets or sets a <see cref="Team"/>[] containing all the teams the player can't damage.
        /// </summary>
        /// <remarks>
        /// Using these values along with <see cref="DamageableByTeams"/> is not allowed.
        /// <br>The computational priority is higher than <see cref="NotDamageableByRoles"/> and <see cref="DamageableByRoles"/>,
        /// hence the execution will ignore all the other values.</br>
        /// </remarks>
        public virtual Team[] NotDamageableTeams { get; set; } = new Team[] { };

        /// <summary>
        /// Gets or sets a <see cref="EffectType"/>[] containing all the effects to be applied when the player is spawned.
        /// </summary>
        public virtual EffectType[] GivenEffects { get; set; } = new EffectType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        /// <remarks>It's not possible using these listed values along with <see cref="AllowedDamageTypes"/>.</remarks>
        public virtual DamageType[] IgnoredDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        /// <remarks>It's not possible using these listed values along with <see cref="IgnoredDamageTypes"/>.</remarks>
        public virtual DamageType[] AllowedDamageTypes { get; set; } = new DamageType[] { };

        /// <summary>
        /// Gets a <see cref="CustomRoleBlueprint"/> by id.
        /// </summary>
        /// <param name="id">The id of the blueprint to get.</param>
        /// <returns>The corresponding <see cref="CustomRoleBlueprint"/>, or <see langword="null"/> if not found.</returns>
        public static CustomRoleBlueprint Get(int id) => Registered?.FirstOrDefault(r => r.Id == id);

        /// <summary>
        /// Gets a <see cref="CustomRoleBlueprint"/> by type.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to get.</param>
        /// <returns>The corresponding <see cref="CustomRoleBlueprint"/>, or <see langword="null"/> if not found.</returns>
        public static CustomRoleBlueprint Get(Type t) => Registered.FirstOrDefault(r => r.Component == t);

        /// <summary>
        /// Gets a <see cref="CustomRoleBlueprint"/> by name.
        /// </summary>
        /// <param name="name">The name of the blueprint to get.</param>
        /// <returns>The corresponding <see cref="CustomRoleBlueprint"/>, or <see langword="null"/> if not found.</returns>
        public static CustomRoleBlueprint Get(string name) => Registered?.FirstOrDefault(r => r.Name == name);

        /// <summary>
        /// Tries to get a <see cref="CustomRoleBlueprint"/> by name.
        /// </summary>
        /// <param name="name">The name of the blueprint to get.</param>
        /// <param name="blueprint">The custom blueprint.</param>
        /// <returns><see langword="true"/> if the blueprint exists; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(string name, out CustomRoleBlueprint blueprint)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            blueprint = int.TryParse(name, out int id) ? Get(id) : Get(name);

            return blueprint is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRoleBlueprint"/> by name.
        /// </summary>
        /// <param name="id">The id of the blueprint to get.</param>
        /// <param name="blueprint">The custom blueprint.</param>
        /// <returns><see langword="true"/> if the blueprint exists; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If the name is <see langword="null"/> or an empty string.</exception>
        public static bool TryGet(int id, out CustomRoleBlueprint blueprint)
        {
            blueprint = Get(id);

            return blueprint is not null;
        }

        /// <summary>
        /// Tries to get a <see cref="CustomRoleBlueprint"/> by name.
        /// </summary>
        /// <param name="type">The type of the blueprint to get.</param>
        /// <param name="blueprint">The custom blueprint.</param>
        /// <returns><see langword="true"/> if the blueprint exists; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">If the type is <see langword="null"/>.</exception>
        public static bool TryGet(Type type, out CustomRoleBlueprint blueprint)
        {
            blueprint = type is null ? null : Get(type);

            return blueprint is not null;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRoleBlueprint"/>'s present in the current assembly.
        /// </summary>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/> which contains all registered <see cref="CustomRoleBlueprint"/>'s.</returns>
        public static IEnumerable<CustomRoleBlueprint> RegisterRoles(bool skipReflection = false, object overrideClass = null)
        {
            List<CustomRoleBlueprint> blueprints = new();
            Log.Warn("Registering blueprints..");
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if ((type.BaseType != typeof(CustomRoleBlueprint) && !type.IsSubclassOf(typeof(CustomRoleBlueprint))) || type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null)
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true))
                {
                    CustomRoleBlueprint blueprint = null;

                    if (!skipReflection && Loader.Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.Loader.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ?? plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            blueprint = property.GetValue(overrideClass ?? plugin.Config) as CustomRoleBlueprint;
                            break;
                        }
                    }

                    if (blueprint is null)
                        blueprint = (CustomRoleBlueprint)Activator.CreateInstance(type);

                    if (blueprint.Role is RoleType.None)
                        blueprint.Role = ((CustomRoleAttribute)attribute).RoleType;

                    if (blueprint.TryRegister())
                        blueprints.Add(blueprint);
                }
            }

            return blueprints;
        }

        /// <summary>
        /// Registers all the <see cref="CustomRoleBlueprint"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <param name="skipReflection">Whether or not reflection is skipped (more efficient if you are not using your custom item classes as config objects).</param>
        /// <param name="overrideClass">The class to search properties for, if different from the plugin's config class.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/> which contains all registered <see cref="CustomRoleBlueprint"/>'s.</returns>
        public static IEnumerable<CustomRoleBlueprint> RegisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false, bool skipReflection = false, object overrideClass = null)
        {
            List<CustomRoleBlueprint> blueprints = new();
            Assembly assembly = Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if ((type.BaseType != typeof(CustomRoleBlueprint) && !type.IsSubclassOf(typeof(CustomRoleBlueprint))) || type.GetCustomAttribute(typeof(CustomRoleAttribute)) is null ||
                    (isIgnored && targetTypes.Contains(type)) || (!isIgnored && !targetTypes.Contains(type)))
                    continue;

                foreach (Attribute attribute in type.GetCustomAttributes(typeof(CustomRoleAttribute), true))
                {
                    CustomRoleBlueprint blueprint = null;

                    if (!skipReflection && Loader.Loader.PluginAssemblies.ContainsKey(assembly))
                    {
                        IPlugin<IConfig> plugin = Loader.Loader.PluginAssemblies[assembly];

                        foreach (PropertyInfo property in overrideClass?.GetType().GetProperties() ??
                                                          plugin.Config.GetType().GetProperties())
                        {
                            if (property.PropertyType != type)
                                continue;

                            blueprint = property.GetValue(overrideClass ?? plugin.Config) as CustomRoleBlueprint;
                        }
                    }

                    if (blueprint is null)
                        blueprint = (CustomRoleBlueprint)Activator.CreateInstance(type);

                    if (blueprint.Role is RoleType.None)
                        blueprint.Role = ((CustomRoleAttribute)attribute).RoleType;

                    if (blueprint.TryRegister())
                        blueprints.Add(blueprint);
                }
            }

            return blueprints;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRoleBlueprint"/>'s present in the current assembly.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/> which contains all unregistered <see cref="CustomRoleBlueprint"/>'s.</returns>
        public static IEnumerable<CustomRoleBlueprint> UnregisterRoles()
        {
            List<CustomRoleBlueprint> unregisteredRoles = new();

            foreach (CustomRoleBlueprint blueprint in Registered)
            {
                blueprint.TryUnregister();
                unregisteredRoles.Add(blueprint);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Unregisters all the <see cref="CustomRoleBlueprint"/>'s present in the current assembly.
        /// </summary>
        /// <param name="targetTypes">The <see cref="IEnumerable{T}"/> of <see cref="Type"/> containing the target types.</param>
        /// <param name="isIgnored">A value indicating whether the target types should be ignored.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="CustomRoleBlueprint"/> which contains all unregistered <see cref="CustomRoleBlueprint"/>'s.</returns>
        public static IEnumerable<CustomRoleBlueprint> UnregisterRoles(IEnumerable<Type> targetTypes, bool isIgnored = false)
        {
            List<CustomRoleBlueprint> unregisteredRoles = new();

            foreach (CustomRoleBlueprint blueprint in Registered)
            {
                if ((targetTypes.Contains(blueprint.GetType()) && isIgnored) || (!targetTypes.Contains(blueprint.GetType()) && !isIgnored))
                    continue;

                blueprint.TryUnregister();
                unregisteredRoles.Add(blueprint);
            }

            return unregisteredRoles;
        }

        /// <summary>
        /// Tries to register this blueprint.
        /// </summary>
        /// <returns>True if the blueprint registered properly.</returns>
        internal bool TryRegister()
        {
            if (!Registered.Contains(this))
            {
                if (Registered.Any(r => r.Id == Id))
                {
                    Log.Warn($"{Name} has tried to register with the same id as another blueprint: {Registered.First(r => r.Id == Id).Name} ({Id}). It won't be registered!");

                    return false;
                }

                Registered.Add(this);

                Log.Debug($"{Name} ({Id}) has been successfully registered.", CustomRoles.Instance.Config.Debug);

                return true;
            }

            Log.Warn($"Couldn't register {Name} ({Id}) [{Role}] as it already exists.");

            return false;
        }

        /// <summary>
        /// Tries to unregister this blueprint.
        /// </summary>
        /// <returns>True if the blueprint is unregistered properly.</returns>
        internal bool TryUnregister()
        {
            if (!Registered.Remove(this))
            {
                Log.Warn($"Cannot unregister {Name} ({Id}) [{Role}], it hasn't been registered yet.");

                return false;
            }

            return true;
        }
    }

    /// <inheritdoc/>
    /// <typeparam name="T">The <see cref="EActor"/> component type to be assigned.</typeparam>
#pragma warning disable SA1402 // File may only contain a single type
    public abstract class CustomRoleBlueprint<T> : CustomRoleBlueprint
#pragma warning restore SA1402 // File may only contain a single type
        where T : EActor
    {
        /// <inheritdoc/>
        public override Type Component { get; set; } = typeof(T);
    }
}
