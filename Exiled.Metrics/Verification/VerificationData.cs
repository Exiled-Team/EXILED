namespace Exiled.Metrics.Verification;

/// <summary>
/// Data used to verify a server's identity.
/// </summary>
internal readonly struct VerificationData
{
    /// <summary>
    /// A unique and anonymous ID that identifies this server.
    /// </summary>
    public readonly string ID;

    /// <summary>
    /// A UNIX timestamp describing when this was issued.
    /// </summary>
    public readonly string Timestamp;

    /// <summary>
    /// A HMAC used for validating the authenticity of this.
    /// </summary>
    public readonly string Hmac;

    public VerificationData(string id, string timestamp, string hmac)
    {
        ID = id;
        Timestamp = timestamp;
        Hmac = hmac;
    }
}
