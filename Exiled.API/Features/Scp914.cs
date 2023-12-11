// -----------------------------------------------------------------------
// <copyright file="Scp914.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;
    using global::Scp914;
    using UnityEngine;

    /// <summary>
    /// A set of tools to modify SCP-914's behaviour.
    /// </summary>
    public static class Scp914
    {
        /// <summary>
        /// Gets the cached <see cref="global::Scp914.Scp914Controller"/>.
        /// </summary>
        public static Scp914Controller Scp914Controller => Scp914Controller.Singleton;

        /// <summary>
        /// Gets or sets SCP-914's <see cref="Scp914KnobSetting"/>.
        /// </summary>
        public static Scp914KnobSetting KnobStatus
        {
            get => Scp914Controller.Network_knobSetting;
            set => Scp914Controller.Network_knobSetting = value;
        }

        /// <summary>
        /// Gets or sets SCP-914's config mode.
        /// </summary>
        public static Scp914Mode ConfigMode
        {
            get => Scp914Controller._configMode.Value;
            set => Scp914Controller._configMode.Value = value;
        }

        /// <summary>
        /// Gets SCP-914's <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public static GameObject GameObject => Scp914Controller.gameObject;

        /// <summary>
        /// Gets SCP-914's <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform Transform => Scp914Controller.transform;

        /// <summary>
        /// Gets the position of SCP-914's intake chamber.
        /// </summary>
        public static Vector3 IntakePosition => Scp914Controller.IntakeChamber.localPosition;

        /// <summary>
        /// Gets the position of SCP-914's output chamber.
        /// </summary>
        public static Vector3 OutputPosition => Scp914Controller.OutputChamber.localPosition;

        /// <summary>
        /// Gets the position offset in which item is moving.
        /// </summary>
        public static Vector3 MovingVector => OutputPosition - IntakePosition;

        /// <summary>
        /// Gets a value indicating whether SCP-914 is active and currently processing items.
        /// </summary>
        public static bool IsWorking => Scp914Controller._isUpgrading;

        /// <summary>
        /// Gets a value indicating all of the GameObjects currently present inside SCP-914's intake chamber.
        /// </summary>
        public static Collider[] InsideIntake => Physics.OverlapBox(IntakePosition, Scp914Controller.IntakeChamberSize);

        /// <summary>
        /// Gets the intake booth <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform IntakeBooth => Scp914Controller.IntakeChamber;

        /// <summary>
        ///  Gets the output booth <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public static Transform OutputBooth => Scp914Controller.OutputChamber;

        /// <summary>
        /// Gets the list with <see cref="Door"/> which SCP-914 has.
        /// </summary>
        public static IReadOnlyCollection<Door> Doors => Scp914Controller._doors.Select(Door.Get).ToList();

        /// <summary>
        /// Filters all GameObjects inside SCP-914's intake chamber into players and items.
        /// </summary>
        /// <param name="playersret">The <see cref="List{Player}"/> to return.</param>
        /// <param name="pickupsret">The <see cref="List{Pickup}"/> to return.</param>
        /// <returns>All GameObjects present inside SCP-914's intake chamber. And also return Player and Pickup casted.</returns>
        public static IEnumerable<GameObject> Scp914InputObject(out IEnumerable<Player> playersret, out IEnumerable<Pickup> pickupsret)
        {
            HashSet<GameObject> inside914 = HashSetPool<GameObject>.Pool.Get();
            List<Player> players = ListPool<Player>.Pool.Get();
            List<Pickup> pickups = ListPool<Pickup>.Pool.Get();

            foreach (Collider collider in InsideIntake.ToList())
            {
                GameObject gameObject = collider.transform.root.gameObject;
                if (inside914.Add(gameObject))
                {
                    Pickup pickup;
                    if ((pickup = Pickup.Get(gameObject)) is not null && !pickup.IsLocked)
                    {
                        pickups.Add(pickup);
                    }
                    else if (Player.TryGet(gameObject, out Player player)
                        && Physics.Linecast(player.Position, IntakePosition, Scp914Upgrader.SolidObjectMask))
                    {
                        players.Add(player);
                    }
                }
            }

            playersret = ListPool<Player>.Pool.ToArrayReturn(players);
            pickupsret = ListPool<Pickup>.Pool.ToArrayReturn(pickups);
            return HashSetPool<GameObject>.Pool.ToArrayReturn(inside914);
        }

        /// <summary>
        /// Plays the SCP-914's sound.
        /// </summary>
        /// <param name="soundId">The <see cref="Scp914InteractCode"/> to play.</param>
        public static void PlaySound(Scp914InteractCode soundId) => Scp914Controller.RpcPlaySound((byte)soundId);

        /// <summary>
        /// Starts SCP-914.
        /// </summary>
        /// <param name="player"><see cref="Player"/> who interacts with Scp914.</param>
        /// <param name="code"><see cref="Scp914InteractCode"/> Interact code.</param>
        public static void Start(Player player = null, Scp914InteractCode code = Scp914InteractCode.Activate) => Scp914Controller.ServerInteract((player ?? Server.Host).ReferenceHub, (byte)code);
    }
}
