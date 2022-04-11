// -----------------------------------------------------------------------
// <copyright file="AnnouncingScpTermination.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using static HarmonyLib.AccessTools;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;
    using Map = Exiled.Events.Handlers.Map;

    /// <summary>
    /// Patches <see cref="NineTailedFoxAnnouncer.AnnounceScpTermination(ReferenceHub, DamageHandlerBase)"/>.
    /// Adds the <see cref="Map.AnnouncingScpTermination"/> event.
    /// </summary>
    [HarmonyPatch(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.AnnounceScpTermination))]
    internal static class AnnouncingScpTermination
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingScpTerminationEventArgs));

            Label ret = generator.DefineLabel();
            Label jcc = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            newInstructions.RemoveRange(0, 19);

            newInstructions.InsertRange(0, new[]
            {
<<<<<<< HEAD
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.Handler))),
                new CodeInstruction(OpCodes.Isinst, typeof(DamageHandler)),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Base))),
                new CodeInstruction(OpCodes.Starg, 1),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.singleton))),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.scpListTimer))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.CurRole))),
                new CodeInstruction(OpCodes.Stloc_0),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.team))),
                new CodeInstruction(OpCodes.Brtrue_S, jmp),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.roleId))),
                new CodeInstruction(OpCodes.Ldc_I4_S, 10),
                new CodeInstruction(OpCodes.Bne_Un_S, jcc),
=======
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingScpTerminationEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingScpTermination))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.Handler))),
                new(OpCodes.Isinst, typeof(DamageHandler)),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DamageHandler), nameof(DamageHandler.Base))),
                new(OpCodes.Starg, 1),
                new(OpCodes.Ldsfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.singleton))),
                new(OpCodes.Ldc_R4, 0f),
                new(OpCodes.Stfld, Field(typeof(NineTailedFoxAnnouncer), nameof(NineTailedFoxAnnouncer.scpListTimer))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(CharacterClassManager), nameof(CharacterClassManager.CurRole))),
                new(OpCodes.Stloc_0),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.team))),
                new(OpCodes.Brtrue_S, jmp),
                new(OpCodes.Ldloc_0),
                new(OpCodes.Ldfld, Field(typeof(Role), nameof(Role.roleId))),
                new(OpCodes.Ldc_I4_S, 10),
                new(OpCodes.Bne_Un_S, jcc),
>>>>>>> Exiled-Team-dev
                new CodeInstruction(OpCodes.Ret).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(jcc),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingScpTerminationEventArgs), nameof(AnnouncingScpTerminationEventArgs.TerminationCause))),
                new(OpCodes.Stloc_1),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
