// -----------------------------------------------------------------------
// <copyright file="EActor.cs" company="Exiled Team">
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

    using UnityEngine;

    /// <summary>
    /// Actor is the base class for a <see cref="EObject"/> that can be placed or spawned in-game.
    /// </summary>
    public abstract class EActor : EObject
    {
        /// <summary>
        /// The default fixed tick rate.
        /// </summary>
        public const float DefaultFixedTickRate = TickComponent.DefaultFixedTickRate;

        private CoroutineHandle serverTick;
        private bool canEverTick;
        private float fixedTickRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EActor"/> class.
        /// </summary>
        protected EActor()
            : base()
        {
            IsEditable = true;
            CanEverTick = true;
            fixedTickRate = DefaultFixedTickRate;
            PostInitialize();
            Timing.CallDelayed(fixedTickRate, () => OnBeginPlay());
            Timing.CallDelayed(fixedTickRate * 2, () => serverTick = Timing.RunCoroutine(ServerTick()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EActor"/> class.
        /// </summary>
        /// <param name="gameObject">The base <see cref="GameObject"/>.</param>
        protected EActor(GameObject gameObject = null)
            : this()
        {
            if (gameObject)
                Base = gameObject;
        }

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform
        {
            get => Base.transform;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">position</see>.
        /// </summary>
        public virtual Vector3 Position
        {
            get => Transform.position;
            set => Transform.position = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Quaternion">rotation</see>.
        /// </summary>
        public virtual Quaternion Rotation
        {
            get => Transform.rotation;
            set => Transform.rotation = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="Vector3">scale</see>.
        /// </summary>
        public virtual Vector3 Scale
        {
            get => Transform.localScale;
            set => Transform.localScale = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="EActor"/> can tick.
        /// </summary>
        public virtual bool CanEverTick
        {
            get => canEverTick;
            set
            {
                if (!IsEditable)
                    return;

                canEverTick = value;

                if (canEverTick)
                {
                    Timing.ResumeCoroutines(serverTick);
                    return;
                }

                Timing.PauseCoroutines(serverTick);
            }
        }

        /// <summary>
        /// Gets or sets the value which determines the size of every tick.
        /// </summary>
        public virtual float FixedTickRate
        {
            get => fixedTickRate;
            set
            {
                if (!IsEditable)
                    return;

                fixedTickRate = value;
            }
        }

        /// <summary>
        /// Gets a <see cref="EActor"/>[] containing all the components in parent.
        /// </summary>
        protected EActor[] ComponentsInParent
        {
            get => FindActiveObjectsOfType<EActor>().Where(actor => actor.ComponentsInChildren.Any(comp => comp == this)).ToArray();
        }

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="EActor"/> containing all the components in children.
        /// </summary>
        protected HashSet<EActor> ComponentsInChildren { get; } = new();

        /// <summary>
        /// Attaches a <see cref="EActor"/> to the specified <see cref="GameObject"/>.
        /// </summary>
        /// <param name="comp"><see cref="EActor"/>.</param>
        /// <param name="gameObject"><see cref="GameObject"/>.</param>
        public static void AttachTo(EActor comp, GameObject gameObject) => comp.Base = gameObject;

        /// <summary>
        /// Attaches a <see cref="EActor"/> to the specified <see cref="EActor"/>.
        /// </summary>
        /// <param name="to">The actor to be modified.</param>
        /// <param name="from">The source actor.</param>
        public static void AttachTo(EActor to, EActor from) => to.Base = from.Base;

        /// <summary>
        /// Adds a component to this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added component.</returns>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (component is null)
                return null;

            ComponentsInChildren.Add(component);
            return component.Cast<T>();
        }

        /// <summary>
        /// Adds a component to this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added component.</returns>
        public EActor AddComponent(Type type, string name = "")
        {
            EActor component = CreateDefaultSubobject(type, Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<EActor>();
            if (component is null)
                return null;

            ComponentsInChildren.Add(component);
            return component;
        }

        /// <summary>
        /// Gets a component from this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <returns>The corresponding component or <see langword="null"/> if not found.</returns>
        public T GetComponent<T>()
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <summary>
        /// Gets a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The corresponding component or <see langword="null"/> if not found.</returns>
        public EActor GetComponent(Type type) => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <summary>
        /// Tries to get a component from this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <param name="component">The <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent<T>(out T component)
            where T : EActor
        {
            component = null;

            if (HasComponent<T>())
                component = GetComponent<T>().Cast<T>();

            return component is not null;
        }

        /// <summary>
        /// Tries to get a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent(Type type, out EActor component)
        {
            component = null;

            if (HasComponent(type))
                component = GetComponent(type);

            return component is not null;
        }

        /// <summary>
        /// Checks if the actor has an active component.
        /// </summary>
        /// <typeparam name="T">The <see cref="EActor"/> to look for.</typeparam>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent<T>()
            where T : EActor => ComponentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <summary>
        /// Checks if the actor has an active component.
        /// </summary>
        /// <param name="type">The <see cref="EActor"/> to look for.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool HasComponent(Type type) => ComponentsInChildren.Any(comp => type == comp.GetType());

        /// <summary>
        /// Fired after the <see cref="EActor"/> instance is created.
        /// </summary>
        protected virtual void PostInitialize()
        {
        }

        /// <summary>
        /// Fired after the first fixed tick.
        /// </summary>
        protected virtual void OnBeginPlay()
        {
            SubscribeEvents();
        }

        /// <summary>
        /// Fired every tick.
        /// </summary>
        protected virtual void Tick()
        {
        }

        /// <summary>
        /// Fired before the current <see cref="EActor"/> instance is destroyed.
        /// </summary>
        protected virtual void OnEndPlay()
        {
            UnsubscribeEvents();
        }

        /// <summary>
        /// Subscribes all the events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes all the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            Timing.KillCoroutines(serverTick);
            OnEndPlay();
        }

        private IEnumerator<float> ServerTick()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(FixedTickRate);

                Tick();
            }
        }
    }
}