// -----------------------------------------------------------------------
// <copyright file="Punching.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="MutantHands.ServerProcessMessage"/> method.
    /// Adds the <see cref="Handlers.Player.Punching"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MutantHands), nameof(MutantHands.ServerProcessMessage))]
    public class Punching
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            // IL_0037: ldfld        class [UnityEngine.CoreModule]UnityEngine.Transform ReferenceHub::PlayerCameraReference
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 14;
            var index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_0) + offset;

            var returnLabel = generator.DefineLabel();
            var trueLabel = generator.DefineLabel();
            var continueLabel = generator.DefineLabel();

            API.Features.Log.Debug($"i {index}");
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(attacker)
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(MutantHands), nameof(MutantHands.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // List<Player> targets = new List<Player>
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(List<API.Features.Player>))[0]),
                new CodeInstruction(OpCodes.Stloc_0),

                // foreach (HitTarget target in desiredTargets)
                //     targets.Add(Player.Get(target.Target))
                new CodeInstruction(OpCodes.Ldarg_3), // desiredTargets
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(List<MutantHands.HitTarget>), nameof(List<MutantHands.HitTarget>))),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Br_S, continueLabel),
                new CodeInstruction(OpCodes.Ldloca_S, 1).WithLabels(trueLabel),
                new CodeInstruction(OpCodes.Call, Method(typeof(List<MutantHands.HitTarget>), nameof(List<MutantHands.HitTarget>.Enumerator.Current))),
                new CodeInstruction(OpCodes.Stloc_2),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(MutantHands.HitTarget.Target))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(List<API.Features.Player>), nameof(List<API.Features.Player>.Add))),
                new CodeInstruction(OpCodes.Ldloca_S, 1).WithLabels(continueLabel),
                new CodeInstruction(OpCodes.Call, Method(typeof(bool), nameof(List<MutantHands.HitTarget>.Enumerator.MoveNext))),
                new CodeInstruction(OpCodes.Brtrue_S, trueLabel),

                // True
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new PunchingEventArgs(attacker, target, true)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(PunchingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnPunching(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnPunching))),

                // if(!ev.isAllowed) return
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PunchingEventArgs), nameof(PunchingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
            {
                API.Features.Log.Debug(z);
                API.Features.Log.Debug(newInstructions[z]);
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /*private static bool Prefix(MutantHands __instance, Vector3 position, Quaternion rotation, List<MutantHands.HitTarget> desiredTargets)
        {
            List<API.Features.Player> targets = new List<API.Features.Player>();
            foreach (MutantHands.HitTarget target in desiredTargets)
            {
                targets.Add(API.Features.Player.Get(target.Target));
            }

            var ev = new PunchingEventArgs(API.Features.Player.Get(__instance.Owner), targets, true);
            Handlers.Player.OnPunching(ev);
            return false;
        }*/
    }
}
