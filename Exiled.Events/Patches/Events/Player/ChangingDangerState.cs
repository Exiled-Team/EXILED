// -----------------------------------------------------------------------
// <copyright file="ChangingDangerState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
#pragma warning disable SA1402 // File may only contain a single type

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects.Danger;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Adds <see cref="ChangingDangerState"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDangerState))]
    [HarmonyPatch(typeof(DangerStackBase), nameof(DangerStackBase.IsActive), MethodType.Setter)]
    internal class ChangingDangerState
    {
        private static bool Prefix(DangerStackBase __instance, bool value)
        {
            DangerType type = __instance.GetDangerType();

            if (value == __instance._isActive)
                return false;

            ChangingDangerStateEventArgs ev = new(Player.Get(__instance.Owner), __instance, type, value);
            Handlers.Player.OnChangingDangerState(ev);

            return ev.IsAllowed;
        }
    }

    /// <summary>
    /// Adds <see cref="ChangingDangerState"/> event on encounter dangers.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDangerState))]
    [HarmonyPatch(typeof(EncounterDangerBase), nameof(EncounterDangerBase.RegisterEncounter))]
    internal class ChangingDangerStateEncounter
    {
        private static bool Prefix(EncounterDangerBase __instance, ReferenceHub target)
        {
            DangerType type = __instance.GetDangerType();

            ChangingDangerStateEventArgs ev = new(Player.Get(__instance.Owner), __instance, type, true, Player.Get(target));
            Handlers.Player.OnChangingDangerState(ev);

            return ev.IsAllowed;
        }
    }

    /// <summary>
    /// Adds <see cref="ChangingDangerState"/> event on encounter dangers.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDangerState))]
    [HarmonyPatch(typeof(PlayerDamagedDanger), nameof(PlayerDamagedDanger.UpdateState))]
    internal class ChangingDangerStateDamage
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingGroupEventArgs));

            Label returnLabel = generator.DefineLabel();

            int offset = -6;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(List<DangerStackBase>), nameof(List<DangerStackBase>.Add)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerDamagedDanger), nameof(PlayerDamagedDanger.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // this.GetDangerType()
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(DangerTypeExtensions), nameof(DangerTypeExtensions.GetDangerType))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // null
                    new(OpCodes.Ldnull),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingDangerStateEventArgs ev = new(Player, ParentDangerBase, DangerType, bool, Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDangerStateEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnChangingDangerState(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingDangerState))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDangerStateEventArgs), nameof(ChangingDangerStateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Adds <see cref="ChangingDangerState"/> event on encounter dangers.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingDangerState))]
    [HarmonyPatch(typeof(ParentDangerBase), nameof(ParentDangerBase.ProcessChildren))]
    internal class ChangingDangerStateChildrenEnding
    {
        private static Player GetEncounteredPlayer(ParentDangerBase danger, int index) => danger.ChildDangers[index] is CachedEncounterDanger cachedEncounter ? Player.Get(cachedEncounter.EncounteredHub) : null;

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingGroupEventArgs));

            Label returnLabel = generator.DefineLabel();

            int offset = -3;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(List<DangerStackBase>), nameof(List<DangerStackBase>.RemoveAt)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ParentDangerBase), nameof(ParentDangerBase.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // this.GetDangerType()
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(DangerTypeExtensions), nameof(DangerTypeExtensions.GetDangerType))),

                    // false
                    new(OpCodes.Ldc_I4_0),

                    // GetEncounteredPlayer(this, num);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(ChangingDangerStateChildrenEnding), nameof(GetEncounteredPlayer))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ChangingDangerStateEventArgs ev = new(Player, ParentDangerBase, DangerType, bool, Player, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingDangerStateEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnChangingDangerState(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingDangerState))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingDangerStateEventArgs), nameof(ChangingDangerStateEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}