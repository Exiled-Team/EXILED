namespace Exiled.Events.EventArgs
{
    using Respawning;

    /// <summary>
    /// Contains all information before adding a new unit name.
    /// </summary>
    public class AddingUnitNameEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddingUnitNameEventArgs"/> class.
        /// </summary>
        /// <param name="unitName">The generated unit name.</param>
        /// <param name="team">The <see cref="SpawnableTeamType"/> team.</param>
        /// <param name="isAllowed">The value indicating whether or not the unit name will be added.</param>
        public AddingUnitNameEventArgs(string unitName, SpawnableTeamType team, bool isAllowed = true)
        {
            UnitName = unitName;
            Team = team;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets a value indicating what the spawnable team is.
        /// </summary>
        public SpawnableTeamType Team { get; }

        /// <summary>
        /// Gets or sets the next unit name.
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the unit name will be added.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
