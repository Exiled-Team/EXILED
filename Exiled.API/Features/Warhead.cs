// -----------------------------------------------------------------------
// <copyright file="Warhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;

    using Enums;
    using Interactables.Interobjects.DoorUtils;
    using Mirror;

    using UnityEngine;

    /// <summary>
    /// A set of tools to easily work with the alpha warhead.
    /// </summary>
    public static class Warhead
    {
        private static AlphaWarheadOutsitePanel alphaWarheadOutsitePanel;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadController"/> component.
        /// </summary>
        public static AlphaWarheadController Controller => AlphaWarheadController.Singleton;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadNukesitePanel"/> component.
        /// </summary>
        public static AlphaWarheadNukesitePanel SitePanel => AlphaWarheadOutsitePanel.nukeside;

        /// <summary>
        /// Gets the cached <see cref="AlphaWarheadOutsitePanel"/> component.
        /// </summary>
        public static AlphaWarheadOutsitePanel OutsitePanel => alphaWarheadOutsitePanel != null ? alphaWarheadOutsitePanel : (alphaWarheadOutsitePanel = Object.FindObjectOfType<AlphaWarheadOutsitePanel>());

        /// <summary>
        /// Gets the <see cref="GameObject"/> of the warhead lever.
        /// </summary>
        public static GameObject Lever => SitePanel.lever.gameObject;

        /// <summary>
        /// Gets or sets a value indicating whether or not automatic detonation is enabled.
        /// </summary>
        public static bool AutoDetonate
        {
            get => Controller._autoDetonate;
            set => Controller._autoDetonate = value;
        }

        /// <summary>
        /// Gets or sets the auto detonation time.
        /// </summary>
        public static float AutoDetonateTime
        {
            get => Controller._autoDetonateTime;
            set => Controller._autoDetonateTime = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not doors will be opened when the warhead activates.
        /// </summary>
        public static bool OpenDoors
        {
            get => Controller._openDoors;
            set => Controller._openDoors = value;
        }

        /// <summary>
        /// Gets all of the warhead blast doors.
        /// </summary>
        public static IReadOnlyCollection<BlastDoor> BlastDoors => BlastDoor.Instances;

        /// <summary>
        /// Gets or sets a value indicating whether or not the warhead lever is enabled.
        /// </summary>
        public static bool LeverStatus
        {
            get => SitePanel.Networkenabled;
            set => SitePanel.Networkenabled = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the warhead's outside panel has been opened.
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
        /// Gets a value indicating whether or not the warhead has already been detonated.
        /// </summary>
        public static bool IsDetonated => Controller._alreadyDetonated;

        /// <summary>
        /// Gets a value indicating whether or not the warhead detonation is in progress.
        /// </summary>
        public static bool IsInProgress => Controller.Info.InProgress;

        /// <summary>
        /// Gets or sets the warhead detonation timer.
        /// </summary>
        public static float DetonationTimer
        {
            get => AlphaWarheadController.TimeUntilDetonation;
            set => Controller.ForceTime(value);
        }

        /// <summary>
        /// Gets the warhead real detonation timer.
        /// </summary>
        public static float RealDetonationTimer => Controller.CurScenario.TimeToDetonate;

        /// <summary>
        /// Gets or sets a value indicating whether the warhead should be disabled.
        /// </summary>
        public static bool IsLocked
        {
            get => Controller.IsLocked;
            set => Controller.IsLocked = value;
        }

        /// <summary>
        /// Gets or sets the amount of kills caused by the warhead (shown on the summary screen).
        /// </summary>
        public static int Kills
        {
            get => Controller.WarheadKills;
            set => Controller.WarheadKills = value;
        }

        /// <summary>
        /// Gets a value indicating whether or not the warhead can be started.
        /// </summary>
        public static bool CanBeStarted => !IsInProgress && !IsDetonated && Controller.CooldownEndTime <= NetworkTime.time;

        /// <summary>
        /// Closes the surface blast doors.
        /// </summary>
        public static void CloseBlastDoors()
        {
            foreach (BlastDoor door in BlastDoors)
                door.SetClosed(false, true);
        }

        /// <summary>
        /// Opens or closes all doors on the map, based on the provided <paramref name="open"/>.
        /// </summary>
        /// <param name="open">Whether to open or close all doors on the map.</param>
        public static void TriggerDoors(bool open) => DoorEventOpenerExtension.TriggerAction(open ? DoorEventOpenerExtension.OpenerEventType.WarheadStart : DoorEventOpenerExtension.OpenerEventType.WarheadCancel);

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
        public static void Detonate() => Controller.ForceTime(0f);

        /// <summary>
        /// Shake all players, like if the warhead has been detonated.
        /// </summary>
        public static void Shake() => Controller.RpcShake(false);

        /// <summary>
        /// Gets whether or not the provided position will be detonated by the alpha warhead.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <param name="includeOnlyLifts">If <see langword="true"/>, only lifts will be checked.</param>
        /// <returns>Whether or not the given position is prone to being detonated.</returns>
        public static bool CanBeDetonated(Vector3 pos, bool includeOnlyLifts = false) => AlphaWarheadController.CanBeDetonated(pos, includeOnlyLifts);
    }
}
