// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using Exiled.API.Enums;

    using UnityEngine;

    /// <summary>
    /// A set of tools to easily work with the alpha warhead.
    /// </summary>
    public static class Warhead
    {
        private static AlphaWarheadNukesitePanel sitePanel;
        private static AlphaWarheadOutsitePanel outsitePanel;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadController"/> component.
        /// </summary>
        public static AlphaWarheadController Controller { get; internal set; }

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadNukesitePanel"/> component.
        /// </summary>
        public static AlphaWarheadNukesitePanel SitePanel => sitePanel ??= Object.FindObjectOfType<AlphaWarheadNukesitePanel>();

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadOutsitePanel"/> component.
        /// </summary>
        public static AlphaWarheadOutsitePanel OutsitePanel => outsitePanel ??= Object.FindObjectOfType<AlphaWarheadOutsitePanel>();

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the warhead lever.
        /// </summary>
        public static GameObject Lever => sitePanel.lever.gameObject;

        /// <summary>
        /// Gets or sets a value indicating whether the warhead lever is enabled or not.
        /// </summary>
        public static bool LeverStatus
        {
            get => SitePanel.Networkenabled;
            set => SitePanel.Networkenabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warhead has already been activated or not.
        /// </summary>
        public static bool IsKeycardActivated
        {
            get => OutsitePanel.NetworkkeycardEntered;
            set => OutsitePanel.NetworkkeycardEntered = value;
        }

        /// <summary>
        /// Gets or sets the warhead status.
        /// </summary>
        public static WarheadStatus Status
        {
            get => IsInProgress ? IsDetonated ? WarheadStatus.Detonated : WarheadStatus.InProgress : LeverStatus ? WarheadStatus.Armed : WarheadStatus.NotArmed;
            set
            {
                switch (value)
                {
                    case WarheadStatus.NotArmed:
                    case WarheadStatus.Armed:
                        Stop();
                        LeverStatus = value is WarheadStatus.Armed;
                        break;

                    case WarheadStatus.InProgress:
                        Start();
                        break;

                    case WarheadStatus.Detonated:
                        Detonate();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the warhead has already been detonated or not.
        /// </summary>
        public static bool IsDetonated => Controller.detonated;

        /// <summary>
        /// Gets a value indicating whether the warhead detonation is in progress or not.
        /// </summary>
        public static bool IsInProgress => Controller.NetworkinProgress;

        /// <summary>
        /// Gets or sets the warhead detonation timer.
        /// </summary>
        public static float DetonationTimer
        {
            get => Controller.NetworktimeToDetonation;
            set => Controller.NetworktimeToDetonation = value;
        }

        /// <summary>
        /// Gets the warhead real detonation timer.
        /// </summary>
        public static float RealDetonationTimer => Controller.RealDetonationTime();

        /// <summary>
        /// Gets or sets a value indicating whether the warhead can be disabled or not.
        /// </summary>
        public static bool IsLocked
        {
            get => Controller._isLocked;
            set => Controller._isLocked = value;
        }

        /// <summary>
        /// Gets a value indicating whether the warhead can be started or not.
        /// </summary>
        public static bool CanBeStarted => Controller.CanDetonate;

        /// <summary>
        /// Starts the warhead countdown.
        /// </summary>
        public static void Start()
        {
            Controller.InstantPrepare();
            Controller.StartDetonation(false);
        }

        /// <summary>
        /// Stops the warhead.
        /// </summary>
        public static void Stop() => Controller.CancelDetonation();

        /// <summary>
        /// Detonates the warhead.
        /// </summary>
        public static void Detonate()
        {
            Controller.InstantPrepare();
            Controller.Detonate();
        }

        /// <summary>
        /// Shake all players, like if the warhead has been detonated.
        /// </summary>
        public static void Shake()
        {
            foreach (Player player in Player.List)
                Controller.TargetRpcShake(player.Connection, false, false);
        }
    }
}
