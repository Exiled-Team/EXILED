namespace Exiled.API.Enums
{
    /// <summary>
    /// Identifies current status of Decontamination phase.
    /// </summary>
    public enum DecontaminationPhase
    {
        /// <summary>
        /// Basic phase.
        /// </summary>
        None,

        /// <summary>
        /// Phase when everybody hears announcement.
        /// </summary>
        GloballyAudible,

        /// <summary>
        /// Phase when checkpoints are opened.
        /// </summary>
        OpenCheckpoints,

        /// <summary>
        /// Final phase.
        /// </summary>
        Final,
    }
}