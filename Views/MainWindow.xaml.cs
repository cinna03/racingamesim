using System;
using System.Windows;
using TimeBasedRacingGame.Core;
using TimeBasedRacingGame.Domain;
using TimeBasedRacingGame.Exceptions;
using TimeBasedRacingGame.Services;

namespace TimeBasedRacingGame.Views
{
    public partial class MainWindow : Window
    {
        private GameEngine _gameEngine;
        private TimerService _timerService;
        private UIService _uiService;

        /// <summary>
        /// Initializes a new instance of the MainWindow class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            _gameEngine = new GameEngine();
            _timerService = new TimerService(1.0);
            _uiService = new UIService();

            CarSelectionComboBox.ItemsSource = _gameEngine.AvailableVehicles;
            CarSelectionComboBox.DisplayMemberPath = "Name";
            
            _timerService.SetTickCallback(GameTimer_Tick);
            UpdateUI();
        }

        private void CarSelectionComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (CarSelectionComboBox.SelectedItem is Vehicle selectedVehicle)
            {
                _gameEngine.SelectedVehicle = selectedVehicle;
                StartRaceButton.IsEnabled = true;
            }
        }

        private void CarSelectionBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CarSelectionComboBox.IsDropDownOpen = true;
        }

        private void StartRaceButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _gameEngine.StartGame();
                StartRaceButton.IsEnabled = false;
                CarSelectionComboBox.IsEnabled = false;
                EnableActionButtons(true);
                _timerService.Start();
                UpdateUI();
            }
            catch (GameException ex)
            {
                MessageBox.Show(ex.Message, "Game Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SpeedUpButton_Click(object sender, RoutedEventArgs e)
        {
            ExecutePlayerAction(PlayerAction.SpeedUp);
        }

        private void MaintainSpeedButton_Click(object sender, RoutedEventArgs e)
        {
            ExecutePlayerAction(PlayerAction.MaintainSpeed);
        }

        private void PitStopButton_Click(object sender, RoutedEventArgs e)
        {
            ExecutePlayerAction(PlayerAction.PitStop);
        }

        private void ExecutePlayerAction(PlayerAction action)
        {
            try
            {
                _gameEngine.ExecuteAction(action);
                UpdateUI();
                if (!_gameEngine.IsGameActive)
                {
                    EndGame();
                }
            }
            catch (GameException ex)
            {
                MessageBox.Show(ex.Message, "Game Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void GameTimer_Tick()
        {
            if (_gameEngine.IsGameActive)
            {
                _gameEngine.TimeRemaining = Math.Max(0, _gameEngine.TimeRemaining - 0.5);
                UpdateUI();
                if (_gameEngine.TimeRemaining <= 0)
                {
                    _gameEngine.ExecuteAction(PlayerAction.MaintainSpeed);
                }
            }
        }

        private void UpdateUI()
        {
            if (_gameEngine.SelectedVehicle != null)
            {
                // Update Lap Label and Progress
                int currentLap = _gameEngine.Circuit.IsRaceCompleted() 
                    ? _gameEngine.Circuit.TotalLaps 
                    : _gameEngine.Circuit.CurrentLap;
                LapLabel.Text = $"Lap {currentLap}/{_gameEngine.Circuit.TotalLaps}";
                
                // Calculate lap progress using UI service
                double lapProgress = _uiService.CalculateLapProgress(currentLap, _gameEngine.Circuit.TotalLaps);
                _uiService.UpdateProgressBar(LapProgressFill, lapProgress);

                // Update Fuel Progress
                double fuelPercentage = _gameEngine.SelectedVehicle.GetFuelPercentage();
                _uiService.UpdateProgressBar(FuelProgressFill, fuelPercentage);

                // Update Time & Speed
                TimeSpeedLabel.Text = $"Speed: {_gameEngine.SelectedVehicle.CurrentSpeed}";

                // Update Overall Progress (based on lap progress)
                _uiService.UpdateProgressBar(OverallProgressFill, lapProgress);
            }
            else
            {
                // Default values when no vehicle is selected
                LapLabel.Text = "Lap 1/5";
                _uiService.UpdateProgressBar(LapProgressFill, 0);
                _uiService.UpdateProgressBar(FuelProgressFill, 100);
                TimeSpeedLabel.Text = "Speed: 0";
                _uiService.UpdateProgressBar(OverallProgressFill, 0);
            }
        }

        private void EnableActionButtons(bool enabled)
        {
            SpeedUpButton.IsEnabled = enabled;
            MaintainSpeedButton.IsEnabled = enabled;
            PitStopButton.IsEnabled = enabled;
        }

        private void EndGame()
        {
            _timerService.Stop();
            EnableActionButtons(false);
            StartRaceButton.IsEnabled = true;
            CarSelectionComboBox.IsEnabled = true;
            UpdateUI();
            
            // Show game result message
            if (!string.IsNullOrEmpty(_gameEngine.GameResult))
            {
                MessageBox.Show(_gameEngine.GameResult, "Game Finished", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
