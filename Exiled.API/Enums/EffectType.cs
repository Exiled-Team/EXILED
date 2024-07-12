// -----------------------------------------------------------------------
// <copyright file="EffectType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Items;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Status effects as enum.
    /// </summary>
    /// <seealso cref="EffectTypeExtension.TryGetEffectType(CustomPlayerEffects.StatusEffectBase, out EffectType)"/>
    /// <seealso cref="EffectTypeExtension.TryGetType(EffectType, out Type)"/>
    public enum EffectType
    {
        /// <summary>
        /// This enum member is used when there is no effect, similar to <see langword="null"/>.
        /// </summary>
        None,

        /// <summary>
        /// Prevents the player from opening their inventory, reloading weapons, and using certain items.
        /// Caused by <see cref="PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudAbility"/>.
        /// </summary>
        AmnesiaItems,

        /// <summary>
        /// Prevents the player from seeing SCP-939.
        /// Caused by <see cref="PlayerRoles.PlayableScps.Scp939.Scp939AmnesticCloudAbility"/>
        /// </summary>
        AmnesiaVision,

        /// <summary>
        /// Every 0.5s drains 5% of the player's stamina, or deals 2 damage if stamina is drained.
        /// </summary>
        Asphyxiated,

        /// <summary>
        /// Drains health every 5 seconds, starting at 20 and halving each time down to a minimum of 2.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Blurs player's screen.
        /// </summary>
        Blinded,

        /// <summary>
        /// Increases damage the player receives by 20%. Does not cause any damage by itself.
        /// </summary>
        Burned,

        /// <summary>
        /// Blurs player's screen when rotating.
        /// </summary>
        Concussed,

        /// <summary>
        /// Slowly drains health and stamina. Allows SCP-106 to teleport the player to pocket dimension.
        /// Caused by <see cref="PlayerRoles.PlayableScps.Scp106.Scp106Attack"/>.
        /// </summary>
        Corroding,

        /// <summary>
        /// Muffles player's hearing.
        /// </summary>
        Deafened,

        /// <summary>
        /// Drains 10% of player's health every second.
        /// </summary>
        Decontaminating,

        /// <summary>
        /// Reduces movement speed by 20%.
        /// </summary>
        Disabled,

        /// <summary>
        /// Completely prevents the player from moving.
        /// </summary>
        Ensnared,

        /// <summary>
        /// Halves stamina maximum value and regeneration rate.
        /// </summary>
        Exhausted,

        /// <summary>
        /// Flashes the player, making them completely unable to see.
        /// <seealso cref="FlashGrenade"/>
        /// </summary>
        Flashed,

        /// <summary>
        /// Drains health while sprinting at the rate of 2/s.
        /// </summary>
        Hemorrhage,

        /// <summary>
        /// Reduces FOV, gives infinite stamina and muffled hearing effect.
        /// </summary>
        Invigorated,

        /// <summary>
        /// Reduces damage dealt by body shots.
        /// </summary>
        BodyshotReduction,

        /// <summary>
        /// Drains health every 5 seconds, starting at 2 and doubling each time up to a maximum of 20.
        /// </summary>
        Poisoned,

        /// <summary>
        /// Increases the movement speed at the cost of draining health based on how fast the player is moving.
        /// </summary>
        Scp207,

        /// <summary>
        /// Makes the player invisible.
        /// Caused by <see cref="Scp268"/>
        /// </summary>
        Invisible,

        /// <summary>
        /// Reduces movement speed with the SCP-106 sinkhole effect.
        /// </summary>
        SinkHole,

        /// <summary>
        /// Reduces damage taken by 0.5% for each intensity level.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// Increases movement speed by 1% for each intensity level.
        /// </summary>
        MovementBoost,

        /// <summary>
        /// Reduces the severity of negative effects.
        /// </summary>
        RainbowTaste,

        /// <summary>
        /// Drops current item, disables interaction with objects, and deals damage while effect is active.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Prevents from sprinting and reduces movement speed by 20%.
        /// </summary>
        Stained,

        /// <summary>
        /// Causes the player to become gain immunity to certain negative status effects.
        /// </summary>
        Vitality,

        /// <summary>
        /// Slowly drains health, reduces bullet accuracy, and increases item pickup time.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Reduce the weapon draw time, increases the speed of reloading, item pickup, and consumable use.
        /// </summary>
        Scp1853,

        /// <summary>
        /// Drain the player's health at a rate of 8 per second, and allows SCP-049 to one-shot the player.
        /// Caused SCP-049.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Cause the lighting in the facility to dim heavily for the player.
        /// </summary>
        InsufficientLighting,

        /// <summary>
        /// Disables ambient sound.
        /// </summary>
        SoundtrackMute,

        /// <summary>
        /// Protects from enemy damage if spawn protection is enabled in configs.
        /// </summary>
        SpawnProtected,

        /// <summary>
        /// Makes SCP-106 able to see the player when he is in the ground (stalking), causes player's screen to become monochromatic when seeing SCP-106, and instantly die if attacked by Scp106.
        /// </summary>
        Traumatized,

        /// <summary>
        /// Reduces movement speed, regenerates health, and prevents the player from dying once.
        /// </summary>
        AntiScp207,

        /// <summary>
        /// Makes SCP-079 able to see the player on the map.
        /// Caused by SCP-079's Breach Scanner ability.
        /// </summary>
        Scanned,

        /// <summary>
        /// Teleports to the pocket dimension and drains player's health until they escape or die. The damage increases over time.
        /// </summary>
        PocketCorroding,

        /// <summary>
        /// Reduces walking sound by 10% for each intensity level.
        /// </summary>
        SilentWalk,

        /// <summary>
        /// Turns the player into a marshmallow guy.
        /// </summary>
        [Obsolete("Not functional in-game")]
        Marshmallow,

        /// <summary>
        /// The effect that is given to the player when getting attacked by SCP-3114's Strangle ability.
        /// </summary>
        Strangled,

        /// <summary>
        /// Makes the player nearly invisible, and allows them to pass through doors.
        /// </summary>
        Ghostly,

        /// <summary>
        /// Modifies which type of fog the player sees.
        /// <remarks>You can choose the <see cref="CustomRendering.FogType"/> and put it in intensity.</remarks>
        /// </summary>
        FogControl,

        /// <summary>
        /// Reduces movement speed by 1% for each intensity level.
        /// Can make the player move backwards if the intensity is over 100.
        /// </summary>
        Slowness,
    }
}
