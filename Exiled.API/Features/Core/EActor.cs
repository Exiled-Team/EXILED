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

    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Components;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.DynamicEvents;
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
#pragma warning disable SA1310
        public const float DEFAULT_FIXED_TICK_RATE = TickComponent.DEFAULT_FIXED_TICK_RATE;
#pragma warning restore SA1310

        private readonly HashSet<EActor> componentsInChildren = HashSetPool<EActor>.Pool.Get();
        private CoroutineHandle serverTick;
        private bool canEverTick = true;
        private float fixedTickRate;

        /// <summary>
        /// Initializes a new instance of the <see cref="EActor"/> class.
        /// </summary>
        protected EActor()
            : base()
        {
            IsEditable = true;
            fixedTickRate = DEFAULT_FIXED_TICK_RATE;
            PostInitialize();
            Timing.CallDelayed(fixedTickRate, OnBeginPlay);
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
        /// <param name="from"><see cref="GameObject"/>.</param>
        /// <param name="to"><see cref="EActor"/>.</param>
        public static void AttachTo(GameObject from, EActor to) => to.Base = from;

        /// <summary>
        /// Attaches a <see cref="EActor"/> to the specified <see cref="EActor"/>.
        /// </summary>
        /// <param name="from">The source actor.</param>
        /// <param name="to">The actor to be modified.</param>
        public static void AttachTo(EActor from, EActor to) => to.Base = from.Base;

        /// <inheritdoc/>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = CreateDefaultSubobject<T>(Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (!component)
                return null;

            componentsInChildren.Add(component);

            return component.Cast(out T param) ? param : throw new InvalidCastException("The provided EActor cannot be cast to the specified type.");
        }

        /// <inheritdoc/>
        public T AddComponent<T>(Type type, string name = "")
            where T : EActor
        {
            T component = CreateDefaultSubobject<T>(type, Base, string.IsNullOrEmpty(name) ? $"{type.Name}-Component#{ComponentsInChildren.Count}" : name).Cast<T>();
            if (!component)
                return null;

            componentsInChildren.Add(component);

            return component.Cast(out T param) ? param : throw new InvalidCastException("The provided EActor cannot be cast to the specified type.");
        }

        /// <inheritdoc/>
        public T AddComponent<T>(EActor actor, string name = "")
            where T : EActor
        {
            if (!actor)
                throw new NullReferenceException("The provided EActor is null.");

            if (!string.IsNullOrEmpty(name))
                actor.Name = name;

            AttachTo(Base, actor);
            componentsInChildren.Add(actor);

            return actor.Cast(out T param) ? param : throw new InvalidCastException("The provided EActor cannot be cast to the specified type.");
        }

        /// <inheritdoc/>
        public EActor AddComponent(Type type, string name = "")
        {
            EActor component = CreateDefaultSubobject(type, Base, string.IsNullOrEmpty(name) ? $"{GetType().Name}-Component#{ComponentsInChildren.Count}" : name).Cast<EActor>();
            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public EActor AddComponent(EActor actor, string name = "")
        {
            if (!actor)
                throw new NullReferenceException("The provided EActor is null.");

            if (!string.IsNullOrEmpty(name))
                actor.Name = name;

            AttachTo(Base, actor);
            componentsInChildren.Add(actor);

            return actor;
        }

        /// <inheritdoc/>
        public IEnumerable<EActor> AddComponents(IEnumerable<Type> types)
        {
            foreach (Type t in types)
                yield return AddComponent(t);
        }

        /// <inheritdoc/>
        public IEnumerable<EActor> AddComponents(IEnumerable<EActor> actors)
        {
            foreach (EActor actor in actors)
                yield return AddComponent(actor);
        }

        /// <inheritdoc/>
        public IEnumerable<T> AddComponents<T>(IEnumerable<T> actors)
            where T : EActor
        {
            foreach (T actor in actors)
                yield return AddComponent<T>(actor);
        }

        /// <inheritdoc/>
        public IEnumerable<T> AddComponents<T>(IEnumerable<EActor> types)
            where T : EActor
        {
            foreach (EActor type in types)
                yield return AddComponent<T>(type);
        }

        /// <inheritdoc />
        public T RemoveComponent<T>(string name = "")
            where T : EActor
        {
            T comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent<T>(out comp))
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor actor in GetComponents<T>())
            {
                if (actor.Name != name)
                    continue;

                comp = actor.Cast<T>();
            }

            return comp;
        }

        /// <inheritdoc />
        public T RemoveComponent<T>(EActor actor, string name = "")
            where T : EActor
        {
            T comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent<T>(out comp) || comp != actor)
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor component in GetComponents<T>())
            {
                if (component.Name != name && component == actor)
                    continue;

                comp = component.Cast<T>();
            }

            return comp;
        }

        /// <inheritdoc />
        public EActor RemoveComponent(Type type, string name = "")
        {
            EActor comp = null;

            if (string.IsNullOrEmpty(name))
            {
                if (!TryGetComponent(type, out comp))
                    return null;

                comp.Base = null;
                componentsInChildren.Remove(comp);
                return comp;
            }

            foreach (EActor actor in GetComponents(type))
            {
                if (actor.Name != name)
                    continue;

                comp = actor;
            }

            return comp;
        }

        /// <inheritdoc />
        public EActor RemoveComponent(EActor actor, string name = "")
        {
            if (!componentsInChildren.Contains(actor))
                return null;

            if (string.IsNullOrEmpty(name))
            {
                actor.Base = null;
                componentsInChildren.Remove(actor);
                return actor;
            }

            foreach (EActor component in componentsInChildren)
            {
                if (component != actor || actor.Name != name)
                    continue;

                actor = component;
            }

            return actor;
        }

        /// <inheritdoc />
        public IEnumerable<T> RemoveComponentOfType<T>(string name = "")
            where T : EActor
        {
            IEnumerable<T> components = GetComponents<T>();

            foreach (T comp in components)
                RemoveComponent(comp, name);

            return components;
        }

        /// <inheritdoc />
        public IEnumerable<EActor> RemoveComponentOfType(Type type, string name = "")
        {
            IEnumerable<EActor> components = GetComponents(type);

            foreach (EActor comp in components)
                RemoveComponent(comp, name);

            return components;
        }

        /// <inheritdoc />
        public void RemoveComponents(IEnumerable<Type> types) => types.ForEach(type => RemoveComponent(type));

        /// <inheritdoc />
        public void RemoveComponents(IEnumerable<EActor> actors) => actors.ForEach(actor => RemoveComponent(actor));

        /// <inheritdoc />
        public void RemoveComponents<T>(IEnumerable<T> actors)
            where T : EActor => actors.ForEach(actor => RemoveComponent(actor));

        /// <inheritdoc />
        public void RemoveComponents<T>(IEnumerable<EActor> types)
            where T : EActor => types.ForEach(type => RemoveComponent(type));

        /// <inheritdoc/>
        public EActor GetComponent(Type type) => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <inheritdoc/>
        public T GetComponent<T>()
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public T GetComponent<T>(Type type)
            where T : EActor => ComponentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>() => ComponentsInChildren.Where(comp => typeof(T).IsAssignableFrom(comp.GetType())).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<T> GetComponents<T>(Type type) => ComponentsInChildren.Where(comp => typeof(T).IsAssignableFrom(comp.GetType())).Cast<T>();

        /// <inheritdoc/>
        public IEnumerable<EActor> GetComponents(Type type) => ComponentsInChildren.Where(comp => type.IsAssignableFrom(comp.GetType()));

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
            ? ComponentsInChildren.Any(comp => comp is T)
            : ComponentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <inheritdoc/>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? ComponentsInChildren.Any(type.IsInstanceOfType)
            : ComponentsInChildren.Any(comp => type == comp.GetType());

        /// <summary>
        /// Fired after the <see cref="EActor"/> instance is created.
        /// </summary>
        protected virtual void PostInitialize()
        {
            Log.InfoWithContext(nameof(PostInitialize));
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