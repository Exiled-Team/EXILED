// -----------------------------------------------------------------------
// <copyright file="CommentsObjectGraphVisitor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization
{
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.ObjectGraphVisitors;

    /// <summary>
    /// Source: https://dotnetfiddle.net/8M6iIE.
    /// </summary>
    public sealed class CommentsObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsObjectGraphVisitor"/> class.
        /// </summary>
        /// <param name="nextVisitor">The next visitor instance.</param>
        public CommentsObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor)
            : base(nextVisitor)
        {
        }

        /// <inheritdoc/>
        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            if (value is CommentsObjectDescriptor commentsDescriptor && commentsDescriptor.Comment is not null)
            {
                foreach (string subComment in commentsDescriptor.Comment.Split('\n'))
                {
                    context.Emit(new Comment(subComment, false));
                }
            }

            return base.EnterMapping(key, value, context);
        }
    }
}
