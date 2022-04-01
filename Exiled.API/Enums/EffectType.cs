// -----------------------------------------------------------------------
// <copyright file="EffectType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums {
    /// <summary>
    /// Status effects as enum.
    /// </summary>
    public enum EffectType {
        /// <summary>
        /// The player isn't able to open their inventory or reload a weapon.
        /// </summary>
        Amnesia,

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
        /// Teleports the player to the pocket dimension and drains health until the player escapes or is killed.
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
        /// Gives the player the SCP-939 sound vision.
        /// </summary>
        Visuals939,

        /// <summary>
        /// Reduces overall damage taken.
        /// </summary>
        DamageReduction,

        /// <summary>
        /// Increases movement speed.
        /// </summary>
        MovementBoost,

        /// <summary>
        /// Severely reduces damage taken.
        /// </summary>
        RainbowTaste,

        /// <summary>
        /// Drops the player's current item and deals damage while effect is active.
        /// </summary>
        SeveredHands,

        /// <summary>
        /// Prevents the player from sprinting and reduces movement speed by 20%.
        /// </summary>
        Stained,

        /// <summary>
        /// Causes the player to blink.
        /// </summary>
        Visual173Blink,

        /// <summary>
        /// Causes the player to slowly regenerate health.
        /// </summary>
        Vitality,
    }
}
