// -----------------------------------------------------------------------
// <copyright file="Speaker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Toys
{
    using System.Linq;

    using AdminToys;
    using Enums;
    using Interfaces;

    /// <summary>
    /// A wrapper class for <see cref="SpeakerToy"/>.
    /// </summary>
    public class Speaker : AdminToy, IWrapper<SpeakerToy>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Speaker"/> class.
        /// </summary>
        /// <param name="speakerToy">The <see cref="SpeakerToy"/> of the toy.</param>
        internal Speaker(SpeakerToy speakerToy)
            : base(speakerToy, AdminToyType.Speaker) => Base = speakerToy;

        /// <summary>
        /// Gets the base <see cref="SpeakerToy"/>.
        /// </summary>
        public SpeakerToy Base { get; }

        /// <summary>
        /// Gets or sets the controller Id of the SpeakerToy.
        /// </summary>
        public byte ControllerId
        {
            get => Base.NetworkControllerId;
            set => Base.NetworkControllerId = value;
        }

        /// <summary>
        /// Gets or sets the volume of the audio source.
        /// </summary>
        /// <remarks>0.0 is silent and 1.0 is full volume.</remarks>
        public float Volume
        {
            get => Base.NetworkVolume;
            set => Base.NetworkVolume = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the audio source is spatialize.
        /// </summary>
        /// <remarks>If true, the audio source will become spatial, which will become 3D relative to the position of the player listening to it.</remarks>
        public bool IsSpatial
        {
            get => Base.NetworkIsSpatial;
            set => Base.NetworkIsSpatial = value;
        }

        /// <summary>
        /// Gets or sets the minimum distance the audio source can be heard.
        /// </summary>
        public float MinDistance
        {
            get => Base.NetworkMinDistance;
            set => Base.NetworkMinDistance = value;
        }

        /// <summary>
        /// Gets or sets the maximum distance the audio source can be heard.
        /// </summary>
        public float MaxDistance
        {
            get => Base.NetworkMaxDistance;
            set => Base.NetworkMaxDistance = value;
        }

        /// <summary>
        /// Gets the <see cref="Speaker"/> belonging to the <see cref="SpeakerToy"/>.
        /// </summary>
        /// <param name="speakerToy">The <see cref="SpeakerToy"/> instance.</param>
        /// <returns>The corresponding <see cref="SpeakerToy"/> instance.</returns>
        public static Speaker Get(SpeakerToy speakerToy)
        {
            AdminToy adminToy = Map.Toys.FirstOrDefault(x => x.AdminToyBase == speakerToy);
            return adminToy is not null ? adminToy as Speaker : new Speaker(speakerToy);
        }
    }
}