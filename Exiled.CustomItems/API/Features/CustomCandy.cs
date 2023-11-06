using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Scp330;
using InventorySystem.Items.Usables.Scp330;
using MEC;

namespace Exiled.CustomItems.API.Features
{
    public abstract class CustomCandy : CustomItem
    {
        internal static new readonly Dictionary<ushort, List<int>> TrackedSerials = new();

        /// <inheritdoc/>
        /// <exception cref="NotSupportedException">Throws when trying to set value to property.</exception>
        public override ItemType Type
        {
            get => ItemType.SCP330;
            set => throw new NotSupportedException("Cannot set ItemType for CustomCandy. To set candy use CandyType.");
        }

        /// <summary>
        /// Gets or sets <see cref="CandyKindID"/> of custom candy.
        /// </summary>
        public abstract CandyKindID CandyType { get; set; }

        private void OnUsingScp330(InteractingScp330EventArgs ev)
        {
            if (ev.Candy == CandyType)
            {
                Timing.CallDelayed(.5f, () =>
                {
                    if (!ev.Player.TryGetItem(ItemType.SCP330, out Item item) || !item.Is(out Scp330 scp330))
                        return;

                    if (TrackedSerials.ContainsKey(scp330.Serial))
                        TrackedSerials[scp330.Serial].Add(scp330.Candies.Count - 1);
                    else
                        TrackedSerials.Add(scp330.Serial, new List<int> { 0 });
                });
            }
        }
    }
}