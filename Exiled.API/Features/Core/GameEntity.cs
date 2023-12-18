// -----------------------------------------------------------------------
// <copyright file="GameEntity.cs" company="Exiled Team">
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

    using UnityEngine;

    /// <summary>
    /// The base class which defines in-game entities.
    /// </summary>
    public abstract class GameEntity : TypeCastObject<GameEntity>, IEntity
    {
        private readonly HashSet<EActor> componentsInChildren = new();

        /// <inheritdoc/>
        public IReadOnlyCollection<EActor> ComponentsInChildren => componentsInChildren;

        /// <summary>
        /// Gets the <see cref="GameEntity"/>'s <see cref="UnityEngine.GameObject"/>.
        /// </summary>
        public abstract GameObject EntityBase { get; }

        /// <inheritdoc/>
        public T AddComponent<T>(string name = "")
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(EntityBase);

            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public EActor AddComponent(Type type, string name = "")
        {
            EActor component = EObject.CreateDefaultSubobject(type, EntityBase).Cast<EActor>();

            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public T AddComponent<T>(Type type, string name = "")
            where T : EActor
        {
            T component = EObject.CreateDefaultSubobject<T>(type, EntityBase);
            if (!component)
                return null;

            componentsInChildren.Add(component);
            return component;
        }

        /// <inheritdoc/>
        public T GetComponent<T>()
            where T : EActor => componentsInChildren.FirstOrDefault(comp => typeof(T) == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public T GetComponent<T>(Type type)
            where T : EActor => componentsInChildren.FirstOrDefault(comp => type == comp.GetType()).Cast<T>();

        /// <inheritdoc/>
        public EActor GetComponent(Type type) => componentsInChildren.FirstOrDefault(comp => type == comp.GetType());

        /// <inheritdoc/>
        public bool TryGetComponent<T>(out T component)
            where T : EActor => component = GetComponent<T>();

        /// <inheritdoc/>
        public bool TryGetComponent(Type type, out EActor component) => component = GetComponent(type);

        /// <inheritdoc/>
        public bool TryGetComponent<T>(Type type, out T component)
            where T : EActor => component = GetComponent<T>(type);

        /// <inheritdoc/>
        public bool HasComponent<T>(bool depthInheritance = false) => depthInheritance
            ? componentsInChildren.Any(comp => typeof(T).IsSubclassOf(comp.GetType()))
            : componentsInChildren.Any(comp => typeof(T) == comp.GetType());

        /// <inheritdoc/>
        public bool HasComponent(Type type, bool depthInheritance = false) => depthInheritance
            ? componentsInChildren.Any(comp => type.IsSubclassOf(comp.GetType()))
            : componentsInChildren.Any(comp => type == comp.GetType());
    }
}