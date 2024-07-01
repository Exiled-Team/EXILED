// -----------------------------------------------------------------------
// <copyright file="LosingSignal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp079;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp079;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079LostSignalHandler.ServerLoseSignal"/>.
    /// Adds the <see cref="Handlers.Scp079.LosingSignal" /> event and <see cref="Handlers.Scp079.LostSignal"/> for SCP-079.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.LosingSignal))]
    [EventPatch(typeof(Handlers.Scp079), nameof(Handlers.Scp079.LostSignal))]
    [HarmonyPatch(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.ServerLoseSignal))]
    internal static class LosingSignal
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(LosingSignalEventArgs));

            newInstructions.InsertRange(0, new CodeInstruction[]
                {
                    // this.Role._lastOwner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Role))),
                    new(OpCodes.Ldfld, Field(typeof(PlayerRoleBase), nameof(PlayerRoleBase._lastOwner))),

                    // duration
                    new(OpCodes.Ldarg_1),

                    // LosingSignalEventArgs ev = new(ReferenceHub, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LosingSignalEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Scp079.OnLosingSignal(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnLosingSignal))),

                    // if (ev.IsAllowed) return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LosingSignalEventArgs), nameof(LosingSignalEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse, retLabel),

                    // duration = ev.Duration
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(LosingSignalEventArgs), nameof(LosingSignalEventArgs.Duration))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
                {
                    // this.Role._lastOwner
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079LostSignalHandler), nameof(Scp079LostSignalHandler.Role))),
                    new(OpCodes.Ldfld, Field(typeof(PlayerRoleBase), nameof(PlayerRoleBase._lastOwner))),

                    // duration
                    new(OpCodes.Ldarg_1),

                    // LostSignalEventArgs ev = new(ReferenceHub, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LostSignalEventArgs))[0]),

                    // Scp079.OnLostSignal(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp079), nameof(Handlers.Scp079.OnLostSignal))),
                });
            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}