// -----------------------------------------------------------------------
// <copyright file="Coffee.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using UnityEngine;

    using BaseCoffee = global::Coffee;

    /// <summary>
    /// A wrapper for coffee cup.
    /// </summary>
    public class Coffee : IWrapper<BaseCoffee>
    {
        /// <summary>
        /// Gets the <see cref="Dictionary{TKey,TValue}"/> with <see cref="BaseCoffee"/> to <see cref="Coffee"/>.
        /// </summary>
        internal static readonly Dictionary<BaseCoffee, Coffee> BaseToWrapper = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Coffee"/> class.
        /// </summary>
        /// <param name="coffee"><inheritdoc cref="Base"/></param>
        public Coffee(BaseCoffee coffee)
        {
            Base = coffee;

            BaseToWrapper.Add(coffee, this);
        }

        /// <summary>
        /// Gets or sets an <see cref="IEnumerable{T}"/> of players who cannot interact with coffees cups.
        /// </summary>
        public static IEnumerable<Player> BlacklistedPlayers
        {
            get => BaseCoffee.BlacklistedPlayers.Select(Player.Get);
            set
            {
                BaseCoffee.BlacklistedPlayers.Clear();

                foreach (var player in value)
                    BaseCoffee.BlacklistedPlayers.Add(player.ReferenceHub);
            }
        }

        /// <summary>
        /// Gets the list with all available <see cref="Coffee"/> instanses.
        /// </summary>
        public static IReadOnlyCollection<Coffee> List => BaseToWrapper.Values;

        /// <summary>
        /// Gets the base coffee class instance.
        /// </summary>
        public BaseCoffee Base { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not coffee had been drunk.
        /// </summary>
        public bool IsConsumed
        {
            get => Base.IsConsumed;
            set => Base.NetworkIsConsumed = value;
        }

        /// <summary>
        /// Gets or sets text which will be displayed to player when he drinks coffee.
        /// </summary>
        public CoffeeTranslation CoffeeTranslation
        {
            get => Base._drinkText;
            set => Base._drinkText = value;
        }

        /// <summary>
        /// Gets or sets an author of current <see cref="CoffeeTranslation"/>.
        /// </summary>
        public string TranslationAuthor
        {
            get => Base._author;
            set => Base._author = value;
        }

        /// <summary>
        /// Gets or sets a color of a drink in a cup.
        /// </summary>
        public Color DrinkColor
        {
            get => Base._drinkColor;
            set => Base._drinkColor = value;
        }

        /// <summary>
        /// Gets a coffee by it's base class.
        /// </summary>
        /// <param name="baseCoffee">Base class.</param>
        /// <returns><see cref="Coffee"/> instance.</returns>
        public static Coffee Get(BaseCoffee baseCoffee) => BaseToWrapper.TryGetValue(baseCoffee, out Coffee coffee) ? coffee : new(baseCoffee);

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of coffee which matches condition.
        /// </summary>
        /// <param name="predicate">Condition to satisfy.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of coffee which matches condition.</returns>
        public static IEnumerable<Coffee> Get(Func<Coffee, bool> predicate) => List.Where(predicate);

        /// <summary>
        /// Interacts with coffee.
        /// </summary>
        /// <param name="player">Player who interact. If <see langword="null"/>, will be chosen random.</param>
        public void Interact(Player player = null) => Base.ServerInteract((player ?? Player.Get(x => x.IsHuman).GetRandomValue()).ReferenceHub, byte.MaxValue);
    }
}