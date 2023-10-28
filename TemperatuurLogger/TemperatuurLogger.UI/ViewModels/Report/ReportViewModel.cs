using System;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;

namespace TemperatuurLogger.UI.ViewModels
{
	public class ReportViewModel : ViewModelBase
	{
		ReportViewModelState state;
		ReportCriteriaViewModel reportCriteriaViewModel;

		public ReportCriteriaViewModel ReportCriteriaViewModel
		{
			get => reportCriteriaViewModel;
			set => this.RaiseAndSetIfChanged(ref reportCriteriaViewModel, value);
		}

		public ReactiveCommand<ICanNext, Unit> Next { get; private set; }

		public ReactiveCommand<ICanClose, Unit> Cancel { get; private set; }

		public ReportViewModelState State
		{
			get => state;
			set
			{
				this.RaiseAndSetIfChanged(ref state, value);
				this.RaisePropertyChanged(nameof(NextActionText));
				this.RaisePropertyChanged(nameof(CancelActionText));
			}

		}

		public string CancelActionText => State == ReportViewModelState.Done ? "Sluiten" : "Annuleren";
		public string NextActionText
		{
			get
			{
				switch (State) {
					case ReportViewModelState.DataValidated: return "Genereren";
					case ReportViewModelState.Rendered: return "Printen";
					default: return "";
				}
			}
		}

		private Unit DoCancel(ICanClose view)
		{
			view?.Close();
			return Unit.Default;
		}

		#region Actions

		private void DoCriteriaEntry()
		{
			ReportCriteriaViewModel = new ReportCriteriaViewModel();
			ReportCriteriaViewModel.LoggerSelected += (s, e) => { State = ReportViewModelState.DataValidated; };
		}

		private void DoRender(UserControl p)
		{
			var reportPlotViewModel = new ReportPlotViewModel();
			reportPlotViewModel.FromDate = reportCriteriaViewModel.FromDate;
			reportPlotViewModel.EndDate = reportCriteriaViewModel.EndDate;
			reportPlotViewModel.SelectedLogger = reportCriteriaViewModel.SelectedLogger;
			p.DataContext = reportPlotViewModel;
			reportPlotViewModel.Plotter = p as ICanPlot;
		}

		#endregion


		void InitCommands()
		{
			//Canceling
			var canCancel = this.WhenAnyValue(vm => vm.State, (x) => x != ReportViewModelState.Done);
			Cancel = ReactiveCommand.Create<ICanClose, Unit>(DoCancel, canCancel);

			//Moving on
			var canNext = this.WhenAnyValue(vm => vm.State, x => CanDoNext(x));
			Next = ReactiveCommand.Create<ICanNext, Unit>(DoNext, canNext);
		}		

		bool CanDoNext(ReportViewModelState state)
		{
			return state == ReportViewModelState.DataValidated||
				state == ReportViewModelState.Rendered ||
				state == ReportViewModelState.Printed ||
				state == ReportViewModelState.Done;
		}

		void HandleStateTransition(UserControl p)
		{
			switch (State) {
				case ReportViewModelState.Rendering:					
					DoRender(p);
					break;
				case ReportViewModelState.Rendered:
					//DoPrint(p, reportCriteriaViewModel);
					break;
				case ReportViewModelState.Printed:
					//DoPersist(p);
					break;
				case ReportViewModelState.Done:
					//this.RaisePropertyChanged("CancelActionText");
					break;
				default:
					throw new InvalidOperationException($"Nothing to launch when state is {State}");
			}
		}


		Unit DoNext(ICanNext canNext)
		{
			var p = canNext.Next();

			State = State + 1;
			HandleStateTransition(p);

			return Unit.Default;
		}

		public ReportViewModel()
		{

			InitCommands();

			DoCriteriaEntry();
		}		
	}
}
