// -----------------------------------------------------------------------
// <copyright file="CommentsObjectGraphVisitor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    extern alias Yaml;

    using Yaml::YamlDotNet.Core.Events;

    using IEmitter = Yaml::YamlDotNet.Core.IEmitter;
    using IObjectDescriptor = Yaml::YamlDotNet.Serialization.IObjectDescriptor;
    using IPropertyDescriptor = Yaml::YamlDotNet.Serialization.IPropertyDescriptor;

    /// <summary>
    /// Source: https://dotnetfiddle.net/8M6iIE.
    /// </summary>
    public sealed class CommentsObjectGraphVisitor : Yaml::YamlDotNet.Serialization.ObjectGraphVisitors.ChainedObjectGraphVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsObjectGraphVisitor"/> class.
        /// </summary>
        /// <param name="nextVisitor">The next visitor instance.</param>
        public CommentsObjectGraphVisitor(Yaml::YamlDotNet.Serialization.IObjectGraphVisitor<IEmitter> nextVisitor)
            : base(nextVisitor)
        {
        }

        /// <inheritdoc/>
        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
            {
                context.Emit(new Comment(commentsDescriptor.Comment, false));
            }

            return base.EnterMapping(key, value, context);
        }
    }
}