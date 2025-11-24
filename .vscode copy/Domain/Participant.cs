namespace TimeBasedRacingGame.Domain
{
    /// <summary>
    /// Represents a participant in the racing competition
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// Gets the participant name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the current lap
        /// </summary>
        public int CurrentLap { get; set; }

        /// <summary>
        /// Gets or sets the lap progress (0-100%)
        /// </summary>
        public double LapProgress { get; set; }

        /// <summary>
        /// Gets or sets the current position
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets whether this is the player
        /// </summary>
        public bool IsPlayer { get; }

        /// <summary>
        /// Initializes a new instance of the Participant class
        /// </summary>
        /// <param name="name">Participant name</param>
        /// <param name="isPlayer">Whether this is the player</param>
        public Participant(string name, bool isPlayer = false)
        {
            Name = name;
            IsPlayer = isPlayer;
            CurrentLap = 1;
            LapProgress = 0;
            Position = 1;
        }

        /// <summary>
        /// Gets total progress for position calculation
        /// </summary>
        /// <returns>Total progress value</returns>
        public double GetTotalProgress()
        {
            return (CurrentLap - 1) * 100 + LapProgress;
        }
    }
}

