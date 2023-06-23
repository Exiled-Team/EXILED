// -----------------------------------------------------------------------
// <copyright file="Lunge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp939
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs.Scp939;
    using HarmonyLib;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp939;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
    /// <summary>
    ///     Patches <see cref="Scp939LungeAbility.ClientSendHit(HumanRole)"/>
    ///     to add the <see cref="Handlers.Scp939.Lunged"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.ClientSendHit))]
    internal static class Lunge
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // this.Owner
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Scp939LungeAbility), nameof(Scp939LungeAbility.Owner))),

                // true
                new(OpCodes.Ldc_I4_1),

                // LungingEventArgs ev = new (...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LungingEventArgs))[0]),

                // Scp939.OnLunging(ev)
                new(OpCodes.Call, Method(typeof(Scp939), nameof(Scp939.OnLunging))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}