// -----------------------------------------------------------------------
// <copyright file="AirlockController.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Doors
{
    using System.Collections.Generic;
    using System.Linq;

    using BaseController = Interactables.Interobjects.AirlockController;

    /// <summary>
    /// Represents airlock.
    /// </summary>
    public class AirlockController
    {
        private static readonly Dictionary<BaseController, AirlockController> BaseToExiledControllers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AirlockController"/> class.
        /// </summary>
        /// <param name="controller">Base-game controller.</param>
        public AirlockController(BaseController controller)
        {
            Base = controller;

            BaseToExiledControllers.Add(controller, this);
        }

        /// <summary>
        /// Gets the list with all airlocks.
        /// </summary>
        public static IReadOnlyCollection<AirlockController> List => BaseToExiledControllers.Values;

        /// <summary>
        /// Gets the basegame controller.
        /// </summary>
        public BaseController Base { get; }

        /// <summary>
        /// Gets the first subdoor.
        /// </summary>
        public Door DoorA => Door.Get(Base._doorA);

        /// <summary>
        /// Gets the second subdoor.
        /// </summary>
        public Door DoorB => Door.Get(Base._doorB);

        /// <summary>
        /// Gets or sets a value indicating whether or not both subdoors are locked.
        /// </summary>
        public bool DoorsLocked
        {
            get => Base._doorsLocked;
            set => Base._doorsLocked = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or airlock is disabled.
        /// </summary>
        public bool AirlockDisabled
        {
            get => Base.AirlockDisabled;
            set => Base.AirlockDisabled = value;
        }

        /// <summary>
        /// Gets the <see cref="AirlockController"/> by its base-game controller.
        /// </summary>
        /// <param name="controller">Base-game controller.</param>
        /// <returns>Instance of <see cref="AirlockController"/>.</returns>
        public static AirlockController Get(BaseController controller) => controller != null ? (BaseToExiledControllers.TryGetValue(controller, out AirlockController airlockController) ? airlockController : new AirlockController(controller)) : null;

        /// <summary>
        /// Gets the <see cref="AirlockController"/> by one of it's subdoors.
        /// </summary>
        /// <param name="door">Subdoor.</param>
        /// <returns>Instance of <see cref="AirlockController"/>.</returns>
        public static AirlockController Get(Door door) => BaseToExiledControllers.Values.FirstOrDefault(x => x.DoorA == door || x.DoorB == door);

        /// <summary>
        /// Toggles airlock.
        /// </summary>
        public void Toggle() => Base.ToggleAirlock();

        /// <summary>
        /// Returns the Door in a human-readable format.
        /// </summary>
        /// <returns>A string containing Door-related data.</returns>
        public override string ToString() => $"|{DoorA}| /{DoorB}/ *{DoorsLocked}* ={AirlockDisabled}=";
    }
}