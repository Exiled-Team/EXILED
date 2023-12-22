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

    /// <summary>
    /// Status effects as enum.
    /// </summary>
    /// <seealso cref="EffectTypeExtension.TryGetEffectType(CustomPlayerEffects.StatusEffectBase, out EffectType)"/>
    /// <seealso cref="EffectTypeExtension.TryGetType(EffectType, out Type)"/>
    public enum EffectType
    {
        /// <summary>
        /// This EffectType do not exist it's only use when not found or error.
        /// </summary>
        None = -1, // TODO: remove = -1

        /// <summary>
        /// The player isn't able to open their inventory or reload a weapon.
        /// </summary>
        AmnesiaItems,

        /// <summary>
        /// The player isn't able to see properly.
        /// </summary>
        AmnesiaVision,

        /// <summary>
        /// Drains the player's stamina and then health.
        /// </summary>
        Asphyxiated,

        /// <summary>
        /// Damages the player over time.
        /// </summary>
        Bleeding,

        /// <summary>
        /// Blurs the player's screen.
        /// </summary>
        Blinded,

        /// <summary>
        /// Increases damage the player receives. Does not apply any standalone damage.
        /// </summary>
        Burned,

        /// <summary>
        /// Blurs the player's screen while rotating.
        /// </summary>
        Concussed,

        /// <summary>
        /// Effect given to player after being hurt by SCP-106.
        /// </summary>
        Corroding,

        /// <summary>
        /// Deafens the player.
        /// </summary>
        Deafened,

        /// <summary>
        /// Removes 10% of the player's health per second.
        /// </summary>
        Decontaminating,

        /// <summary>
        /// Slows down the player's movement.
        /// </summary>
        Disabled,

        /// <summary>
        /// Prevents the player from moving.
        /// </summary>
        Ensnared,

        /// <summary>
        /// Halves the player's maximum stamina and stamina regeneration rate.
        /// </summary>
        Exhausted,

        /// <summary>
        /// Flashes the player.
        /// </summary>
        Flashed,

        /// <summary>
        /// Drains the player's health while sprinting.
        /// </summary>
        Hemorrhage,

        /// <summary>
        /// Reduces the player's FOV, gives infinite stamina and gives the effect of underwater sound.
        /// </summary>
        Invigorated,

        /// <summary>
        /// Reduces damage taken by body shots.
        /// </summary>
        BodyshotReduction,

        /// <summary>
        /// Damages the player every 5 seconds, starting low and increasing over time.
        /// </summary>
        Poisoned,

        /// <summary>
        /// Increases the speed of the player while also draining health.
        /// </summary>
        Scp207,

        /// <summary>
        /// Makes the player invisible.
        /// </summary>
        Invisible,

        /// <summary>
        /// Slows down the player's movement with the SCP-106 sinkhole effect.
        /// </summary>
        SinkHole,

        /// <summary>
        /// Reduces overall damage taken.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// Increases movement speed.
        /// </summary>
        MovementBoost,

        /// <summary>
        /// Reduces the severity of negative effects.
        /// </summary>
        RainbowTaste,

        /// <summary>
        /// Drops the player's current item, disables interaction with objects, and deals damage while effect is active.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Prevents the player from sprinting and reduces movement speed by 20%.
        /// </summary>
        Stained,

        /// <summary>
        /// Causes the player to become gain immunity to certain negative status effects.
        /// </summary>
        Vitality,

        /// <summary>
        /// Cause the player to slowly take damage, reduces bullet accuracy, and increases item pickup time.
        /// </summary>
        Hypothermia,

        /// <summary>
        /// Increases the player's motor function, causing the player to reduce the weapon draw time, reload spead, item pickup speed, and medical item usage.
        /// </summary>
        Scp1853,

        /// <summary>
        /// Effect given to player after being hurt by SCP-049.
        /// </summary>
        CardiacArrest,

        /// <summary>
        /// Cause the lighting in the facility to dim heavily for the player.
        /// </summary>
        InsufficientLighting,

        /// <summary>
        /// Disable ambient sound.
        /// </summary>
        SoundtrackMute,

        /// <summary>
        /// Protects players from enemy damage if the config is enabled.
        /// </summary>
        SpawnProtected,

        /// <summary>
        /// Make Scp106 able to see you when he is in the ground (stalking), causes the player's screens to become monochromatic when seeing Scp106, and instantly killed if attacked by Scp106.
        /// </summary>
        Traumatized,

        /// <summary>
        /// It slows down the player, providing a passive health regeneration and saving the player from death once.
        /// </summary>
        AntiScp207,

        /// <summary>
        /// The effect that SCP-079 gives the scanned player with the Breach Scanner.
        /// </summary>
        Scanned,

        /// <summary>
        /// Teleports the player to the pocket dimension and drains health until the player escapes or is killed. The amount of damage recieved increases the longer the effect is applied.
        /// </summary>
        PocketCorroding,

        /// <summary>
        /// Reduces walking sound by 10%.
        /// </summary>
        SilentWalk,

        /// <summary>
        /// Makes you a marshmallow guy.
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
        /// Makes you a flamingo.
        /// </summary>
        BecomingFlamingo,

        /// <summary>
        /// Cake.
        /// </summary>
        Scp559,

        /// <summary>
        /// Scp956 found you.
        /// </summary>
        Scp956Target,
    }
}
