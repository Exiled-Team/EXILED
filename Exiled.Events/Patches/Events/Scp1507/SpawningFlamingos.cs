// -----------------------------------------------------------------------
// <copyright file="SpawningFlamingos.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp1507
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp1507;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp1507;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp1507Spawner.Spawn"/>
    /// to add <see cref="Handlers.Scp1507.SpawningFlamingos"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.SpawningFlamingos))]
    [HarmonyPatch(typeof(Scp1507Spawner), nameof(Scp1507Spawner.Spawn))]
    internal class SpawningFlamingos
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldc_I4_4);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningFlamingosEventArgs));

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(Scp1507Spawner._alpha);
                    new(OpCodes.Ldsfld, Field(typeof(Scp1507Spawner), nameof(Scp1507Spawner._alpha))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // SpawningFlamingosEventArgs ev = new(Player, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningFlamingosEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp1507.OnSpawningFlamingos(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp1507), nameof(Handlers.Scp1507.OnSpawningFlamingos))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningFlamingosEventArgs), nameof(SpawningFlamingosEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // Scp1507Spawner._alpha = ev.NewAlpha;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningFlamingosEventArgs), nameof(SpawningFlamingosEventArgs.Player))),
                    new(OpCodes.Stsfld, Field(typeof(Scp1507Spawner), nameof(Scp1507Spawner._alpha))),
                });

            index = newInstructions.FindIndex(x => x.Is(OpCodes.Call, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))));

            newInstructions.RemoveAt(index);

            // replacing ReferenceHub.AllHubs to SpawningFlamingosEventArgs::PlayersToSpawn
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningFlamingosEventArgs), nameof(SpawningFlamingosEventArgs.PlayersToSpawn))),
                    new(OpCodes.Call, Method(typeof(SpawningFlamingos), nameof(ReturnEnumerator))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static HashSet<ReferenceHub> ReturnEnumerator(HashSet<Player> enumerable) => enumerable.Select(x => x.ReferenceHub).ToHashSet();
    }
}