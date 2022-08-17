// -----------------------------------------------------------------------
// <copyright file="TickComponent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MEC;

    /// <summary>
    /// The component which handles tick related features.
    /// </summary>
    public class TickComponent : EObject
    {
        /// <summary>
        /// The default fixed tick rate.
        /// </summary>
        public const float DefaultFixedTickRate = 0.016f;

        private readonly HashSet<CoroutineHandle> boundHandles;
        private readonly CoroutineHandle executeAllHandle;
        private bool canEverTick;

        /// <summary>
        /// Initializes a new instance of the <see cref="TickComponent"/> class.
        /// </summary>
        protected TickComponent()
            : base()
        {
            CanEverTick = true;
            executeAllHandle = Timing.RunCoroutine(ExecuteAll());
            boundHandles = new();
        }

        /// <summary>
        /// Gets or sets the current tick rate.
        /// </summary>
        public float TickRate { get; set; } = DefaultFixedTickRate;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EActor"/> can tick.
        /// </summary>
        public bool CanEverTick
        {
            get => canEverTick;
            set
            {
                if (!IsEditable || canEverTick == value)
                    return;

                canEverTick = value;

                if (canEverTick)
                {
                    Timing.ResumeCoroutines(executeAllHandle);
                    Timing.ResumeCoroutines(boundHandles.ToArray());
                }
                else
                {
                    Timing.PauseCoroutines(executeAllHandle);
                    Timing.PauseCoroutines(boundHandles.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets a <see cref="List{T}"/> of <see cref="Action"/> containing all the delegates to be invoked.
        /// </summary>
        public List<Action> Instructions { get; } = new();

        /// <summary>
        /// Gets all the currently bound handles.
        /// </summary>
        public IReadOnlyCollection<CoroutineHandle> BoundHandles => boundHandles;

        /// <summary>
        /// Binds a <see cref="CoroutineHandle"/>.
        /// </summary>
        /// <param name="handle">The <see cref="CoroutineHandle"/> to bind.</param>
        public void BindHandle(CoroutineHandle handle) => boundHandles.Add(handle);

        /// <summary>
        /// Binds a <see cref="CoroutineHandle"/>.
        /// </summary>
        /// <param name="handle">The <see cref="CoroutineHandle"/> to bind.</param>
        /// <param name="coroutine">The coroutine to handle.</param>
        public void BindHandle(ref CoroutineHandle handle, IEnumerator<float> coroutine) => BindHandle(handle = Timing.RunCoroutine(coroutine));

        /// <summary>
        /// Unbinds a <see cref="CoroutineHandle"/>.
        /// </summary>
        /// <param name="handle">The <see cref="CoroutineHandle"/> to unbind.</param>
        public void UnbindHandle(CoroutineHandle handle)
        {
            Timing.KillCoroutines(handle);
            boundHandles.RemoveWhere(ax => ax == handle);
        }

        /// <summary>
        /// Unbinds all the currently bound handles.
        /// </summary>
        public void UnbindAllHandles()
        {
            Timing.KillCoroutines(boundHandles.ToArray());
            boundHandles.Clear();
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            UnbindAllHandles();
            Timing.KillCoroutines(executeAllHandle);
        }

        private IEnumerator<float> ExecuteAll()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(TickRate);

                foreach (Action action in Instructions)
                {
                    try
                    {
                        action();
                    }
                    catch(Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }
        }
    }
}
