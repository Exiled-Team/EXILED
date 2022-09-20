// -----------------------------------------------------------------------
// <copyright file="Scp079Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Mirror;

    /// <summary>
    /// Defines a role that represents SCP-079.
    /// </summary>
    public class Scp079Role : Role
    {
        private Scp079PlayerScript script;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp079Role(Player player)
        {
            Owner = player;
        }

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="Scp079PlayerScript"/> script for the role.
        /// </summary>
        public Scp079PlayerScript Script
        {
            get => script ??= Owner.ReferenceHub.scp079PlayerScript;
        }

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// </summary>
        public Camera Camera
        {
            get => Camera.Get(Owner.ReferenceHub.scp079PlayerScript.currentCamera);
            set => SetCamera(value);
        }

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Ability079[] Abilities
        {
            get => Script?.abilities;
            set
            {
                if (Script is not null)
                    Script.abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's levels. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Level079[] Levels
        {
            get => Script?.levels;
            set
            {
                if (Script is not null)
                    Script.levels = value;
            }
        }

        /// <summary>
        /// Gets or sets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public string Speaker
        {
            get => Script?.Speaker;
            set
            {
                if (Script is not null)
                    Script.Speaker = value;
            }
        }

        /// <summary>
        /// Gets or sets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public SyncList<uint> LockedDoors
        {
            get => Script?.lockedDoors;
            set
            {
                if (Script is not null)
                    Script.lockedDoors = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public float Experience
        {
            get => Script?.Exp ?? float.NaN;
            set
            {
                if (Script is null)
                    return;

                Script.Exp = value;
                Script.OnExpChange();
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public byte Level
        {
            get => Script?.Lvl ?? byte.MinValue;
            set
            {
                if (Script is null || Script.Lvl == value)
                    return;

                Script.ForceLevel(value, true);
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => Script?.NetworkmaxMana ?? float.NaN;
            set
            {
                if (Script is null)
                    return;

                Script.NetworkmaxMana = value;
                Script.levels[Level].maxMana = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => Script?.Mana ?? float.NaN;
            set
            {
                if (Script is null)
                    return;

                Script.Mana = value;
            }
        }

        /// <inheritdoc/>
        internal override RoleType RoleType
        {
            get => RoleType.Scp079;
        }

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraId">Camera ID.</param>
        public void SetCamera(ushort cameraId) => Script?.RpcSwitchCamera(cameraId, false);

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraType">The <see cref="Enums.CameraType"/>.</param>
        public void SetCamera(Enums.CameraType cameraType) => SetCamera(Camera.Get(cameraType));

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="camera">The <see cref="Camera"/> object to switch to.</param>
        public void SetCamera(Camera camera) => SetCamera(camera.Id);

        /// <summary>
        /// Unlocks all doors that SCP-079 has locked.
        /// </summary>
        public void UnlockDoors() => Owner.ReferenceHub.scp079PlayerScript.UnlockDoors();
    }
}