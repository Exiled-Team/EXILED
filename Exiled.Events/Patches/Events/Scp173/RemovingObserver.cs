// -----------------------------------------------------------------------
// <copyright file="RemovingObserver.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp173;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.Subroutines;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173ObserversTracker.UpdateObserver(ReferenceHub)" />.
    /// Adds the <see cref="Handlers.Scp173.RemovingObserver" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.RemovingObserver))]
    [HarmonyPatch(typeof(Scp173ObserversTracker), nameof(Scp173ObserversTracker.UpdateObserver))]
    internal static class RemovingObserver
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions =
                ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(RemovingObserverEventArgs));

            int index = newInstructions.FindLastIndex(x => x.Calls(Method(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Remove)))) + 2;
            newInstructions[index].WithLabels(returnLabel);
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(Owner);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,
                    PropertyGetter(typeof(StandardSubroutine<Scp173Role>), nameof(StandardSubroutine<Scp173Role>.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player.Get(ply);
                new(OpCodes.Ldarg_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // RemovingObserverEventArgs ev = new(Player.Get(Owner), Player.Get(ply));
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingObserverEventArgs))[0]),
                new(OpCodes.Stloc, ev),

                // Scp173.OnRemovingObserver(ev);
                new(OpCodes.Ldloc, ev),
                new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnRemovingObserver))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}