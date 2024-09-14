// -----------------------------------------------------------------------
// <copyright file="EnumClass.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Interfaces;
    using LiteNetLib.Utils;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A class which allows <see cref="Enum"/> implicit conversions.
    /// <para>Can be used along with <see cref="Enum"/>, means it doesn't require another <see cref="EnumClass{TSource, TObject}"/> instance to be comparable or usable.</para>
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to handle the instance of.</typeparam>
    /// <typeparam name="TObject">The type of the child object to handle the instance of.</typeparam>
    public abstract class EnumClass<TSource, TObject> : IComparable, IEquatable<TObject>, IComparable<TObject>, IEnumClass
        where TSource : Enum
        where TObject : EnumClass<TSource, TObject>
    {
        private static SortedList<TSource, TObject> values;
        private static bool isDefined;

        private string name;

        static EnumClass()
        {
            values = new SortedList<TSource, TObject>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumClass{TSource, TObject}"/> class.
        /// Required for YAML deserialization.
        /// </summary>
        protected EnumClass()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumClass{TSource, TObject}"/> class.
        /// </summary>
        /// <param name="value">The value of the enum item.</param>
        protected EnumClass(TSource value)
        {
            Value = value;
            values.Add(value, (TObject)this);
        }

        /// <summary>
        /// Gets all <typeparamref name="TObject"/> object instances.
        /// </summary>
        [YamlIgnore]
        public static IEnumerable<TObject> Values => values.Values;

        /// <summary>
        /// Gets the value of the enum item.
        /// </summary>
        [YamlIgnore]
        public TSource Value { get; private set; }

        /// <summary>
        /// Gets the name determined from reflection.
        /// </summary>
        [YamlMember(Alias = "name")]
        public string Name
        {
            get
            {
                if (isDefined)
                    return name;

                IEnumerable<FieldInfo> fields = typeof(TObject)
                    .GetFields(BindingFlags.Static | BindingFlags.GetField | BindingFlags.Public)
                    .Where(t => t.FieldType == typeof(TObject));

                foreach (FieldInfo field in fields)
                {
                    TObject instance = (TObject)field.GetValue(null);
                    instance.name = field.Name;
                }

                isDefined = true;
                return name;
            }

            private set
            {
                TObject obj = Parse(value);
                if (obj is null)
                {
                    Log.ErrorWithContext($"The enum class ({GetType().Name}) requires a valid name. The current name will remain unchanged.", Log.CONTEXT_SERIALIZATION);
                    return;
                }

                Value = obj.Value;
                name = value;
            }
        }

        /// <summary>
        /// Implicitly converts the <see cref="EnumClass{TSource, TObject}"/> to <typeparamref name="TSource"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator TSource(EnumClass<TSource, TObject> value) => value.Value;

        /// <summary>
        /// Implicitly converts the <see cref="EnumClass{TSource, TObject}"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator string(EnumClass<TSource, TObject> value) => value.Name;

        /// <summary>
        /// Implicitly converts the <typeparamref name="TSource"/> to <see cref="EnumClass{TSource, TObject}"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        public static implicit operator EnumClass<TSource, TObject>(TSource value) => values[value];

        /// <summary>
        /// Casts the specified <paramref name="value"/> to the corresponding type.
        /// </summary>
        /// <param name="value">The enum value to be cast.</param>
        /// <returns>The cast object.</returns>
        public static TObject Cast(TSource value) => values[value];

        /// <summary>
        /// Casts the specified <paramref name="values"/> to the corresponding type.
        /// </summary>
        /// <param name="values">The enum values to be cast.</param>
        /// <returns>The cast object.</returns>
        public static IEnumerable<TObject> Cast(IEnumerable<TSource> values) => values.Select(value => EnumClass<TSource, TObject>.values[value]);

        /// <summary>
        /// Retrieves an array of the values of the constants in a specified <see cref="EnumClass{TSource, TObject}"/>.
        /// </summary>
        /// <param name="type">The <see cref="EnumClass{TSource, TObject}"/> type.</param>
        /// <returns>An array of the values of the constants in a specified <see cref="EnumClass{TSource, TObject}"/>.</returns>
        public static TSource[] GetValues(Type type)
        {
            if (type is null)
                throw new NullReferenceException("The specified type parameter is null");

            if (!type.IsSubclassOf(typeof(EnumClass<TSource, TObject>)) && type.BaseType != typeof(EnumClass<TSource, TObject>))
                throw new InvalidTypeException("The specified type parameter is not a EnumClass<TSource, TObject> type.");

            return typeof(TSource)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField)
                .Where(field => field.FieldType == typeof(TSource))
                .Select(field => (TSource)field.GetValue(null))
                .ToArray();
        }

        /// <summary>
        /// Safely casts the specified <paramref name="value"/> to the corresponding type.
        /// </summary>
        /// <param name="value">The enum value to be cast.</param>
        /// <param name="result">The cast <paramref name="value"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="value"/> was cast; otherwise, <see langword="false"/>.</returns>
        public static bool SafeCast(TSource value, out TObject result) => values.TryGetValue(value, out result);

        /// <summary>
        /// Safely casts the specified <paramref name="values"/> to the corresponding type.
        /// </summary>
        /// <param name="values">The enum value to be cast.</param>
        /// <param name="results">The cast <paramref name="values"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="values"/> was cast; otherwise, <see langword="false"/>.</returns>
        public static bool SafeCast(IEnumerable<TSource> values, out IEnumerable<TObject> results)
        {
            results = null;

            List<TObject> tmpValues = ListPool<TObject>.Pool.Get();
            foreach (TSource value in values)
            {
                if (!EnumClass<TSource, TObject>.values.TryGetValue(value, out TObject result))
                    return false;

                tmpValues.Add(result);
            }

            results = tmpValues;
            return true;
        }

        /// <summary>
        /// Parses a <see cref="string"/> object.
        /// </summary>
        /// <param name="obj">The object to be parsed.</param>
        /// <returns>The corresponding <typeparamref name="TObject"/> object instance, or <see langword="null"/> if not found.</returns>
        public static TObject Parse(string obj) => values.Values.FirstOrDefault(value => string.Compare(value.Name, obj, StringComparison.OrdinalIgnoreCase) == 0);

        /// <summary>
        /// Converts the <see cref="EnumClass{TSource, TObject}"/> instance to a human-readable <see cref="string"/> representation.
        /// </summary>
        /// <returns>A human-readable <see cref="string"/> representation of the <see cref="EnumClass{TSource, TObject}"/> instance.</returns>
        public override string ToString() => Name;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(TSource other) => Value.Equals(other);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(TObject other)
        {
            if (other is null)
                return false;

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj) => obj is TSource value ? Equals(value) : Equals(obj as TObject);

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => Value.GetHashCode();

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(TSource other) => Comparer<TSource>.Default.Compare(Value, other);

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="other">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(TObject other)
        {
            if (other is null)
                return 1;

            return CompareTo(other.Value);
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns
        /// an integer that indicates whether the current instance precedes, follows, or
        /// occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>
        /// A value that indicates the relative order of the objects being compared.
        /// The return value has these meanings: Value Meaning Less than zero This instance precedes other in the sort order.
        /// Zero This instance occurs in the same position in the sort order as other.
        /// Greater than zero This instance follows other in the sort order.
        /// </returns>
        public int CompareTo(object obj) => obj is TSource value ? CompareTo(value) : CompareTo(obj as TObject);

    }
}
