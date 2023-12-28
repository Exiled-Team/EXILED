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

    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Features.Pools;
    using Exiled.API.Interfaces;
    using MEC;

    using UnityEngine;

    /// <summary>
    /// Actor is the base class for a <see cref="EObject"/> that can be placed or spawned in-game.
    /// </summary>
    public abstract class EActor : EObject, IEntity, IWorldSpace
    {
        /// <summary>
        /// The default fixed tick rate.
        /// </summary>
        public const float DefaultFixedTickRate = TickComponent.DefaultFixedTickRate;

        private readonly HashSet<EActor> componentsInChildren = HashSetPool<EActor>.Pool.Get();
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
        /// Gets or sets a value indicating whether the <see cref="EActor"/> should be destroyed the next tick.
        /// </summary>
        public bool DestroyNextTick { get; set; }

        /// <inheritdoc/>
        public IReadOnlyCollection<EActor> ComponentsInChildren => componentsInChildren;

        /// <summary>
        /// Gets the <see cref="UnityEngine.Transform"/>.
        /// </summary>
        public Transform Transform => Base.transform;

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
        protected IEnumerable<EActor> ComponentsInParent => FindActiveObjectsOfType<EActor>().Where(actor => actor.ComponentsInChildren.Any(comp => comp == this));

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

        /// <inheritdoc/>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (component is null)
                return null;

            componentsInChildren.Add(component);
            return component.Cast<T>();
        }

        /// <inheritdoc/>
        public EActor AddComponent(Type type, string name = "")
        {
            EActor component = CreateDefaultSubobject(type, Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<EActor>();
            if (component is null)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public T AddComponent<T>(Type type, string name = "")
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public EActor GetComponent(Type type) => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <inheritdoc/>
        public T GetComponent<T>()
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public T GetComponent<T>(Type type)
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>() => ComponentsInChildren.Where(comp => typeof(T) == comp.GetType() || comp.GetType().IsSubclassOf(typeof(T)) || comp.GetType().BaseType == typeof(T)).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>(Type type) => ComponentsInChildren.Where(comp => type == comp.GetType() || comp.GetType().IsSubclassOf(type) || comp.GetType().BaseType == type).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<EActor> GetComponents(Type type) => ComponentsInChildren.Where(comp => type == comp.GetType() || comp.GetType().IsSubclassOf(type) || comp.GetType().BaseType == type);

        /// <inheritdoc/>
        public bool TryGetComponent<T>(Type type, out T component)
            where T : EActor
        {
            EActor actor = GetComponent(type);

            if (actor.Cast(out component))
                component = actor.Cast<T>();

            return component;
        }

        /// <inheritdoc/>
        public bool TryGetComponent<T>(out T component)
            where T : EActor
        {
            component = null;

            if (HasComponent<T>())
                component = GetComponent<T>().Cast<T>();

            return component is not null;
        }

        /// <inheritdoc/>
        public bool TryGetComponent(Type type, out EActor component)
        {
            component = null;

            if (HasComponent(type))
                component = GetComponent(type);

            return component is not null;
        }

        /// <inheritdoc/>
        public bool HasComponent<T>(bool depthInheritance = false) => depthInheritance
            ? ComponentsInChildren.Any(comp => typeof(T).IsSubclassOf(comp.GetType()))
            : ComponentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <inheritdoc/>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? ComponentsInChildren.Any(comp => type.IsSubclassOf(comp.GetType()))
            : ComponentsInChildren.Any(comp => type == comp.GetType());

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
            if (DestroyNextTick)
            {
                Destroy();
                return;
            }
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
            StaticActor.Get<DynamicEventManager>().BindAllFromTypeInstance(this);
        }

        /// <summary>
        /// Unsubscribes all the events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
            StaticActor.Get<DynamicEventManager>().UnbindAllFromTypeInstance(this);
        }

        /// <inheritdoc/>
        protected override void OnBeginDestroy()
        {
            base.OnBeginDestroy();

            HashSetPool<EActor>.Pool.Return(componentsInChildren);
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