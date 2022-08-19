using System.Linq;

using Exiled.API.Features;

using Mirror.LiteNetLib4Mirror;

namespace Exiled.Metrics.Verification;

/// <summary>
/// Handles the client-side part of server verification (making sure that a server is who they say they are).
/// </summary>
public class Verifier
{
    private static VerificationData? data;

    private const string IDPrefix = "EXILED-ID-";
    private const string TimestampPrefix = "EXILED-TIMESTAMP-";
    private const string HmacPrefix = "EXILED-HMAC-";

    /// <summary>
    /// Refreshes the cached verification data.
    /// </summary>
    internal static void RefreshVerificationData()
    {
        // We need the server to be able to contact the server list.
        if (string.IsNullOrEmpty(ServerConsole.Password))
            return;

        // To get the verification data, we need to send a request to the Northwood authenticator API.
        string response = HttpQuery.Post(CentralServer.MasterUrl + "v4/authenticator.php", HttpQuery.ToPostArgs(new []
        {
            $"ip={ServerConsole.Ip}",
            "players=" + ServerConsole.PlayersAmount + "/" + CustomNetworkManager.slots,
            "port=" + (ServerConsole.PortOverride == 0 ? LiteNetLib4MirrorTransport.Singleton.port : ServerConsole.PortOverride),
            "version=2",
            "passcode=" + ServerConsole.Password,
            // This is a special flag to give us verification data.
            "extAuth=EXILED"
        }));
        AuthenticatorResponse authenticatorResponse = JsonSerialize.FromJson<AuthenticatorResponse>(response);

        // Now, we need to perform a few checks.
        // If we received an error, we should print it.
        // If the request was unsuccessful or the server isn't verified, we can quit.

        if (!string.IsNullOrEmpty(authenticatorResponse.error))
        {
            Log.Error($"Received an error while requesting verification data: {authenticatorResponse.error}");
            return;
        }

        if (!authenticatorResponse.success || !authenticatorResponse.verified)
            return;

        // We should also verify that we received the verification data.
        string id = authenticatorResponse.actions.FirstOrDefault(action => action.StartsWith(IDPrefix))?.Substring(IDPrefix.Length);
        string timestamp = authenticatorResponse.actions.FirstOrDefault(action => action.StartsWith(TimestampPrefix))?.Substring(TimestampPrefix.Length);
        string hmac = authenticatorResponse.actions.FirstOrDefault(action => action.StartsWith(HmacPrefix))?.Substring(HmacPrefix.Length);

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(hmac))
            return;

        // Finally, cache the verification data.
        data = new VerificationData(id, timestamp, hmac);
    }

    /// <summary>
    /// Retrieves (cached) verification data from the Northwood API.
    /// </summary>
    internal static VerificationData? GetVerificationData()
    {
        return data;
    }
}
