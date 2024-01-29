// -----------------------------------------------------------------------
// <copyright file="AnnouncingNtfEntrance.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using System.Text.RegularExpressions;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Map;

    using Handlers;

    using HarmonyLib;

    using Respawning.NamingRules;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="NineTailedFoxNamingRule.PlayEntranceAnnouncement(string)" />.
    ///     Adds the <see cref="Map.AnnouncingNtfEntrance" /> event.
    /// </summary>
    [EventPatch(typeof(Map), nameof(Map.AnnouncingNtfEntrance))]
    [HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.PlayEntranceAnnouncement))]
    internal static class AnnouncingNtfEntrance
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AnnouncingNtfEntranceEventArgs));
            LocalBuilder unitInformation = generator.DeclareLocal(typeof(string[]));

            Label ret = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            // int scpsLeft = ReferenceHub.GetAllHubs().Values.Count(x => x.characterClassManager.CurRole.team == Team.SCP && x.characterClassManager.CurClass != RoleTypeId.Scp0492);
            // string unitNameClear = Regex.Replace(unitName, "<[^>]*?>", string.Empty);
            // string[] unitInformation = unitNameClear.Split('-');
            //
            // AnnouncingNtfEntranceEventArgs ev = new(scpsLeft, unitInformation[0], int.Parse(unitInformation[1]));
            // Map.OnAnnouncingNtfEntrance(ev);
            //
            // if (!ev.IsAllowed)
            //     return;
            //
            // unitName = $"{ev.UnitName}-{ev.UnitNumber};
            // cassieUnitName = this.GetCassieUnitName(unitName);
            // scpsLeft = ev.ScpsLeft;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // int scpsLeft = ReferenceHub.GetAllHubs().Values.Count(x => x.characterClassManager.CurRole.team == Team.SCP && x.characterClassManager.CurClass != RoleTypeId.Scp0492);
                    new CodeInstruction(OpCodes.Ldloc_1),

                    // string[] unitInformation = unitNameClear.Split('-');
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldstr, "<[^>]*?>"),
                    new(OpCodes.Ldsfld, Field(typeof(string), nameof(string.Empty))),
                    new(OpCodes.Call, Method(typeof(Regex), nameof(Regex.Replace), new System.Type[] { typeof(string), typeof(string), typeof(string) })),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newarr, typeof(char)),
                    new(OpCodes.Dup),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldc_I4_S, 45),
                    new(OpCodes.Stelem_I2),
                    new(OpCodes.Call, Method(typeof(string), nameof(string.Split), new[] { typeof(char[]) })),

                    // AnnouncingNtfEntranceEventArgs ev = new(scpsLeft, unitInformation[0], int.Parse(unitInformation[1]));
                    //
                    // Map.OnAnnouncingNtfEntrance(ev);
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, unitInformation.LocalIndex),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ldelem_Ref),
                    new(OpCodes.Ldloc_S, unitInformation.LocalIndex),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ldelem_Ref),
                    new(OpCodes.Call, Method(typeof(int), nameof(int.Parse), new[] { typeof(string) })),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AnnouncingNtfEntranceEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Map), nameof(Map.OnAnnouncingNtfEntrance))),

                    // if (!ev.IsAllowed)
                    //     return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // unitName = $"{ev.UnitName}-{ev.UnitNumber};
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldstr, "{0}-{1}"),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.UnitName))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.UnitNumber))),
                    new(OpCodes.Box, typeof(int)),
                    new(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object), typeof(object) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Starg_S, 1),

                    // cassieUnitName = this.GetCassieUnitName(unitName);
                    new(OpCodes.Callvirt, Method(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GetCassieUnitName))),
                    new(OpCodes.Stloc_0),

                    // scpsLeft = ev.ScpsLeft;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AnnouncingNtfEntranceEventArgs), nameof(AnnouncingNtfEntranceEventArgs.ScpsLeft))),
                    new(OpCodes.Stloc_1),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}