using System;
using System.Collections.Generic;
using System.Linq;
using TimeBasedRacingGame.Domain;
using TimeBasedRacingGame.Exceptions;

namespace TimeBasedRacingGame.Core
{
    /// <summary>
    /// Represents different player actions during the race
    /// </summary>
    public enum PlayerAction
    {
        /// <summary>Increase speed and consume more fuel</summary>
        SpeedUp,
        /// <summary>Maintain current speed with normal fuel consumption</summary>
        MaintainSpeed,
        /// <summary>Stop to refuel and reset speed to zero</summary>
        PitStop
    }

    /// <summary>
    /// Core game engine that manages the racing simulation
    /// </summary>
    public class GameEngine
    {
        /// <summary>
        /// Gets the available vehicles for selection
        /// </summary>
        public List<Vehicle> AvailableVehicles { get; }

        /// <summary>
        /// Gets or sets the selected vehicle
        /// </summary>
        public Vehicle SelectedVehicle { get; set; }

        /// <summary>
        /// Gets the racing circuit
        /// </summary>
        public Circuit Circuit { get; }

        /// <summary>
        /// Gets or sets the remaining time in seconds
        /// </summary>
        public double TimeRemaining { get; set; }

        /// <summary>
        /// Gets the maximum game time
        /// </summary>
        public double MaxTime { get; }

        /// <summary>
        /// Gets whether the game is active
        /// </summary>
        public bool IsGameActive { get; private set; }

        /// <summary>
        /// Gets the game result message
        /// </summary>
        public string GameResult { get; private set; }

        /// <summary>
        /// Gets the current game state
        /// </summary>
        public GameState GameState { get; private set; }

        /// <summary>
        /// Gets the list of all participants
        /// </summary>
        public List<Participant> Participants { get; private set; }

        /// <summary>
        /// Initializes a new instance of the GameEngine class
        /// </summary>
        /// <param name="maxTime">Maximum game time in seconds</param>
        public GameEngine(double maxTime = 300)
        {
            MaxTime = maxTime;
            TimeRemaining = maxTime;
            Circuit = new Circuit();
            IsGameActive = false;
            GameResult = "";
            GameState = new GameState();

            // Initialize participants
            Participants = new List<Participant>
            {
                new Participant("You", true),
                new Participant("Speed Racer"),
                new Participant("Lightning McQueen"),
                new Participant("Turbo Tom")
            };

            AvailableVehicles = new List<Vehicle>
            {
                new Vehicle(VehicleType.RacingVehicle, "Lightning McQueen", 160, 10.0, 65),
                new Vehicle(VehicleType.EcoVehicle, "Mater", 90, 5.0, 90),
                new Vehicle(VehicleType.SportsVehicle, "Sally Carrera", 140, 8.5, 60),
                new Vehicle(VehicleType.SportsVehicle, "Doc Hudson", 130, 7.5, 70),
                new Vehicle(VehicleType.EcoVehicle, "Ramone", 110, 6.0, 75)
            };
        }

        /// <summary>
        /// Starts the game with the selected vehicle
        /// </summary>
        /// <exception cref="GameException">Thrown when no vehicle is selected</exception>
        public void StartGame()
        {
            if (SelectedVehicle == null)
                throw new GameException("No vehicle selected for game start");

            IsGameActive = true;
            GameResult = "";
            TimeRemaining = MaxTime;
            Circuit.CurrentLap = 1;
            Circuit.LapProgress = 0;
            SelectedVehicle.CurrentSpeed = 0;
            SelectedVehicle.CurrentFuel = SelectedVehicle.MaxFuel;
            
            // Update game state
            GameState.IsActive = true;
            GameState.CurrentLap = 1;
            GameState.LapProgress = 0;
            GameState.TimeRemaining = MaxTime;
            GameState.CurrentFuel = SelectedVehicle.CurrentFuel;
            GameState.CurrentSpeed = 0;
            GameState.LogAction($"Game started with {SelectedVehicle.Name}");
        }

        /// <summary>
        /// Executes a player action and updates game state
        /// </summary>
        /// <param name="action">The action to perform</param>
        /// <exception cref="GameException">Thrown when game is not active or action is invalid</exception>
        public void ExecuteAction(PlayerAction action)
        {
            if (!IsGameActive)
                throw new GameException("Cannot execute action - game is not active");

            try
            {
                switch (action)
                {
                    case PlayerAction.SpeedUp:
                        SpeedUp();
                        GameState.LogAction("Speed increased");
                        break;
                    case PlayerAction.MaintainSpeed:
                        MaintainSpeed();
                        GameState.LogAction("Speed maintained");
                        break;
                    case PlayerAction.PitStop:
                        PitStop();
                        GameState.LogAction("Pit stop completed");
                        break;
                }

                UpdateGameState();
            }
            catch (InvalidOperationException ex)
            {
                GameState.LogAction($"Action failed: {ex.Message}");
                EndGame("Out of fuel!");
            }
        }

        /// <summary>
        /// Increases vehicle speed and consumes fuel
        /// </summary>
        private void SpeedUp()
        {
            if (SelectedVehicle.CurrentSpeed < SelectedVehicle.MaxSpeed)
            {
                SelectedVehicle.CurrentSpeed = Math.Min(SelectedVehicle.MaxSpeed, SelectedVehicle.CurrentSpeed + 20);
            }
            SelectedVehicle.ConsumeFuel(1.5);
            TimeRemaining -= 5;
        }

        /// <summary>
        /// Maintains current speed with normal fuel consumption
        /// </summary>
        private void MaintainSpeed()
        {
            SelectedVehicle.ConsumeFuel(1.0);
            TimeRemaining -= 3;
        }

        /// <summary>
        /// Performs a pit stop to refuel
        /// </summary>
        /// <exception cref="GameException">Thrown when fuel tank is already full</exception>
        private void PitStop()
        {
            if (SelectedVehicle.CurrentFuel >= SelectedVehicle.MaxFuel * 0.95) // 95% full threshold
                throw new GameException("Cannot pit stop - fuel tank is already full");
                
            SelectedVehicle.Refuel();
            SelectedVehicle.CurrentSpeed = 0;
            TimeRemaining -= 15;
        }

        /// <summary>
        /// Updates the game state after an action
        /// </summary>
        private void UpdateGameState()
        {
            if (SelectedVehicle.CurrentSpeed > 0)
            {
                bool lapCompleted = Circuit.AdvanceProgress(SelectedVehicle.CurrentSpeed);
                if (lapCompleted && !Circuit.IsRaceCompleted())
                {
                    GameState.LogAction($"Lap {Circuit.CurrentLap - 1} completed!");
                }
            }

            // Update game state
            GameState.CurrentLap = Circuit.CurrentLap;
            GameState.LapProgress = Circuit.LapProgress;
            GameState.TimeRemaining = TimeRemaining;
            GameState.CurrentFuel = SelectedVehicle.CurrentFuel;
            GameState.CurrentSpeed = SelectedVehicle.CurrentSpeed;

            // Update participant positions
            UpdatePositions();

            CheckGameConditions();
        }

        /// <summary>
        /// Checks for game end conditions
        /// </summary>
        private void CheckGameConditions()
        {
            if (Circuit.IsRaceCompleted())
            {
                EndGame("Race completed! You won!");
            }
            else if (TimeRemaining <= 0)
            {
                EndGame("Time's up! Race over.");
            }
            else if (SelectedVehicle.CurrentFuel <= 0)
            {
                EndGame("Out of fuel! Race over.");
            }
        }

        /// <summary>
        /// Ends the game with a result message
        /// </summary>
        /// <param name="result">The game result message</param>
        private void EndGame(string result)
        {
            IsGameActive = false;
            GameResult = result;
        }

        /// <summary>
        /// Gets the time remaining percentage
        /// </summary>
        /// <returns>Time percentage (0-100)</returns>
        public double GetTimePercentage()
        {
            return (TimeRemaining / MaxTime) * 100;
        }

        /// <summary>
        /// Updates participant positions based on progress
        /// </summary>
        private void UpdatePositions()
        {
            var playerParticipant = Participants.First(p => p.IsPlayer);
            playerParticipant.CurrentLap = Circuit.CurrentLap;
            playerParticipant.LapProgress = Circuit.LapProgress;

            // Simulate AI progress
            var random = new Random();
            foreach (var participant in Participants.Where(p => !p.IsPlayer))
            {
                if (IsGameActive)
                {
                    participant.LapProgress += random.Next(8, 15);
                    if (participant.LapProgress >= 100)
                    {
                        participant.LapProgress = 0;
                        participant.CurrentLap++;
                    }
                }
            }

            // Calculate positions
            var sortedParticipants = Participants.OrderByDescending(p => p.GetTotalProgress()).ToList();
            for (int i = 0; i < sortedParticipants.Count; i++)
            {
                sortedParticipants[i].Position = i + 1;
            }
        }

        /// <summary>
        /// Gets the player's current position
        /// </summary>
        /// <returns>Player position (1st, 2nd, etc.)</returns>
        public string GetPlayerPosition()
        {
            var playerParticipant = Participants.First(p => p.IsPlayer);
            return playerParticipant.Position switch
            {
                1 => "1st",
                2 => "2nd",
                3 => "3rd",
                _ => $"{playerParticipant.Position}th"
            };
        }
    }
}

