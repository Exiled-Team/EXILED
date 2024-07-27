// -----------------------------------------------------------------------
// <copyright file="ScenesType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// Unique identifier for the different types of Scenes the client and server can load.
    /// </summary>
    public enum ScenesType
    {

        /// <summary>
        /// The scene both the server and player have and its the facility itself
        /// </summary>
        Facility,

        /// <summary>
        /// The current main menu (Only Client-Side)
        /// ! Will cause crash when trying joining servers !
        /// </summary>
        NewMainMenu,

        /// <summary>
        /// The old menu (Only Client-Side)
        /// </summary>
        MainMenuRemastered,

        /// <summary>
        /// The old server screen (Only Client-Side)
        /// </summary>
        FastMenu,

        /// <summary>
        /// Its Loading screen at the start of the game.
        /// ! Will cause crash when trying joining servers !
        /// </summary>
        PreLoader,

        /// <summary>
        /// A black menu before loading the <see cref="NewMainMenu"/>
        /// </summary>
        Loader,
    }
}
