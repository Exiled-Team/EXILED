// -----------------------------------------------------------------------
// <copyright file="ParseVisionInformation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1118 // Parameter should not span multiple lines
#pragma warning disable SA1515 // Single-line comment should be preceded by blank line

    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.ParseVisionInformation), new[] { typeof(PlayableScps.VisionInformation) })]
    internal static class ParseVisionInformation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            const int offset = 1;
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var index = newInstructions.FindIndex(ci => ci.opcode == OpCodes.Ret);

            // Quick check if it's the end
            if (index + 1 >= newInstructions.Count)
            {
                Log.Error($"Couldn't path '{typeof(PlayableScps.Scp096).FullName}.{nameof(PlayableScps.Scp096.ParseVisionInformation)}': invalid index - {index}");
                ListPool<CodeInstruction>.Shared.Return(newInstructions);
                yield break;
            }

            index += offset;

            var continueLabel = generator.DefineLabel();
            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                // if (PlayableScpsController.hub.characterClassManager.CurClass == RoleType.Tutorial && !Exiled.Events.Events.Instance.Config.CanTutorialTriggerScp096)
                //      return;
                // START
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayableScpsController), nameof(PlayableScpsController.hub))),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CharacterClassManager), nameof(CharacterClassManager.CurClass))),
                new CodeInstruction(OpCodes.Ldc_I4_S, (sbyte)RoleType.Tutorial),
                new CodeInstruction(OpCodes.Bne_Un_S, continueLabel),

                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Plugin<Exiled.Events.Config>), nameof(Plugin<Exiled.Events.Config>.Config))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Exiled.Events.Config), nameof(Exiled.Events.Config.CanTutorialTriggerScp096))),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
                new CodeInstruction(OpCodes.Ret),
                // END
                // if (API.Features.Scp096.TurnedPlayers.Contsins(Player.Get(PlayableScpsController.Hub)))
                //      return;
                // START
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PlayableScpsController), nameof(PlayableScpsController.hub))),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Exiled.API.Features.Player), nameof(Exiled.API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Call, AccessTools.PropertyGetter(typeof(API.Features.Scp096), nameof(API.Features.Scp096.TurnedPlayers))),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(HashSet<Player>), nameof(HashSet<Player>.Contains))),
                new CodeInstruction(OpCodes.Brfalse_S, continueLabel),
                new CodeInstruction(OpCodes.Ret),
                // END
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
