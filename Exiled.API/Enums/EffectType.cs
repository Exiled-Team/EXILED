// -----------------------------------------------------------------------
// <copyright file="EffectType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Status effects as enum.
    /// </summary>
    public enum EffectType
    {
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
        /// Increases damage the player gets.
        /// </summary>
        Burned,

        /// <summary>
        /// Blurs the player's screen when rotating.
        /// </summary>
        Concussed,

        /// <summary>
        /// Teleports the player to the pocket dimension and drains health.
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
        /// Stops the player's movement.
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
        /// Damages the player every 5 seconds, starting low and ramping hight.
        /// </summary>
        Poisoned,

        /// <summary>
        /// Makes the player faster but also drains health.
        /// </summary>
        Scp207,

        /// <summary>
        /// Makes the player invisibility.
        /// </summary>
        Invisible,

        /// <summary>
        /// Slows down the player's movement with SCP-106 effect.
        /// </summary>
        SinkHole,

        /// <summary>
        /// Gives the player the sound vision of SCP-939.
        /// </summary>
        Visuals939,

        /// <summary>
        /// Reduces all damage taken.
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
        /// Stops the player from sprinting and reduces movement speed by 20%.
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
