using System;
using ReactiveUI;
using System.Reactive;
using Avalonia.Controls;

namespace TemperatuurLogger.UI.ViewModels
{
    public partial class ReportViewModel : ViewModelBase
    {
        private int reportYear;

        ReportViewModelState state;

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

        public string CancelActionText => "Annuleren";
        public string NextActionText
        {
            get
            {
                switch (State)
                {
                    case ReportViewModelState.DataEntry: return "Gebruik";
                    case ReportViewModelState.ReportGeneration: return "Genereren";
                    case ReportViewModelState.ReportGenerating: return "Opslaan";
                    default: return "Verder";
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
            LoggerSelected += (s, e) => { State = ReportViewModelState.DataEntered; };
            GetLoggers(reportYear);
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
            return state == ReportViewModelState.DataEntered ||
                state == ReportViewModelState.ReportGeneration ||
                state == ReportViewModelState.Done;
        }

        void HandleStateTransition(UserControl p)
        {
            switch (State)
            {
                case ReportViewModelState.ReportGeneration:
                    ChooseDestination();
                    break;
                case ReportViewModelState.ReportGenerating:
                    DoRender(p);
                    break;
                case ReportViewModelState.Done:
                    DoEnd();
                    break;
                default:
                    throw new InvalidOperationException($"Nothing to launch when state is {State}");
            }
        }

        private void DoEnd()
        {
            this.RaisePropertyChanged("CancelActionText");
        }


        Unit DoNext(ICanNext canNext)
        {
            if (State == ReportViewModelState.Done)
            {
                DoCancel(canNext as ICanClose);
            }
            else
            {
                var p = canNext.Next();

                State = State + 1;
                HandleStateTransition(p);
            }
            return Unit.Default;
        }

        public ReportViewModel():this(DateTime.Now.Year.ToString())
        { }


        public ReportViewModel(string year)
        {
            reportYear=int.Parse(year);

            InitCommands();

            DoCriteriaEntry();
        }
    }
}
