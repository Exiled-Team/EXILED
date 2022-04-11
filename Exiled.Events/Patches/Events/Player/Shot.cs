// -----------------------------------------------------------------------
// <copyright file="Shot.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;
    using InventorySystem.Items.Firearms.BasicMessages;
    using InventorySystem.Items.Firearms.Modules;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1512 // Single-line comments should not be followed by blank line
#pragma warning disable SA1005 // Single line comments should begin with single space
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

    /// <summary>
    /// Patches <see cref="FirearmBasicMessagesHandler.ServerShotReceived"/>.
    /// Adds the <see cref="Handlers.Player.Shooting"/> and <see cref="Handlers.Player.Shot"/> events.
    /// </summary>
    [HarmonyPatch(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.ServerPerformShot))]
    internal static class Shot
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 2;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(FirearmBaseStats), nameof(FirearmBaseStats.DamageAtDistance))) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player player = Player.Get(this.Hub)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SingleBulletHitreg), nameof(SingleBulletHitreg.Hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // distance = hitInfo.distance
                new(OpCodes.Ldloc_3),

                // component (IDestructible)
                new(OpCodes.Ldloc, 5),

                // damage
                new(OpCodes.Ldloc, 6),

                // var ev = new ShotEventArgs(player, distance, component, damage)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnShot(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                // if (!ev.CanHurt)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                new(OpCodes.Brfalse, returnLabel),

                // damage = ev.Damage
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.Damage))),
                new(OpCodes.Stloc, 6),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        /// <summary>
        /// Patches <see cref="BuckshotHitreg.ShootPellet"/>.
        /// Adds the <see cref="Handlers.Player.Shooting"/> and <see cref="Handlers.Player.Shot"/> events.
        /// </summary>
        [HarmonyPatch(typeof(BuckshotHitreg), nameof(BuckshotHitreg.ShootPellet))]
        internal static class ShotPellets
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

                int offset = 1;
<<<<<<< HEAD
                int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_S) + offset;
=======
                int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;
>>>>>>> Exiled-Team-dev

                LocalBuilder ev = generator.DeclareLocal(typeof(ShotEventArgs));

                Label returnLabel = generator.DefineLabel();

                newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    // Player player = Player.Get(this.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(BuckshotHitreg), nameof(BuckshotHitreg.Hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // distance = hitInfo.distance
                    new(OpCodes.Ldloc_2),

                    // component (IDestructible)
                    new(OpCodes.Ldloc, 3),

                    // damage
                    new(OpCodes.Ldloc, 4),

                    // var ev = new ShotEventArgs(player, distance, component, damage)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ShotEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc, ev.LocalIndex),

                    // Handlers.Player.OnShot(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnShot))),

                    // if (!ev.CanHurt)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.CanHurt))),
                    new(OpCodes.Brfalse, returnLabel),

                    // damage = ev.Damage
                    new(OpCodes.Ldloc, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ShotEventArgs), nameof(ShotEventArgs.Damage))),
                    new(OpCodes.Stloc, 5),
                });

                newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

                for (int z = 0; z < newInstructions.Count; z++)
                    yield return newInstructions[z];

                ListPool<CodeInstruction>.Shared.Return(newInstructions);
            }
        }
    }
}
