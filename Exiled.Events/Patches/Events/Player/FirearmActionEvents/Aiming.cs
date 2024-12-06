// -----------------------------------------------------------------------
// <copyright file="Aiming.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player.FirearmActionEvents
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Firearms.Modules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="LinearAdsModule.ServerProcessCmd" />.
    /// Adds <see cref="Handlers.Player.AimingDownSight" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.AimingDownSight))]
    [HarmonyPatch(typeof(LinearAdsModule), nameof(LinearAdsModule.ServerProcessCmd))]
    internal static class Aiming
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnAimStatusChanged(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret);

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Firearm.Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(LinearAdsModule), nameof(LinearAdsModule.Firearm))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(InventorySystem.Items.Firearms.Firearm), nameof(InventorySystem.Items.Firearms.Firearm.Owner))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // (Firearm)Item.Get(this.Firearm)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(LinearAdsModule), nameof(LinearAdsModule.Firearm))),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(InventorySystem.Items.Firearms.Firearm) })),
                new(OpCodes.Castclass, typeof(Firearm)),

                // this._userInput
                new(OpCodes.Ldfld, Field(typeof(LinearAdsModule), nameof(LinearAdsModule._userInput))),

                // AimingDownSightEventArgs args = new(Player, Firearm, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AimingDownSightEventArgs))[0]),

                // Player.OnAimingDownSight(args)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnAimingDownSight))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}