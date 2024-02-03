// -----------------------------------------------------------------------
// <copyright file="IEntity.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the contract for basic ECS implementation.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> of <see cref="EActor"/> containing all the components in children.
        /// </summary>
        abstract IReadOnlyCollection<EActor> ComponentsInChildren { get; }

        /// <summary>
        /// Adds a component to the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be added.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public abstract T AddComponent<T>(string name = "")
            where T : EActor;

        /// <summary>
        /// Adds a component to the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public abstract EActor AddComponent(Type type, string name = "");

        /// <summary>
        /// Adds a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> cast <see cref="EActor"/> type.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be added.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The added <see cref="EActor"/> component.</returns>
        public abstract T AddComponent<T>(Type type, string name = "")
            where T : EActor;

        /// <summary>
<<<<<<< HEAD
=======
        /// Adds multiple components to the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="types">The collection of <see cref="Type"/> representing the components to be added.</param>
        /// <returns>The added <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<EActor> AddComponents(IEnumerable<Type> types);

        /// <summary>
        /// Adds multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="actors">The collection of <see cref="EActor"/> instances to be added.</param>
        /// <returns>The added <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<EActor> AddComponents(IEnumerable<EActor> actors);

        /// <summary>
        /// Adds multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> type to be added.</typeparam>
        /// <param name="actors">The collection of <typeparamref name="T"/> <see cref="EActor"/> instances to be added.</param>
        /// <returns>The added <typeparamref name="T"/> <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<T> AddComponents<T>(IEnumerable<T> actors)
            where T : EActor;

        /// <summary>
        /// Adds multiple components to the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> type to be added.</typeparam>
        /// <param name="types">The collection of <see cref="Type"/> representing the components to be added.</param>
        /// <returns>The added <typeparamref name="T"/> <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<T> AddComponents<T>(IEnumerable<EActor> types)
            where T : EActor;

        /// <summary>
        /// Removes a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to be removed.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <typeparamref name="T"/> <see cref="EActor"/> component.</returns>
        public abstract T RemoveComponent<T>(string name = "")
            where T : EActor;

        /// <summary>
        /// Removes a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> cast <see cref="EActor"/> type.</typeparam>
        /// <param name="actor">The <see cref="EActor"/> to be removed.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <typeparamref name="T"/> <see cref="EActor"/> component.</returns>
        public abstract T RemoveComponent<T>(EActor actor, string name = "")
            where T : EActor;

        /// <summary>
        /// Removes a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be removed.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <see cref="EActor"/> component.</returns>
        public abstract EActor RemoveComponent(Type type, string name = "");

        /// <summary>
        /// Removes a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="actor">The <see cref="EActor"/> to be removed.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <see cref="EActor"/> component.</returns>
        public abstract EActor RemoveComponent(EActor actor, string name = "");

        /// <summary>
        /// Removes all components of the specified type from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> type to be removed.</typeparam>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <typeparamref name="T"/> <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<T> RemoveComponentOfType<T>(string name = "")
            where T : EActor;

        /// <summary>
        /// Removes all components of the specified type from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to be removed.</param>
        /// <param name="name">The name of the component.</param>
        /// <returns>The removed <see cref="EActor"/> components.</returns>
        public abstract IEnumerable<EActor> RemoveComponentOfType(Type type, string name = "");

        /// <summary>
        /// Removes multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="types">The collection of <see cref="Type"/> representing the components to be removed.</param>
        public abstract void RemoveComponents(IEnumerable<Type> types);

        /// <summary>
        /// Removes multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="actors">The collection of <see cref="EActor"/> instances to be removed.</param>
        public abstract void RemoveComponents(IEnumerable<EActor> actors);

        /// <summary>
        /// Removes multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> type to be removed.</typeparam>
        /// <param name="actors">The collection of <typeparamref name="T"/> <see cref="EActor"/> instances to be removed.</param>
        public abstract void RemoveComponents<T>(IEnumerable<T> actors)
            where T : EActor;

        /// <summary>
        /// Removes multiple components from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> type to be removed.</typeparam>
        /// <param name="types">The collection of <see cref="Type"/> representing the components to be removed.</param>
        public abstract void RemoveComponents<T>(IEnumerable<EActor> types)
            where T : EActor;

        /// <summary>
>>>>>>> apis-rework
        /// Gets a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public abstract T GetComponent<T>()
            where T : EActor;

        /// <summary>
        /// Gets a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="EActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public abstract T GetComponent<T>(Type type)
            where T : EActor;

        /// <summary>
        /// Gets a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to look for.</param>
        /// <returns>The <see cref="EActor"/> component.</returns>
        public abstract EActor GetComponent(Type type);

        /// <summary>
        /// Tries to get a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The <typeparamref name="T"/> <see cref="EActor"/> to look for.</typeparam>
        /// <param name="component">The <typeparamref name="T"/> <see cref="EActor"/>.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent<T>(out T component)
            where T : EActor;

        /// <summary>
        /// Tries to get a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent(Type type, out EActor component);

        /// <summary>
        /// Tries to get a component from the <see cref="IEntity"/>.
        /// </summary>
        /// <typeparam name="T">The cast <typeparamref name="T"/> <see cref="EActor"/>.</typeparam>
        /// <param name="type">The <see cref="Type"/> of the <see cref="EActor"/> to get.</param>
        /// <param name="component">The found component.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool TryGetComponent<T>(Type type, out T component)
            where T : EActor;

        /// <summary>
        /// Checks if the <see cref="IEntity"/> has an active component.
        /// </summary>
        /// <typeparam name="T">The <see cref="EActor"/> to look for.</typeparam>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool HasComponent<T>(bool depthInheritance = false);

        /// <summary>
        /// Checks if the <see cref="IEntity"/> has an active component.
        /// </summary>
        /// <param name="type">The <see cref="EActor"/> to look for.</param>
        /// <param name="depthInheritance">A value indicating whether or not subclasses should be considered.</param>
        /// <returns><see langword="true"/> if the component was found; otherwise, <see langword="false"/>.</returns>
        public abstract bool HasComponent(Type type, bool depthInheritance = false);
    }
}