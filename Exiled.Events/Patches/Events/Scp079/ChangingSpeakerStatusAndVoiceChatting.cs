// -----------------------------------------------------------------------
// <copyright file="ChangingSpeakerStatusAndVoiceChatting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Scp079;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles;
    using PlayerRoles.Voice;

    using static HarmonyLib.AccessTools;

    using Player = API.Features.Player;

    /// <summary>
    ///     Patches Scp079VoiceModule.ServerIsSending />.
    ///     Adds the <see cref="Scp079.ChangingSpeakerStatus" /> and the <see cref="Handlers.Player.VoiceChatting"/> events.
    /// </summary>
    [HarmonyPatch(typeof(VoiceModuleBase), nameof(VoiceModuleBase.ServerIsSending), MethodType.Setter)]
    internal static class ChangingSpeakerStatusAndVoiceChatting
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            const int index = 0;

            newInstructions[index].WithLabels(continueLabel);

            // if (base.CurrentChannel != VoiceChatChannel.Proximity)
            //    goto continueLabel;
            //
            // ChangingSpeakerStatusEventArgs ev = new(Player.Get(base.Owner), value);
            //
            // Scp079.OnStartingSpeaker(ev);
            //
            // value = ev.IsAllowed
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (this.Role.RoleTypeId != RoleTypeId.Scp079)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.Role))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
                    new(OpCodes.Ldc_I4_7),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // if (this.CurrentChannel != VoiceChatChannel.Proximity)
                    //    goto continueLabel;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(VoiceModuleBase), nameof(VoiceModuleBase.CurrentChannel))),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(VoiceModuleBase), nameof(VoiceModuleBase._owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // value
                    new(OpCodes.Ldarg_1),

                    // ChangingSpeakerStatusEventArgs ev = new(Player, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingSpeakerStatusEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Scp079.OnChangingSpeakerStatus(ev);
                    new(OpCodes.Call, Method(typeof(Scp079), nameof(Scp079.OnChangingSpeakerStatus))),

                    // value = ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingSpeakerStatusEventArgs), nameof(ChangingSpeakerStatusEventArgs.IsAllowed))),
                    new(OpCodes.Starg_S, 1),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}