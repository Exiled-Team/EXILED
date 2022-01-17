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

    using UnityEngine;

    /// <summary>
    /// A wrapper class for <see cref="Recontainer079"/>.
    /// </summary>
    public class Recontainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Recontainer"/> class.
        /// </summary>
        /// <param name="recontainer079"><inheritdoc cref="Base"/></param>
        internal Recontainer(Recontainer079 recontainer079) => Base = recontainer079;

        /// <summary>
        /// Gets the base <see cref="Recontainer079"/>.
        /// </summary>
        public Recontainer079 Base { get; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances used for the containment zone.
        /// </summary>
        public IEnumerable<Door> ContainmentGates => Map.Doors.Where(door => Base._containmentGates.Contains(door.Base));

        /// <summary>
        /// Gets a value indicating whether the C.A.S.S.I.E is currently busy.
        /// </summary>
        public bool IsCassieBusy => Base.CassieBusy;

        /// <summary>
        /// Gets or sets a value indicating whether the containment zone is open.
        /// </summary>
        public bool IsContaimentZoneOpen
        {
            get => ContainmentGates.All(door => door.IsOpen);
            set => Base.SetContainmentDoors(value, IsContainmentZoneLocked);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment zone is locked.
        /// </summary>
        public bool IsContainmentZoneLocked
        {
            get => ContainmentGates.All(door => door.IsLocked);
            set => Base.SetContainmentDoors(IsContaimentZoneOpen, value);
        }

        /// <summary>
        /// Gets or sets the delay to wait before overcharging.
        /// </summary>
        public float OverchargeDelay
        {
            get => Base._activationDelay;
            set => Base._activationDelay = value;
        }

        /// <summary>
        /// Gets or sets the lockdown duration.
        /// </summary>
        public float LockdownDuration
        {
            get => Base._lockdownDuration;
            set => Base._lockdownDuration = value;
        }

        /// <summary>
        /// Gets the activator button's <see cref="Transform"/>.
        /// </summary>
        public Transform ActivatorButton => Base._activatorButton;

        /// <summary>
        /// Gets or sets the <see cref="ActivatorButton"/>'s position.
        /// </summary>
        public Vector3 ActivatorButtonPosition
        {
            get => ActivatorButton.localPosition;
            set => ActivatorButton.localPosition = value;
        }

        /// <summary>
        /// Gets the activator's window.
        /// </summary>
        public BreakableWindow ActivatorWindow => Base._activatorGlass;

        /// <summary>
        /// Gets the activator's position.
        /// </summary>
        public Vector3 ActivatorPosition => Base._activatorPos;

        /// <summary>
        /// Gets or sets the activator's lerp speed.
        /// </summary>
        public float ActivatorLerpSpeed
        {
            get => Base._activatorLerpSpeed;
            set => Base._activatorLerpSpeed = value;
        }

        /// <summary>
        /// Gets or sets the announcement played to warn players about the contaiment sequence's progress.
        /// </summary>
        public string ProgressAnnouncement
        {
            get => Base._announcementProgress;
            set => Base._announcementProgress = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when counting down to overcharge.
        /// </summary>
        public string CountdownAnnouncement
        {
            get => Base._announcementCountdown;
            set => Base._announcementCountdown = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when the contaiment is successful done.
        /// </summary>
        public string ContainmentSuccessAnnouncement
        {
            get => Base._announcementSuccess;
            set => Base._announcementSuccess = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when the contaiment is failed.
        /// </summary>
        public string ContainmentFailureAnnouncement
        {
            get => Base._announcementFailure;
            set => Base._announcementFailure = value;
        }

        /// <summary>
        /// Gets or sets the announcement played when all the generators have been activated.
        /// </summary>
        public string AllGeneratorsActivatedAnnouncement
        {
            get => Base._announcementAllActivated;
            set => Base._announcementAllActivated = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment sequence is done.
        /// </summary>
        public bool IsContainmentSequenceDone
        {
            get => Base._alreadyRecontained;
            set => Base._alreadyRecontained = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the containment sequence is successful done.
        /// </summary>
        public bool IsContainmentSequenceSuccessful
        {
            get => Base._success;
            set => Base._success = value;
        }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="Door"/> which contains all the <see cref="Door"/> instances locked during the overcharge procedure.
        /// </summary>
        public IEnumerable<Door> LockedDoors => Map.Doors.Where(door => Base._lockedDoors.Contains(door.Base));

        /// <summary>
        /// Gets or sets a <see cref="List{T}"/> of <see cref="Recontainer"/> which contains all the <see cref="Recontainer"/> instances.
        /// </summary>
        internal static List<Recontainer> Instances { get; set; } = new List<Recontainer>();

        /// <summary>
        /// Gets a <see cref="Recontainer"/> belonging to the <see cref="Recontainer079"/>, if any.
        /// </summary>
        /// <param name="recontainer079">The base <see cref="Recontainer079"/>.</param>
        /// <returns>A <see cref="Recontainer"/> or <see langword="null"/> if not found.</returns>
        public static Recontainer Get(Recontainer079 recontainer079) => Instances.FirstOrDefault(recontainer => recontainer.Base == recontainer079);

        /// <summary>
        /// Tries to kill SCP-079.
        /// </summary>
        /// <returns><see langword="true"/> if SCP-079 was successfully contained; otherwise, <see langword="false"/>.</returns>
        public bool TryKillScp079() => Base.TryKill079();

        /// <summary>
        /// Playes an announcement.
        /// </summary>
        /// <param name="announcement">The announcement to play.</param>
        /// <param name="glitchyMultiplier">The glitchy multiplier.</param>
        public void PlayAnnouncement(string announcement, float glitchyMultiplier) => Base.PlayAnnouncement(announcement, glitchyMultiplier);

        /// <summary>
        /// Begins the overcharge procedure.
        /// </summary>
        public void BeginOvercharge() => Base.BeginOvercharge();

        /// <summary>
        /// Ends the overcharge procedure.
        /// </summary>
        public void EndOvercharge() => Base.EndOvercharge();

        /// <summary>
        /// Announces the engagement status.
        /// </summary>
        public void AnnounceEngagementStatus() => Base.UpdateStatus(Generator.Get(GeneratorState.Engaged));

        /// <summary>
        /// Announces the engagement status.
        /// </summary>
        /// <param name="engagedGenerators">The engaged generators count.</param>
        public void AnnounceEngagementStatus(int engagedGenerators) => Base.UpdateStatus(engagedGenerators);

        /// <summary>
        /// Refreshes the engagement status.
        /// </summary>
        public void RefreshEngamentStatus() => Base.RefreshAmount();

        /// <summary>
        /// Begins the recontainment procedure.
        /// </summary>
        public void Recontain() => Base.Recontain();

        /// <summary>
        /// Refreshes the activator.
        /// </summary>
        public void RefreshActivator() => Base.RefreshActivator();
    }
}
