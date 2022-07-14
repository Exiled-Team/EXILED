// -----------------------------------------------------------------------
// <copyright file="Window.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.DamageHandlers;

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="BreakableWindow"/>.
    /// </summary>
    public class Window
    {
        /// <summary>
        /// A <see cref="List{T}"/> of <see cref="Window"/> on the map.
        /// </summary>
        internal static readonly List<Window> WindowValue = new(30);
        private static readonly Dictionary<BreakableWindow, Window> BreakableWindowToWindow = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="window">The base <see cref="BreakableWindow"/> for this door.</param>
        public Window(BreakableWindow window)
        {
            BreakableWindowToWindow.Add(window, this);
            Base = window;
            Room = window.GetComponentInParent<Room>();
            Type = GetGlassType();
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances.
        /// </summary>
        public static IEnumerable<Window> List => WindowValue.AsReadOnly();

        /// <summary>
        /// Gets the base-game <see cref="BreakableWindow"/> for this window.
        /// </summary>
        public BreakableWindow Base { get; }

        /// <summary>
        /// Gets the <see cref="UnityEngine.GameObject"/> of the window.
        /// </summary>
        public GameObject GameObject => Base.gameObject;

        /// <summary>
        /// Gets the window's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base._transform;

        /// <summary>
        /// Gets the <see cref="Exiled.API.Features.Room"/> the window is in.
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
        /// Gets or sets the window's position.
        /// </summary>
        public Vector3 Position
        {
            get => GameObject.transform.position;
            set => GameObject.transform.position = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not this window represents the window in front of SCP-079's recontainment button.
        /// </summary>
        public bool Is079Trigger => Recontainer.ActivatorWindow == this;

        /// <summary>
        /// Gets a value indicating whether or not this window is breakable.
        /// </summary>
        public bool IsBreakable => !Base.isBroken;

        /// <summary>
        /// Gets or sets a value indicating whether or not this window is broken.
        /// </summary>
        public bool IsBroken
        {
            get => Base.isBroken;
            set => Base.isBroken = value;
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
        /// Gets or sets the window's rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get => GameObject.transform.rotation;
            set => GameObject.transform.rotation = value;
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
        /// Gets or sets a value indicating whether or not this window is broken.
        /// </summary>
        public BreakableWindow.BreakableWindowStatus SyncStatus
        {
            get => Base.NetworksyncStatus;
            set => Base.NetworksyncStatus = value;
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
        public static Window Get(BreakableWindow breakableWindow) => BreakableWindowToWindow.ContainsKey(breakableWindow)
            ? BreakableWindowToWindow[breakableWindow]
            : new Window(breakableWindow);

        /// <summary>
        /// Break the window.
        /// </summary>
        public void BreakWindow() => Base.BreakWindow();

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        public void DamageWindow(float amount) => Base.ServerDamageWindow(amount);

        /// <summary>
        /// Damages the window.
        /// </summary>
        /// <param name="amount">The amount of damage to deal.</param>
        /// <param name="handler">The handler of damage.</param>
        public void DamageWindow(float amount, DamageHandlerBase handler)
        {
            Base.Damage(amount, handler, Vector3.zero);
        }

        /// <summary>
        /// Returns the Window in a human-readable format.
        /// </summary>
        /// <returns>A string containing Window-related data.</returns>
        public override string ToString() => $"{Type} {Health} {IsBroken} {DisableScpDamage}";

        private GlassType GetGlassType()
        {
            if (Recontainer.ActivatorWindow.Base == Base)
                return GlassType.Scp079Trigger;
            return Room.gameObject.name.RemoveBracketsOnEndOfName() switch
            {
                "LCZ_330" => GlassType.Scp330,
                "LCZ_372" => GlassType.GR18,
                "LCZ_Plants" => GlassType.Plants,
                "HCZ_049" => GlassType.Scp049,
                "HCZ_079" => GlassType.Scp079,
                "HCZ_Hid" => GlassType.MicroHid,
                _ => GlassType.Unknown,
            };
        }
    }
}
