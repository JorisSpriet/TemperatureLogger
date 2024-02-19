using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;

namespace TemperatuurLogger.UI.Views.ReportSteps
{
	public partial class Step2ReportCreation : UserControl//, ICanPlot
	{

		public Step2ReportCreation()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
