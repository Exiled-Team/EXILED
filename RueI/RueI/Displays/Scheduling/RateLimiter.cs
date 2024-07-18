namespace RueI.Displays.Scheduling;

/// <summary>
/// Provides a way to ratelimit actions or detect ratelimits.
/// </summary>
/// <remarks>
/// The <see cref="RateLimiter"/> operates using a simple token bucket ratelimiting algorithm.
/// </remarks>
/// <seealso cref="Scheduler"/>
public class RateLimiter
{
    private DateTimeOffset lastConsumed = DateTime.UtcNow;
    private double progress;
    private int tokenLimit;
    private int lastTokenCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateLimiter"/> class.
    /// </summary>
    /// <param name="tokenLimit">The maximum number of tokens and the default number of tokens.</param>
    /// <param name="regenRate">How quickly tokens are regenerated after they have been consumed.</param>
    public RateLimiter(int tokenLimit, TimeSpan regenRate)
    {
        lastTokenCount = tokenLimit;
        RegenRate = regenRate;
    }

    /// <summary>
    /// Gets or sets the regeneration rate for this ratelimiter.
    /// </summary>
    public TimeSpan RegenRate { get; set; }

    /// <summary>
    /// Gets or sets the limit on tokens in this ratelimiter.
    /// </summary>
    public int TokenLimit
    {
        get => tokenLimit;
        set
        {
            tokenLimit = value;
            CalculateNewTokens();
        }
    }

    /// <summary>
    /// Gets the number of tokens available in this ratelimiter.
    /// </summary>
    public int Tokens
    {
        get
        {
            CalculateNewTokens();
            return lastTokenCount;
        }
    }

    /// <summary>
    /// Gets a value indicating whether or not this ratelimiter has a token available.
    /// </summary>
    public bool HasTokens => Tokens > 0;

    /// <summary>
    /// Tries to consume a token from this ratelimiter.
    /// </summary>
    /// <returns>A value indicating whether or not this <see cref="RateLimiter"/> has a token available.</returns>
    public bool TryConsume()
    {
        CalculateNewTokens();
        if (Tokens > 0)
        {
            lastTokenCount--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Consumes a token from this ratelimiter.
    /// </summary>
    public void Consume()
    {
        CalculateNewTokens();
        if (Tokens > 0)
        {
            lastTokenCount--;
        }
    }

    /// <summary>
    /// Calculates the number of new tokens for this ratelimiter.
    /// </summary>
    private void CalculateNewTokens()
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        TimeSpan difference = now - lastConsumed;
        double diff = (difference.TotalMilliseconds / RegenRate.TotalMilliseconds) + progress;

        int newTokens = Math.Min((int)Math.Floor(diff), TokenLimit - Tokens);
        progress = diff - newTokens;
        lastTokenCount += newTokens;
    }
}