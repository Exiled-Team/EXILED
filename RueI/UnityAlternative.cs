namespace RueI;

using Hints;
using MEC;

using RueI.Extensions;
using HarmonyLib;
using System.Diagnostics;
using System;

/// <summary>
/// Defines the base class for a provider of methods that may or may not use Unity.
/// </summary>
/// <remarks>
/// The <see cref="UnityAlternative"/> class is primarily intended for internal use within RueI. This enables certain features
/// to work even outside of SCP:SL and Unity, which is utilized primarily for unit-testing.
/// </remarks>
public abstract class UnityAlternative
{
    /// <summary>
    /// Represents a generalized handler for an async operation.
    /// </summary>
    public interface IAsyncOperation : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether or not this operation is currently running.
        /// </summary>
        public bool IsRunning { get; }

        /// <summary>
        /// Cancels this operation.
        /// </summary>
        public void Cancel();
    }

    /// <summary>
    /// Gets the current <see cref="UnityAlternative"/> of the application.
    /// </summary>
    public static UnityAlternative Provider { get; } = GetProvider();

    /// <summary>
    /// Logs a message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="color">The color of the message.</param>
    public abstract void Log(string message, ConsoleColor color = ConsoleColor.Yellow);

    /// <summary>
    /// Logs a warning message to the console.
    /// </summary>
    /// <param name="message">The warn message to log.</param>
    public abstract void LogWarn(string message);

    /// <summary>
    /// Logs a debug message to the console.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    [Conditional("DEBUG")]
    public abstract void LogDebug(string message);

    /// <summary>
    /// Loads all patches.
    /// </summary>
    /// <param name="harmony">The <see cref="Harmony"/> instance to use.</param>
    public abstract void PatchAll(Harmony harmony);

    /// <summary>
    /// Performs an async operation.
    /// </summary>
    /// <param name="span">How long until the action should be ran.</param>
    /// <param name="action">The action to run when finished.</param>
    /// <returns>A <see cref="IAsyncOperation"/> to use.</returns>
    public abstract IAsyncOperation PerformAsync(TimeSpan span, Action action);

    /// <summary>
    /// Shows a hint for a <see cref="ReferenceHub"/>.
    /// </summary>
    /// <param name="hub">The <see cref="ReferenceHub"/> to use.</param>
    /// <param name="message">The message to show.</param>
    internal abstract void ShowHint(ReferenceHub hub, string message);

    private static UnityAlternative GetProvider()
    {
        try
        {
            _ = UnityEngine.Object.FindObjectOfType<ReferenceHub>(); // errors if not in unity
            return new UnityProvider();
        }
        catch(Exception)
        {
            return new NonUnityProvider();
        }
    }
}

/// <summary>
/// Provides non-Unity alternatives for the <see cref="UnityProvider"/> of the application.
/// </summary>
public class NonUnityProvider : UnityAlternative
{
    /// <inheritdoc/>
    public override void Log(string message, ConsoleColor color = ConsoleColor.Yellow) => Console.WriteLine(message);

    /// <inheritdoc/>
    public override void LogWarn(string message) => Console.WriteLine($"WARN: {message}");

    /// <inheritdoc/>
    public override void LogDebug(string message) => Console.WriteLine($"DEBUG: {message}");

    /// <inheritdoc/>
    public override void PatchAll(Harmony harmony) => Console.WriteLine("Faux loading patches");

    /// <inheritdoc/>
    public override IAsyncOperation PerformAsync(TimeSpan span, Action action) => new TaskAsyncOperation(span, action);

    /// <inheritdoc/>
    internal override void ShowHint(ReferenceHub hub, string message) => Log(message);

    /// <summary>
    /// Represents an async operation using a <see cref="Task"/>.
    /// </summary>
    public class TaskAsyncOperation : IAsyncOperation
    {
        private readonly Task task;
        private readonly CancellationTokenSource source;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskAsyncOperation"/> class, and then immediately runs.
        /// </summary>
        /// <param name="span">How long until the action should be ran.</param>
        /// <param name="action">The action to run when finished.</param>
        public TaskAsyncOperation(TimeSpan span, Action action)
        {
            CancellationTokenSource newSource = new();
            source = newSource;
            CancellationToken token = newSource.Token;
            task = Task.Run(
                async () =>
            {
                await Task.Delay(span, token);
                token.ThrowIfCancellationRequested();
                action();
                Dispose();
            }, token);
        }

        /// <inheritdoc/>
        public bool IsRunning => !task.IsCompleted;

        /// <inheritdoc/>
        public void Cancel()
        {
            source.Cancel();
            source.Dispose();
        }

        /// <summary>
        /// Disposes this async operation.
        /// </summary>
        public void Dispose() => source.Dispose();
    }
}

/// <summary>
/// Provides Unity methods for the application.
/// </summary>
public class UnityProvider : UnityAlternative
{
    /// <inheritdoc/>
    public override void Log(string message, ConsoleColor color = ConsoleColor.Yellow) => ServerConsole.AddLog(message, color);

    /// <inheritdoc/>
    public override void LogWarn(string message) => ServerConsole.AddLog(message, ConsoleColor.Red);

    /// <inheritdoc/>
    public override void LogDebug(string message) => ServerConsole.AddLog(message, ConsoleColor.Magenta);

    /// <inheritdoc/>
    public override void PatchAll(Harmony harmony) => harmony.PatchAll(typeof(RueIMain).Assembly);

    /// <inheritdoc/>
    public override IAsyncOperation PerformAsync(TimeSpan span, Action action) => new MECAsyncOperation(span, action);

    /// <inheritdoc/>
    internal override void ShowHint(ReferenceHub hub, string message) => hub.connectionToClient.Send(new HintMessage(new TextHint(message, new HintParameter[] { new StringHintParameter(message) }, null, 99999)));

    /// <summary>
    /// Represents an async operation using a <see cref="CoroutineHandle"/>.
    /// </summary>
    public class MECAsyncOperation : IAsyncOperation
    {
        private readonly CoroutineHandle handle;

        /// <summary>
        /// Initializes a new instance of the <see cref="MECAsyncOperation"/> class, and then immediately runs.
        /// </summary>
        /// <param name="span">How long until the action should be ran.</param>
        /// <param name="action">The action to run when finished.</param>
        public MECAsyncOperation(TimeSpan span, Action action)
        {
            float time = ((float)span.TotalSeconds).Max(0f);

            handle = Timing.CallDelayed(time, action);
        }

        /// <inheritdoc/>
        public bool IsRunning => handle.IsRunning;

        /// <inheritdoc/>
        public void Cancel()
        {
            Timing.KillCoroutines(handle);
        }

        /// <summary>
        /// Disposes this MEC operation.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
