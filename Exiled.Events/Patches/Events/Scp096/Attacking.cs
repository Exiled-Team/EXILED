// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp096;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp096;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Patches <see cref="Scp096HitHandler.ProcessHits" />.
    /// Adds the <see cref="Handlers.Scp096.Attacking" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp096), nameof(Handlers.Scp096.Attacking))]
    [HarmonyPatch(typeof(Scp096HitHandler), nameof(Scp096HitHandler.ProcessHits))]
    internal static class Attacking
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label retLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(AttackingEventArgs));

            const int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Scp096TargetsTracker), nameof(Scp096TargetsTracker.HasTarget), new[] { typeof(ReferenceHub) }))) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Scp096HitHandler::_scpRole::_lastOwner
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp096HitHandler), nameof(Scp096HitHandler._scpRole))),
                new(OpCodes.Ldfld, Field(typeof(Scp096Role), nameof(Scp096Role._lastOwner))),

                // Player::Get(Scp096HitHandler::_scpRole::_lastOwner)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // targetHub
                new(OpCodes.Ldloc_S, 7),

                // Player::Get(targetHub)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Scp096HitHandler::_humanTargetDamage
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp096HitHandler), nameof(Scp096HitHandler._humanTargetDamage))),

                // Scp096HitHandler::_humanNontargetDamage
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp096HitHandler), nameof(Scp096HitHandler._humanNontargetDamage))),

                // true
                new(OpCodes.Ldc_I4_1),

                // AttackingEventArgs args = new(player, target, _humanTargetDamage, _humanNontargetDamage)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Scp096::OnAttacking(args)
                new(OpCodes.Call, Method(typeof(Handlers.Scp096), nameof(Handlers.Scp096.OnAttacking))),

                // if (!args.IsAllowed) return
                new(OpCodes.Callvirt, PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, retLabel),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Attacking), nameof(SetFieldValues))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void SetFieldValues(Scp096HitHandler hitHandler, AttackingEventArgs args)
        {
            FieldInfo humanDamageField = typeof(Scp096HitHandler).GetField(nameof(Scp096HitHandler._humanTargetDamage), BindingFlags.Instance | BindingFlags.NonPublic);
            humanDamageField?.SetValue(hitHandler, args.HumanDamage);

            FieldInfo nonTargetField = typeof(Scp096HitHandler).GetField(nameof(Scp096HitHandler._humanNontargetDamage), BindingFlags.Instance | BindingFlags.NonPublic);
            nonTargetField?.SetValue(hitHandler, args.NonTargetDamage);
        }
    }
}