// -----------------------------------------------------------------------
// <copyright file="MovementCore.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Components
{
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Handles the movements of an NPC.
    /// </summary>
    public class MovementCore
    {
        private const float SneakSpeed = 1.8f;
        private readonly Npc npc;
        private readonly CoroutineHandle coroutineHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MovementCore"/> class.
        /// </summary>
        /// <param name="npc">The NPC to control the movement of.</param>
        public MovementCore(Npc npc)
        {
            this.npc = npc;
            Movement = PlayerMovementState.Sprinting;
            coroutineHandle = Timing.RunCoroutine(MovementCoroutine(), Segment.FixedUpdate);
        }

        /// <summary>
        /// Gets or sets a game object that the NPC should follow.
        /// </summary>
        public GameObject FollowTarget { get; set; }

        /// <summary>
        /// Gets or sets the movement type of the NPC.
        /// </summary>
        public PlayerMovementState Movement
        {
            get => npc.Player.ReferenceHub.animationController.MoveState;
            set => npc.Player.ReferenceHub.animationController.MoveState = value;
        }

        /// <summary>
        /// Gets or sets the direction that the NPC is moving in.
        /// </summary>
        public MovementDirection Direction { get; set; } = MovementDirection.Forward;

        /// <summary>
        /// Gets or sets a value indicating whether the movement controller is paused.
        /// </summary>
        public bool IsPaused { get; set; } = true;

        private float WalkSpeed => CharacterClassManager._staticClasses[(int)npc.Role].walkSpeed;

        private float RunSpeed => CharacterClassManager._staticClasses[(int)npc.Role].runSpeed;

        /// <summary>
        /// Kills movement control.
        /// </summary>
        public void Kill() => Timing.KillCoroutines(coroutineHandle);

        private void Follow()
        {
            Vector3 moveDirection = FollowTarget.transform.position - npc.Player.Position;

            Quaternion rot = Quaternion.LookRotation(moveDirection.normalized);
            npc.Player.Rotation = new Vector2(rot.eulerAngles.x, rot.eulerAngles.y);

            if (moveDirection.magnitude < 3)
                return;

            if (moveDirection.magnitude > 10)
            {
                npc.Position = FollowTarget.transform.position;
                return;
            }

            Move();
        }

        private void Move()
        {
            float speed = Movement switch
            {
                PlayerMovementState.Sneaking => SneakSpeed,
                PlayerMovementState.Sprinting => RunSpeed,
                PlayerMovementState.Walking => WalkSpeed,
                _ => 0f
            };

            Vector3 newPosition = npc.Position;
            switch (Direction)
            {
                case MovementDirection.Forward:
                    newPosition += npc.Player.CameraTransform.forward / 10 * speed;
                    break;
                case MovementDirection.Backwards:
                    newPosition -= npc.Player.CameraTransform.forward / 10 * speed;
                    break;
                case MovementDirection.Right:
                    newPosition += Quaternion.AngleAxis(90, Vector3.up) * npc.Player.CameraTransform.forward / 10 * speed;
                    break;
                case MovementDirection.Left:
                    newPosition -= Quaternion.AngleAxis(90, Vector3.up) * npc.Player.CameraTransform.forward / 10 * speed;
                    break;
            }

            if (npc.Position != newPosition && !Physics.Linecast(npc.Position, newPosition, FallDamage.StaticGroundMask))
                npc.Position = newPosition;
        }

        private IEnumerator<float> MovementCoroutine()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);
                if (IsPaused)
                    continue;

                if (FollowTarget != null)
                    Follow();
                else
                    Move();
            }
        }
    }
}
