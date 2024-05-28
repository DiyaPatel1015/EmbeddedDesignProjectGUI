using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace EmbeddedDesignProjectGUI
{
    public partial class Form1 : Form
    {
        private void InitializeChart()
        {
            // Clear any existing series
            tempChart.Series.Clear();

            // Create a new series and add it to the chart
            var series = new Series("TemperatureSeries")
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                IsVisibleInLegend = false,
            };
            tempChart.Series.Add(series);
            // Configure chart axes
            tempChart.ChartAreas[0].AxisY.Maximum = 100;
            tempChart.ChartAreas[0].AxisY.Minimum = 0;

            tempChart.ChartAreas[0].AxisX.Title = "Time";
            tempChart.ChartAreas[0].AxisY.Title = "Temperature (°C)";

            // Set chart area to adjust automatically
            tempChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Seconds;
            tempChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
        }

    }
}
