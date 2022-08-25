// -----------------------------------------------------------------------
// <copyright file="InteractingGeneratorEvents.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Player.ActivatingGenerator"/>, <see cref="Player.ClosingGenerator"/>, <see cref="Player.OpeningGenerator"/>, <see cref="Player.UnlockingGenerator"/> and <see cref="Player.StoppingGenerator"/> events.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    internal static class InteractingGeneratorEvents
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));

            Label isOpen = generator.DefineLabel();
            Label isActivating = generator.DefineLabel();
            Label check = generator.DefineLabel();
            Label check2 = generator.DefineLabel();
            Label notAllowed = generator.DefineLabel();
            Label skip = generator.DefineLabel();
            Label @break = newInstructions.FindLast(i => i.IsLdarg(0)).labels[0];

            int offset = 1;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(Stopwatch), nameof(Stopwatch.Stop)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Stloc_S, player.LocalIndex),
            });

            offset = -9;
            index = newInstructions.FindIndex(i => i.Calls(Method(typeof(Scp079Generator), nameof(Scp079Generator.ServerSetFlag)))) + offset;

            // if (this.HasFlag(_flags, GeneratorFlags.Open))
            // {
            //     ClosingGeneratorEventArgs ev = new(player, this, true);
            //     Player.OnClosingGenerator(ev);
            //     if(!ev.IsAllowed)
            //     {
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            // else
            // {
            //     OpeningGeneratorEventArgs ev = new(player, this, true);
            //     Player.OnOpeningGenerator(ev);
            //     if(!ev.IsAllowed)
            //     {
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._flags))),
                new(OpCodes.Ldc_I4_4), // GeneratorFlags.Open
                new(OpCodes.Callvirt, Method(typeof(Scp079Generator), nameof(Scp079Generator.HasFlag))),
                new(OpCodes.Brfalse_S, isOpen),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ClosingGeneratorEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnClosingGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ClosingGeneratorEventArgs), nameof(ClosingGeneratorEventArgs.IsAllowed))),
                new(OpCodes.Br_S, check),

                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningGeneratorEventArgs))[0]).WithLabels(isOpen),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnOpeningGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningGeneratorEventArgs), nameof(OpeningGeneratorEventArgs.IsAllowed))),

                new CodeInstruction(OpCodes.Brtrue_S, skip).WithLabels(check),

                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(notAllowed),
                new(OpCodes.Callvirt, Method(typeof(Scp079Generator), nameof(Scp079Generator.RpcDenied))),
                new(OpCodes.Br_S, @break),

                new CodeInstruction(OpCodes.Nop).WithLabels(skip),
            });

            offset = 7;
            index = newInstructions.FindIndex(i => i.LoadsField(Field(typeof(Scp079Generator), nameof(Scp079Generator._requiredPermission)))) + offset;

            // remove base game unlocking(we will unlock generator after UnlockingGeneratorEventArgs invoke and allowed check)
            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(i => i.Calls(Method(typeof(Scp079Generator), nameof(Scp079Generator.RpcDenied)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
            });

            index += 3;

            // remove base game RpcDenied(same as unlocking)
            newInstructions.RemoveRange(index, 2);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnlockingGeneratorEventArgs))[0]).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnlockingGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UnlockingGeneratorEventArgs), nameof(UnlockingGeneratorEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, notAllowed),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_2), // GeneratorFlags.Unlocked
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, Method(typeof(Scp079Generator), nameof(Scp079Generator.ServerSetFlag))),
            });

            offset = -5;
            index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(Scp079Generator), nameof(Scp079Generator.Activating)))) + offset;

            // if (this.Activating)
            // {
            //     StoppingGeneratorEventArgs ev = new(player, this, true);
            //     Player.OnStoppingGenerator(ev);
            //     if(!ev.IsAllowed)
            //     {
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            // else
            // {
            //     ActivatingGeneratorEventArgs ev = new(player, this, true);
            //     Player.OnActivatingGenerator(ev);
            //     if(!ev.IsAllowed)
            //     {
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079Generator), nameof(Scp079Generator.Activating))),
                new(OpCodes.Brfalse_S, isActivating),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingGeneratorEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnStoppingGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingGeneratorEventArgs), nameof(StoppingGeneratorEventArgs.IsAllowed))),
                new(OpCodes.Br_S, check2),

                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingGeneratorEventArgs))[0]).WithLabels(isActivating),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnActivatingGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingGeneratorEventArgs), nameof(ActivatingGeneratorEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, notAllowed).WithLabels(check2),
            });

            offset = 2;
            index = newInstructions.FindLastIndex(i => i.Calls(PropertyGetter(typeof(Scp079Generator), nameof(Scp079Generator.Engaged)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingGeneratorEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnStoppingGenerator))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingGeneratorEventArgs), nameof(StoppingGeneratorEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, notAllowed),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
