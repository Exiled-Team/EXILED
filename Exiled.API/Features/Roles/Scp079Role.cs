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
        internal Scp079Role(Player player) => Owner = player;

        /// <inheritdoc/>
        public override Player Owner { get; }

        /// <summary>
        /// Gets the actual script of the Scp.
        /// </summary>
        public Scp079PlayerScript Scp079 => script ?? (script = Owner.ReferenceHub.scp079PlayerScript);

        /// <summary>
        /// Gets or sets the camera SCP-079 is currently controlling.
        /// </summary>
        public Camera Camera
        {
            get => Camera.Get(Scp079.currentCamera);
            set => SetCamera(value);
        }

        /// <summary>
        /// Gets or sets SCP-079's abilities. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Ability079[] Abilities
        {
            get => Scp079?.abilities;
            set
            {
                if (Scp079 != null)
                    Scp079.abilities = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's levels. Can be <see langword="null"/>.
        /// </summary>
        public Scp079PlayerScript.Level079[] Levels
        {
            get => Owner.ReferenceHub.scp079PlayerScript?.levels;
            set
            {
                if (Scp079 != null)
                    Scp079.levels = value;
            }
        }

        /// <summary>
        /// Gets or sets the speaker SCP-079 is currently using. Can be <see langword="null"/>.
        /// </summary>
        public string Speaker
        {
            get => Scp079?.Speaker;
            set
            {
                if (Scp079 != null)
                    Scp079.Speaker = value;
            }
        }

        /// <summary>
        /// Gets or sets the doors SCP-079 has locked. Can be <see langword="null"/>.
        /// </summary>
        public SyncList<uint> LockedDoors
        {
            get => Scp079?.lockedDoors;
            set
            {
                if (Scp079 != null)
                    Scp079.lockedDoors = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of experience SCP-079 has.
        /// </summary>
        public float Experience
        {
            get => Scp079 != null ? Scp079.Exp : float.NaN;
            set
            {
                if (Scp079 == null)
                    return;

                Scp079.Exp = value;
                Scp079.OnExpChange();
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's level.
        /// </summary>
        public byte Level
        {
            get => Scp079 != null ? Scp079.Lvl : byte.MinValue;
            set
            {
                if (Scp079 == null || Scp079.Lvl == value)
                    return;

                Scp079.ForceLevel(value, true);
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's max energy.
        /// </summary>
        public float MaxEnergy
        {
            get => Scp079 != null ? Scp079.NetworkmaxMana : float.NaN;
            set
            {
                if (Scp079 == null)
                    return;

                Scp079.NetworkmaxMana = value;
                Scp079.levels[Level].maxMana = value;
            }
        }

        /// <summary>
        /// Gets or sets SCP-079's energy.
        /// </summary>
        public float Energy
        {
            get => Scp079 != null ? Scp079.Mana : float.NaN;
            set
            {
                if (Scp079 == null)
                    return;

                Scp079.Mana = value;
            }
        }

        /// <inheritdoc/>
        internal override RoleType RoleType => RoleType.Scp079;

        /// <summary>
        /// Sets the camera SCP-079 is currently located at.
        /// </summary>
        /// <param name="cameraId">Camera ID.</param>
        public void SetCamera(ushort cameraId) => Scp079?.RpcSwitchCamera(cameraId, false);

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
        public void UnlockDoors() => Scp079.UnlockDoors();
    }
}
