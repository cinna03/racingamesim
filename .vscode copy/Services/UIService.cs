using System.Windows.Controls;
using TimeBasedRacingGame.Domain;

namespace TimeBasedRacingGame.Services
{
    /// <summary>
    /// Service for managing UI updates and progress bar calculations
    /// </summary>
    public class UIService
    {
        /// <summary>
        /// Updates a progress bar fill based on percentage
        /// </summary>
        /// <param name="progressFill">The border element representing the progress fill</param>
        /// <param name="percentage">Percentage value (0-100)</param>
        public void UpdateProgressBar(Border progressFill, double percentage)
        {
            if (progressFill?.Parent is Border parentBorder)
            {
                // Use ActualWidth if available, otherwise use a default or measure
                double maxWidth = parentBorder.ActualWidth;
                if (maxWidth <= 0)
                {
                    // Force a layout update if width is not available
                    parentBorder.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
                    parentBorder.Arrange(new System.Windows.Rect(0, 0, parentBorder.DesiredSize.Width, parentBorder.DesiredSize.Height));
                    maxWidth = parentBorder.ActualWidth > 0 ? parentBorder.ActualWidth : 200;
                }
                progressFill.Width = System.Math.Max(0, System.Math.Min(maxWidth, maxWidth * (percentage / 100.0)));
            }
        }

        /// <summary>
        /// Calculates lap progress percentage
        /// </summary>
        /// <param name="currentLap">Current lap number</param>
        /// <param name="totalLaps">Total number of laps</param>
        /// <returns>Lap progress percentage</returns>
        public double CalculateLapProgress(int currentLap, int totalLaps)
        {
            return totalLaps > 0 ? ((currentLap - 1) * 100.0 / totalLaps) : 0;
        }
    }
}

