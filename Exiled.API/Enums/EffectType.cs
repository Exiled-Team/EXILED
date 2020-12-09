  
// -----------------------------------------------------------------------
// <copyright file="EffectType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// StatusEffects from the game.
    /// </summary>
    public enum EffectType
    {
        /// <summary>
        /// infinite  sprint
        /// </summary>
        Invigorated,

        /// <summary>
        /// Invisiblity.
        /// </summary>
        Scp268,

        /// <summary>
        /// Not being able to open your inventory nor reload.
        /// </summary>
        Amnesia,
        
        /// <summary>
        /// Drain stamina then health.
        /// </summary>
        Asphyxiated,
        
        /// <summary>
        /// Damage over time.
        /// </summary>
        Bleeding,
        
        /// <summary>
        /// Blurry screen.
        /// </summary>
        Blinded,
        
        /// <summary>
        /// Increase damage the player gets.
        /// </summary>
        Burned,
        
        /// <summary>
        /// Screen gets blurrier the faster the player turns.
        /// </summary>
        Concussed,
        
        /// <summary>
        /// Teleports to PD and drains health.
        /// </summary>
        Corroding,
        
        /// <summary>
        /// Noises are harder to hear.
        /// </summary>
        Deafened,
        
        /// <summary>
        /// Remove 10% HP per second.
        /// </summary>
        Decontaminating,
        
        /// <summary>
        /// Slows all movement down.
        /// </summary>
        Disabled,
        
        /// <summary>
        /// Stops all movement.
        /// </summary>
        Ensnared,
        
        /// <summary>
        /// Halves the maximum stamina and the regeneration rate.
        /// </summary>
        Exhausted,
        
        /// <summary>
        /// Player will be unable to see.
        /// </summary>
        Flashed,
        
        /// <summary>
        /// Drain health while sprinting.
        /// </summary>
        Hemorrhage,
        
        /// <summary>
        /// Increaste stamina consumption.
        /// </summary>
        Panic,
        
        /// <summary>
        /// Damage over time. Which gets higher over time.
        /// </summary>
        Poisoned,
        
        /// <summary>
        /// Slows down movement.
        /// </summary>
        SinkHole,
        
        /// <summary>
        /// Makes the player faster but also drains health.
        /// </summary>
        Scp207,
        
        /// <summary>
        /// Gives the player the sound vision of SCP-939.
        /// </summary>
        Visuals939,
    }
}
