// -----------------------------------------------------------------------
// <copyright file="Recontainer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Enums;
    using Exiled.API.Features.Doors;
    using PlayerRoles.PlayableScps.Scp079;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="Scp079Recontainer"/>.
    /// </summary>
    public static class Recontainer
    {
        /// <summary>
        /// Gets the base <see cref="Scp079Recontainer"/>.
        /// </summary>
        public static Scp079Recontainer Base { get; internal set; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances used for the containment zone.
        /// </summary>
        public static IEnumerable<Door> ContainmentGates => Door.Get(door => Base._containmentGates.Contains(door.Base));

        /// <summary>
        /// Gets a value indicating whether the C.A.S.S.I.E is currently busy.
        /// </summary>
        public static bool IsCassieBusy => Base.CassieBusy;

        /// <summary>
        /// Gets or sets a value indicating whether the containment zone is open.
        /// </summary>
        public static bool IsContainmentZoneOpen
        {
            get => ContainmentGates.All(door => door.IsOpen);
            set => Base.SetContainmentDoors(value, IsContainmentZoneLocked);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment zone is locked.
        /// </summary>
        public static bool IsContainmentZoneLocked
        {
            get => ContainmentGates.All(door => door.IsLocked);
            set => Base.SetContainmentDoors(IsContainmentZoneOpen, value);
        }

        /// <summary>
        /// Gets or sets the delay to wait before overcharging.
        /// </summary>
        public static float OverchargeDelay
        {
            get => Base._activationDelay;
            set => Base._activationDelay = value;
        }

        /// <summary>
        /// Gets or sets the lockdown duration.
        /// </summary>
        public static float LockdownDuration
        {
            get => Base._lockdownDuration;
            set => Base._lockdownDuration = value;
        }

        /// <summary>
        /// Gets the activator button's <see cref="GameObject"/>.
        /// </summary>
        public static GameObject ActivatorButton => Base._activatorButton.gameObject;

        /// <summary>
        /// Gets or sets the <see cref="ActivatorButton"/>'s position.
        /// </summary>
        public static Vector3 ActivatorButtonPosition
        {
            get => ActivatorButton.transform.localPosition;
            set => ActivatorButton.transform.localPosition = value;
        }

        /// <summary>
        /// Gets the activator's window.
        /// </summary>
        public static Window ActivatorWindow => Window.Get(Base._activatorGlass);

        /// <summary>
        /// Gets the activator's position.
        /// </summary>
        public static Vector3 ActivatorPosition => Base._activatorPos;

        /// <summary>
        /// Gets or sets the activator's lerp speed.
        /// </summary>
        public static float ActivatorLerpSpeed
        {
            get => Base._activatorLerpSpeed;
            set => Base._activatorLerpSpeed = value;
        }

        /// <summary>
        /// Gets or sets the announcement played to warn players about the contaiment sequence's progress.
        /// </summary>
        public static string ProgressAnnouncement
        {
            get => Base._announcementProgress;
            set => Base._announcementProgress = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when counting down to overcharge.
        /// </summary>
        public static string CountdownAnnouncement
        {
            get => Base._announcementCountdown;
            set => Base._announcementCountdown = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when the contaiment is successful done.
        /// </summary>
        public static string ContainmentSuccessAnnouncement
        {
            get => Base._announcementSuccess;
            set => Base._announcementSuccess = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when the contaiment is failed.
        /// </summary>
        public static string ContainmentFailureAnnouncement
        {
            get => Base._announcementFailure;
            set => Base._announcementFailure = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when all the generators have been activated.
        /// </summary>
        public static string AllGeneratorsActivatedAnnouncement
        {
            get => Base._announcementAllActivated;
            set => Base._announcementAllActivated = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment sequence is done.
        /// </summary>
        public static bool IsContainmentSequenceDone
        {
            get => Base._alreadyRecontained;
            set => Base._alreadyRecontained = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment sequence is successful done.
        /// </summary>
        public static bool IsContainmentSequenceSuccessful
        {
            get => Base._success;
            set => Base._success = value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances locked during the overcharge procedure.
        /// </summary>
        public static IEnumerable<Door> LockedDoors => Base._lockedDoors.Select(Door.Get);

        /// <summary>
        /// Tries to kill SCP-079.
        /// </summary>
        /// <returns><see langword="true"/> if SCP-079 was successfully contained; otherwise, <see langword="false"/>.</returns>
        public static bool TryKillScp079() => Base.TryKill079();

        /// <summary>
        /// Playes an announcement.
        /// </summary>
        /// <param name="announcement">The announcement to play.</param>
        /// <param name="glitchyMultiplier">The glitchy multiplier.</param>
        public static void PlayAnnouncement(string announcement, float glitchyMultiplier) => Base.PlayAnnouncement(announcement, glitchyMultiplier);

        /// <summary>
        /// Begins the overcharge procedure.
        /// </summary>
        public static void BeginOvercharge() => Base.BeginOvercharge();

        /// <summary>
        /// Ends the overcharge procedure.
        /// </summary>
        public static void EndOvercharge() => Base.EndOvercharge();

        /// <summary>
        /// Announces the engagement status.
        /// </summary>
        public static void AnnounceEngagementStatus() => Base.UpdateStatus(Generator.Get(GeneratorState.Engaged).Count());

        /// <summary>
        /// Announces the engagement status.
        /// </summary>
        /// <param name="engagedGenerators">The engaged generators count.</param>
        public static void AnnounceEngagementStatus(int engagedGenerators) => Base.UpdateStatus(engagedGenerators);

        /// <summary>
        /// Refreshes the engagement status.
        /// </summary>
        public static void RefreshEngamentStatus() => Base.RefreshAmount();

        /// <summary>
        /// Begins the recontainment procedure.
        /// </summary>
        public static void Recontain() => Base.Recontain();

        /// <summary>
        /// Refreshes the activator.
        /// </summary>
        public static void RefreshActivator() => Base.RefreshActivator();

        /// <summary>
        /// Breaks the glass protecting the activator button.
        /// </summary>
        public static void BreakGlass() => ActivatorWindow.BreakWindow();
    }
}