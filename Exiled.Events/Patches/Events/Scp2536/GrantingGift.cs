// -----------------------------------------------------------------------
// <copyright file="GrantingGift.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp2536
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Christmas.Scp2536;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp2536;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp2536GiftController.GrantGift"/>
    /// to add <see cref="Handlers.Scp2536.GrantingGift"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.GrantingGift))]
    [HarmonyPatch(typeof(Scp2536GiftController), nameof(Scp2536GiftController.GrantGift))]
    internal class GrantingGift
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(GrantingGiftEventArgs));

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // Player.Get(hub);
                    new(OpCodes.Ldarg_2),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // gift
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // GrantingGiftEventArgs ev = new(Player, Scp2536GiftBase, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(GrantingGiftEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp2536.OnGrantingGift(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp2536), nameof(Handlers.Scp2536.OnGrantingGift))),

                    // if (!ev.IsAllowed)
                    //   goto retLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // gift = ev.Gift;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GrantingGiftEventArgs), nameof(GrantingGiftEventArgs.Gift))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}