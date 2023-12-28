namespace Exiled.CustomModules.API.Features.CustomItems
{
    using System;

    /// <inheritdoc/>
    public abstract class CustomItem<T> : CustomItem
        where T : ItemBehaviour
    {
        /// <inheritdoc/>
        public override Type BehaviourComponent => typeof(T);
    }
}
