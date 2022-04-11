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
        public const float DefaultFixedTickRate = 0.016f;

        private CoroutineHandle serverTick;
        private bool canEverTick;
        private float fixedTickRate = DefaultFixedTickRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EActor"/> class.
        /// </summary>
        /// <param name="gameObject"><inheritdoc cref="EObject.Base"/></param>
        public EActor(GameObject gameObject = null)
            : base(gameObject)
        {
            CanEverTick = true;
            Timing.CallDelayed(fixedTickRate, () => OnBeginPlay());
            Timing.CallDelayed(fixedTickRate * 2, () => serverTick = Timing.RunCoroutine(ServerTick()));
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
        /// Gets a <see cref="EActorComponent"/>[] containing all the components in parent.
        /// </summary>
        protected internal EActor[] ComponentsInParent => FindActiveObjectsOfType<EActor>().Where(actor => actor.ComponentsInChildren.Any(comp => comp == this)).ToArray();

        /// <summary>
        /// Gets a <see cref="EActorComponent"/>[] containing all the components in children.
        /// </summary>
        protected internal HashSet<EActor> ComponentsInChildren { get; } = new HashSet<EActor>();

        /// <summary>
        /// Adds a component to this actor.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name);
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
        /// <returns>The added <see cref="EActor"/> component.</returns>
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
        /// <returns>The <see cref="EActor"/> component.</returns>
        public T GetComponent<T>()
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <summary>
        /// Gets a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The <see cref="EActor"/> component.</returns>
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
            component = GetComponent<T>();

            return component != null;
        }

        /// <summary>
        /// Tries to get a component from this actor.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetComponent(Type type, out EActor component)
        {
            component = GetComponent(type);

            return component != null;
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
        protected virtual void OnBeginPlay()
        {
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
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            Timing.KillCoroutines(serverTick);

            foreach (EActor parent in ComponentsInParent)
                parent.ComponentsInChildren.Remove(this);

            OnEndPlay();
        }

        private IEnumerator<float> ServerTick()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(FixedTickRate);
                if (!CanEverTick)
                    continue;

                Tick();
            }
        }
    }
}
