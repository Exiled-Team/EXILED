// -----------------------------------------------------------------------
// <copyright file="Dancing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.EventArgs.Scp3114;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp3114Dance.ServerProcessCmd"/>
    /// to add <see cref="Handlers.Scp3114.Dancing"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp3114Dance), nameof(Scp3114Dance.ServerProcessCmd))]
    internal class Dancing
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Brfalse_S);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(DancingEventArgs));

            // reader.ReadBool()
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Dance), nameof(Scp3114Dance.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DancingEventArgs ev = new(bool, Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DancingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp3114.OnDancing(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Scp3114), nameof(Handlers.Scp3114.OnDancing))),

                    // if (ev.IsAllowed)
                    //   goto continueLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DancingEventArgs), nameof(DancingEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // return
                    new(OpCodes.Ret),

                    // ev.NewState
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DancingEventArgs), nameof(DancingEventArgs.IsDancing))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp3114Dance.ServerProcessCmd"/>
    /// to implement <see cref="Exiled.API.Features.Roles.Scp3114Role.DanceType"/> setter.
    /// </summary>
    [HarmonyPatch(typeof(Scp3114Dance), nameof(Scp3114Dance.ServerWriteRpc))]
    internal class ChooseDanceType
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -4;
            int index = newInstructions.FindIndex(x => x.operand is MethodInfo methodInfo && methodInfo == Method(typeof(NetworkWriter), nameof(NetworkWriter.WriteByte))) + offset;

            newInstructions.RemoveRange(index, 4);

            // replace "writer.WriteByte((byte)UnityEngine.Random.Range(0, 255))"
            // with "writer.WriteByte(ChooseDanceType.Handle(Player.Get(this.Owner)))"
            newInstructions.InsertRange(index, new CodeInstruction[]
                {
                    // Handle(Player.Get(this.Owner));
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(ChooseDanceType), nameof(Handle))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static byte Handle(Scp3114Dance scp3114Dance)
        {
            DanceType danceType = DanceType.Random;

            if (Player.TryGet(scp3114Dance.Owner, out Player player) && player.Role.Is(out API.Features.Roles.Scp3114Role role))
            {
                danceType = role.DanceType;
            }

            if (danceType == DanceType.Random)
            {
                byte val = (byte)UnityEngine.Random.Range(0, 255);
                Log.Info(val);
                return val;
            }

            return (byte)((byte)danceType * scp3114Dance._danceVariants);
        }
    }
}