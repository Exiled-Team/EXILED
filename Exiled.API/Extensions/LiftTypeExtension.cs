// -----------------------------------------------------------------------
// <copyright file="LiftTypeExtension.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Contains an extension method to get <see cref="ElevatorType"/> from <see cref="Lift"/>.
    /// Internal class <see cref="RegisterElevatorTypesOnLevelLoad"/> to cache the <see cref="ElevatorType"/> on level load.
    /// </summary>
    public static class LiftTypeExtension
    {
        private static readonly Dictionary<int, ElevatorType> OrderedElevatorTypes = new Dictionary<int, ElevatorType>();

        /// <summary>
        /// Gets the <see cref="ElevatorType"/>.
        /// </summary>
        /// <param name="lift">The <see cref="Lift"/> to check.</param>
        /// <returns>The <see cref="ElevatorType"/>.</returns>
        public static ElevatorType Type(this Lift lift) => OrderedElevatorTypes.TryGetValue(lift.GetInstanceID(), out var elevatorType) ? elevatorType : ElevatorType.Unknown;

        /// <summary>
        /// Gets all the <see cref="ElevatorType"/> values for for the <see cref="Lift"/> instances using <see cref="Lift.elevatorName"/> and <see cref="UnityEngine.GameObject"/> name.
        /// </summary>
        internal static void RegisterElevatorTypesOnLevelLoad()
        {
            OrderedElevatorTypes.Clear();

            var lifts = Map.Lifts;

            if (lifts == null)
                return;

            var liftCount = lifts.Count;
            for (int i = 0; i < liftCount; i++)
            {
                var lift = lifts[i];
                var liftID = lift.GetInstanceID();

                var liftName = string.IsNullOrWhiteSpace(lift.elevatorName) ? lift.elevatorName.RemoveBracketsOnEndOfName() : lift.elevatorName;

                var elevatorType = GetElevatorType(liftName);

                OrderedElevatorTypes.Add(liftID, elevatorType);
            }
        }

        private static ElevatorType GetElevatorType(string elevatorName)
        {
            switch (elevatorName)
            {
                case "":
                {
                    return ElevatorType.Nuke;
                }

                case "ElA":
                case "ElA2":
                {
                    return ElevatorType.LczA;
                }

                case "ElB":
                case "ElB2":
                {
                    return ElevatorType.LczB;
                }

                case "GateA":
                {
                    return ElevatorType.GateA;
                }

                case "GateB":
                {
                    return ElevatorType.GateB;
                }

                case "SCP-049":
                {
                    return ElevatorType.Scp049;
                }

                default:
                {
                    return ElevatorType.Unknown;
                }
            }
        }
    }
}
