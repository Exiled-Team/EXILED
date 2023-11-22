using System.Collections.Generic;
using System.Reflection.Emit;
using Exiled.Events.EventArgs.Scp3114;
using Utils.Networking;

namespace Exiled.Events.Patches.Events.Scp3114
{
    namespace Exiled.Events.Patches.Events.Scp3114
    {
        using Exiled.Events.EventArgs.Scp3114;
        using Exiled.Events.Handlers;
        using Exiled.API.Features;

        using HarmonyLib;

        using static HarmonyLib.AccessTools;

        using PlayerRoles.PlayableScps.Scp3114;

    [HarmonyPatch(typeof(Scp3114Strangle), nameof(Scp3114Strangle.ProcessAttackRequest))]
    internal class Strangling
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(ReferenceHubReaderWriter), nameof(ReferenceHubReaderWriter.TryReadReferenceHub)))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player target = Player.Get(ReferenceHub)
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Player player = Player.Get(this.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp3114Strangle), nameof(Scp3114Strangle.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // true
                new(OpCodes.Ldc_I4_1),

                // StranglingEventArgs ev = new(target, player, true)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StranglingEventArgs))[0]),
                new(OpCodes.Dup),

                // Handlers.Scp3114.StranglingEventArgs(ev)
                new(OpCodes.Call, Method(typeof(Handlers), nameof(Handlers.OnStrangling))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(StranglingEventArgs), nameof(StranglingEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),
                new CodeInstruction(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}

