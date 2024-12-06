// -----------------------------------------------------------------------
// <copyright file="Window.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DamageHandlers;
    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Doors;
    using Exiled.API.Interfaces;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="BreakableWindow"/>.
    /// </summary>
    public class Window : GameEntity, IWrapper<BreakableWindow>
    {
        /// <summary>
        /// A <see cref="Dictionary{TKey,TValue}"/> containing all known <see cref="BreakableWindow"/>s and their corresponding <see cref="Window"/>.
        /// </summary>
        internal static readonly Dictionary<BreakableWindow, Window> BreakableWindowToWindow = new(20, new ComponentsEqualityComparer());

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The base <see cref="BreakableWindow"/> for this door.</param>
        /// <param name="room">The <see cref="Room"/> for this window.</param>
        internal Window(BreakableWindow window, Room room)
            : base(window.gameObject)
        {
            BreakableWindowToWindow.Add(window, this);
            Base = window;
            Room = room;
            Type = GetGlassType();
#if Debug
            if (Type is GlassType.Unknown)
                Log.Error($"[GLASSTYPE UNKNOWN] {this}");
#endif
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
        /// </summary>
        public static new IReadOnlyCollection<Window> List => BreakableWindowToWindow.Values;

        /// <summary>
        /// Gets a randomly selected <see cref="Window"/>.
        /// </summary>
        /// <returns>A randomly selected <see cref="Window"/> object.</returns>
        public static Window Random => List.Random();

        /// <summary>
        /// Gets the base-game <see cref="BreakableWindow"/> for this window.
        /// </summary>
        public BreakableWindow Base { get; }

        /// <summary>
        /// Gets the window's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public override Transform Transform => Base._transform;

        /// <summary>
        /// Gets the <see cref="Features.Room"/> the window is in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        /// Gets the window's <see cref="GlassType"/>.
        /// </summary>
        public GlassType Type { get; }

        /// <summary>
        /// Gets the window's <see cref="ZoneType"/>.
        /// </summary>
        public ZoneType Zone => Room.Zone;

        /// <summary>
        /// Gets a value indicating whether or not this window is breakable.
        /// </summary>
        public bool IsBreakable => !Base.NetworkisBroken;

        /// <summary>
        /// Gets or sets a value indicating whether or not this window is broken.
        /// </summary>
        public bool IsBroken
        {
            get => Base.NetworkisBroken;
            set => Base.NetworkisBroken = value;
        }

        /// <summary>
        /// Gets or sets if the window's remaining health. No effect if the window cannot be broken.
        /// </summary>
        public float Health
        {
            get => Base.health;
            set => Base.health = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this window can be broken by SCP.
        /// </summary>
        public bool DisableScpDamage
        {
            get => Base._preventScpDamage;
            set => Base._preventScpDamage = value;
        }

        /// <summary>
        /// Gets or sets a value indicating who is the LastAttacker.
        /// </summary>
        public Player LastAttacker
        {
            get => Player.Get(Base.LastAttacker.Hub);
            set => Base.LastAttacker = value.Footprint;
        }

        /// <summary>
        /// Gets the window object associated with a specific <see cref="Window"/>, or creates a new one if there isn't one.
        /// </summary>
        /// <param name="breakableWindow">The base-game <see cref="Window"/>.</param>
        /// <returns>A <see cref="Door"/> wrapper object.</returns>
        public static Window Get(BreakableWindow breakableWindow) => BreakableWindowToWindow.TryGetValue(breakableWindow, out Window window)
            ? window
            : new(breakableWindow, breakableWindow.GetComponentInParent<Room>());

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Window"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Window"/> which contains elements that satisfy the condition.</returns>
        public static IEnumerable<Window> Get(Func<Window, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Try-get a <see cref="Window"/> belonging to the <see cref="BreakableWindow"/>, if any.
        /// </summary>
        /// <param name="breakableWindow">The <see cref="BreakableWindow"/> instance.</param>
        /// <param name="window">A <see cref="Window"/> or <see langword="null"/> if not found.</param>
        /// <returns>Whether or not a window was found.</returns>
        public static bool TryGet(BreakableWindow breakableWindow, out Window window)
        {
            window = Get(breakableWindow);
            return window is not null;
        }

        /// <summary>
        /// Try-get a <see cref="IEnumerable{T}"/> of <see cref="Window"/> filtered based on a predicate.
        /// </summary>
        /// <param name="predicate">The condition to satisfy.</param>
        /// <param name="windows">A <see cref="IEnumerable{T}"/> of <see cref="Window"/> which contains elements that satisfy the condition.</param>
        /// <returns>Whether or not at least one window was found.</returns>
        public static bool TryGet(Func<Window, bool> predicate, out IEnumerable<Window> windows)
        {
            windows = Get(predicate);
            return windows.Any();
        }

        /// <summary>
        /// Break the window.
        /// </summary>
        public void Break() => Base.BreakWindow();

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        public void Damage(float amount) => Base.ServerDamageWindow(amount);

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="handler">The handler of damage.</param>
        /// <returns>Whether or not the Window get Damage.</returns>
        public bool Damage(float amount, DamageHandlerBase handler) => Base.Damage(amount, handler, Vector3.zero);

        /// <summary>
        /// Returns the Window in a human-readable format.
        /// </summary>
        /// <returns>A string containing Window-related data.</returns>
        public override string ToString() => $"{Type} ({Health}) [{IsBroken}] *{DisableScpDamage}*";

        private GlassType GetGlassType() => !Room ? GlassType.Unknown : Room.Type switch
        {
            RoomType.Lcz330 => GlassType.Scp330,
            RoomType.LczGlassBox => GlassType.GR18,
            RoomType.LczPlants => GlassType.Plants,
            RoomType.Hcz049 => GlassType.Scp049,
            RoomType.Hcz079 => Recontainer.Base._activatorGlass == Base ? GlassType.Scp079Trigger : GlassType.Scp079,
            RoomType.HczHid => GlassType.MicroHid,
            RoomType.HczTestRoom => GlassType.TestRoom,
            RoomType.HczEzCheckpointA => GlassType.HczEzCheckpointA,
            RoomType.HczEzCheckpointB => GlassType.HczEzCheckpointB,
            _ => GlassType.Unknown,
        };
    }
}