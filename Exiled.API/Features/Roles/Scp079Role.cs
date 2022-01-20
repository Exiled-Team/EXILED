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
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp079Role"/> class.
        /// </summary>
        /// <param name="player">The encapsulated player.</param>
        internal Scp079Role(Player player) => Player = player;

        /// <inheritdoc/>
        public override Player Player { get; }

        /// <inheritdoc/>
        public override RoleType RoleType => RoleType.Scp079;

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// </summary>
        public Camera Camera
        {
            get => Camera.Get(Player.ReferenceHub.scp079PlayerScript.currentCamera);
            set => SetCamera(value);
        }

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Ability079[] Abilities
        {
            get => Player.ReferenceHub.scp079PlayerScript?.abilities;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript != null)
                    Player.ReferenceHub.scp079PlayerScript.abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's levels. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Level079[] Levels
        {
            get => Player.ReferenceHub.scp079PlayerScript?.levels;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript != null)
                    Player.ReferenceHub.scp079PlayerScript.levels = value;
            }
        }

        /// <summary>
        /// Gets or sets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public string Speaker
        {
            get => Player.ReferenceHub.scp079PlayerScript?.Speaker;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript != null)
                    Player.ReferenceHub.scp079PlayerScript.Speaker = value;
            }
        }

        /// <summary>
        /// Gets or sets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public SyncList<uint> LockedDoors
        {
            get => Player.ReferenceHub.scp079PlayerScript?.lockedDoors;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript != null)
                    Player.ReferenceHub.scp079PlayerScript.lockedDoors = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public float Experience
        {
            get => Player.ReferenceHub.scp079PlayerScript != null ? Player.ReferenceHub.scp079PlayerScript.Exp : float.NaN;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript == null)
                    return;

                Player.ReferenceHub.scp079PlayerScript.Exp = value;
                Player.ReferenceHub.scp079PlayerScript.OnExpChange();
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public byte Level
        {
            get => Player.ReferenceHub.scp079PlayerScript != null ? Player.ReferenceHub.scp079PlayerScript.Lvl : byte.MinValue;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript == null || Player.ReferenceHub.scp079PlayerScript.Lvl == value)
                    return;

                Player.ReferenceHub.scp079PlayerScript.ForceLevel(value, true);
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => Player.ReferenceHub.scp079PlayerScript != null ? Player.ReferenceHub.scp079PlayerScript.NetworkmaxMana : float.NaN;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript == null)
                    return;

                Player.ReferenceHub.scp079PlayerScript.NetworkmaxMana = value;
                Player.ReferenceHub.scp079PlayerScript.levels[Level].maxMana = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => Player.ReferenceHub.scp079PlayerScript != null ? Player.ReferenceHub.scp079PlayerScript.Mana : float.NaN;
            set
            {
                if (Player.ReferenceHub.scp079PlayerScript == null)
                    return;

                Player.ReferenceHub.scp079PlayerScript.Mana = value;
            }
        }

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraId">Camera ID.</param>
        public void SetCamera(ushort cameraId) => Player.ReferenceHub.scp079PlayerScript?.RpcSwitchCamera(cameraId, false);

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
        public void UnlockDoors() => Player.ReferenceHub.scp079PlayerScript.UnlockDoors();
    }
}
