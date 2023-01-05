using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Exiled.Events.EventArgs.Player;
using HarmonyLib;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.Voice;
using PluginAPI.Core;
using UnityEngine;
using VoiceChat;
using VoiceChat.Networking;

namespace Exiled.Events.Patches.Events.Player
{
    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(VoiceTransceiver), nameof(VoiceTransceiver.ServerReceiveMessage))]
    public class ReceivingAudio
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = -1;

            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(IVoiceRole), nameof(IVoiceRole.VoiceModule)))) + offset;

            // Immediately return
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // IVoiceRole, NetworkConnection, VoiceMessage
                    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldarg_1),
                    // Returns DoctorSenseEventArgs
                    new(OpCodes.Call, Method(typeof(ReceivingAudio), nameof(ReceivingAudioMessage))),
                    // If !ev.IsAllowed, return
                    new(OpCodes.Br, returnLabel),

                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static void ReceivingAudioMessage(IVoiceRole voiceRole, NetworkConnection connection, VoiceMessage msg)
        {
            API.Features.Player currentPlayer = API.Features.Player.Get(msg.Speaker);
            ReceivingAudioEventArgs playerAudioEvent = new ReceivingAudioEventArgs(currentPlayer, voiceRole, connection, msg);
            Handlers.Player.OnReceivingAudio(playerAudioEvent);

            if (!playerAudioEvent.IsAllowed)
            {
                return;
            }

            if (playerAudioEvent.CheckRateLimit)
            {
                if (!voiceRole.VoiceModule.CheckRateLimit())
                {
                    return;
                }
            }

            VcMuteFlags flags = VoiceChatMutes.GetFlags(msg.Speaker);
            if (flags is VcMuteFlags.GlobalRegular or VcMuteFlags.LocalRegular)
            {
                return;
            }

            VoiceChatChannel voiceChatChannel = playerAudioEvent.Channel;
            if (!playerAudioEvent.BypassAudioValidateSend)
            {
                voiceChatChannel = voiceRole.VoiceModule.ValidateSend(playerAudioEvent.Channel);
            }

            if (voiceChatChannel == VoiceChatChannel.None)
            {
                return;
            }

            voiceRole.VoiceModule.CurrentChannel = voiceChatChannel;

            if (playerAudioEvent.CustomChat)
            {
                foreach (ReferenceHub customConnectionPlayer in playerAudioEvent.CustomConnectionPlayers)
                {
                    Log.Info($"Sending custom chat from {msg.Speaker.name} to {customConnectionPlayer.name} in channel {voiceChatChannel}");
                    handleSendingAudio(customConnectionPlayer, playerAudioEvent, msg, voiceChatChannel);
                }
            }
            else
            {
                foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
                {
                    // Should not self-send, and should not send to server host.
                    if (referenceHub == msg.Speaker || referenceHub == API.Features.Server.Host.ReferenceHub)
                    {
                        continue;
                    }

                    handleSendingAudio(referenceHub, playerAudioEvent, msg, voiceChatChannel);
                }
            }
        }

        private static void handleSendingAudio(ReferenceHub referenceHub, ReceivingAudioEventArgs playerAudioEvent, VoiceMessage msg, VoiceChatChannel voiceChatChannel)
        {
            IVoiceRole voiceRole2;
            if ((voiceRole2 = referenceHub.roleManager.CurrentRole as IVoiceRole) != null)
            {
                VoiceChatChannel voiceChannelOfNewClient = playerAudioEvent.Channel;
                if (!playerAudioEvent.BypassAudioValidateReceive)
                {
                    voiceChannelOfNewClient = voiceRole2.VoiceModule.ValidateReceive(msg.Speaker, voiceChatChannel);
                }

                if (voiceChannelOfNewClient == VoiceChatChannel.None || playerAudioEvent.PlayersToNotReceiveAudio.Contains(referenceHub))
                    return;

                msg.Channel = voiceChannelOfNewClient;
                referenceHub.connectionToClient.Send(msg);

            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}