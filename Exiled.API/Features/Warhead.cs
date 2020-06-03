// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    /// <summary>
    /// A set of tools to work with the warhead code more easily.
    /// </summary>
    public class Warhead
    {
        private static AlphaWarheadController controller;
        private static AlphaWarheadNukesitePanel sitePanel;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadController"/> component.
        /// </summary>
        public static AlphaWarheadController Controller
        {
            get
            {
                if (controller == null)
                    controller = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();

                return controller;
            }
        }

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadNukesitePanel"/> component.
        /// </summary>
        public static AlphaWarheadNukesitePanel SitePanel
        {
            get
            {
                if (sitePanel == null)
                    sitePanel = UnityEngine.Object.FindObjectOfType<AlphaWarheadNukesitePanel>();

                return sitePanel;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the warhead lever is enabled or not.
        /// </summary>
        public static bool IsLeverEnabled
        {
            get => SitePanel.Networkenabled;
            set => SitePanel.Networkenabled = value;
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
        /// Starts the warhead.
        /// </summary>
        public static void Start()
        {
            Controller.InstantPrepare();
            Controller.StartDetonation();
        }

        /// <summary>
        /// Stops the warhead.
        /// </summary>
        public static void Stop() => Controller.CancelDetonation();

        /// <summary>
        /// Detonates the warhead.
        /// </summary>
        public static void Detonate() => Controller.Detonate();
    }
}
