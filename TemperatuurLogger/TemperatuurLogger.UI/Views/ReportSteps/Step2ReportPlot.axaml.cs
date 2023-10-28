using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ScottPlot.Avalonia;
using System.Collections.Generic;

namespace TemperatuurLogger.UI.Views.ReportSteps
{
	public class Step2ReportPlot : UserControl, ICanPlot
	{
		AvaPlot avaPlot;

		public Step2ReportPlot()
		{
			InitializeComponent();
		}

		public void Plot(string title, List<DataPoint> dataPoints)
		{
			avaPlot = this.Find<AvaPlot>("avaPlot");
			var xs = dataPoints.Select(dp => dp.Timestamp.ToOADate()).ToArray();
			var ys = dataPoints.Select(dp => Convert.ToDouble(dp.Temperature)).ToArray();

			avaPlot.Plot.PlotScatter(xs, ys);
			avaPlot.Plot.XAxis.DateTimeFormat(true);
			avaPlot.Plot.Title(title, bold: true);
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
