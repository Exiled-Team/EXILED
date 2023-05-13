// -----------------------------------------------------------------------
// <copyright file="FilmMakerRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using PlayerRoles;
    using UnityEngine;

    using FilmmakerGameRole = PlayerRoles.Filmmaker.FilmmakerRole;

    /// <summary>
    /// Represents the BaseGame FilmMakerRole.
    /// </summary>
    public class FilmMakerRole : Role
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FilmMakerRole"/> class.
        /// </summary>
        /// <param name="filmmakerRole">the base <see cref="FilmmakerGameRole"/>.</param>
        internal FilmMakerRole(FilmmakerGameRole filmmakerRole)
            : base(filmmakerRole)
        {
            Base = filmmakerRole;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Filmmaker;

        /// <summary>
        /// Gets or sets the filmmaker's camera rotation.
        /// </summary>
        public Quaternion CameraRotation
        {
            get => Base.CameraRotation;
            set => Base.CameraRotation = value;
        }

        /// <summary>
        /// Gets the base <see cref="FilmmakerGameRole"/>.
        /// </summary>
        public new FilmmakerGameRole Base { get; }
    }
}