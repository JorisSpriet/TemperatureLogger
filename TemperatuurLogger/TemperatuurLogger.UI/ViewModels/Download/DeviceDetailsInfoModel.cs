using System;
using ReactiveUI;
using System.Linq;
using System.Reactive;
using TemperatuurLogger.Model;
using TemperatuurLogger.Protocol;
using Xtensive.Orm;

namespace TemperatuurLogger.UI.ViewModels
{
    public class DeviceDetailsInfoModel : ViewModelBase
    {
        string loggerName = "onbekend";
        string lastDownload = "onbekend";
        bool loggerIsKnown;
        DeviceDetails details;

        public ReactiveCommand<Unit, Unit> CreateLogger { get; private set; }

        public bool LoggerIsKnown
        {
            get => loggerIsKnown;
            set => this.RaiseAndSetIfChanged(ref loggerIsKnown, value);
        }

        public string LoggerName
        {
            get => loggerName;
            set => this.RaiseAndSetIfChanged(ref loggerName, value);
        }

        public string SerialNumber => details.SerialNumber;
        public int NumberOfLogs => details.NumberOfSamples;

        public string LastDownload
        {
            get => lastDownload;
            set => this.RaiseAndSetIfChanged(ref lastDownload, value);
        }

        private void RetrieveInfoOnLogger()
        {
            DeviceService.With(() =>
            {
                var l = DeviceService.Instance.GetLogger(details.SerialNumber);

                LoggerIsKnown = l != null;

                if (!LoggerIsKnown) return;

                LoggerName = l.Name;
                LastDownload = l.LastDownloadTime?.ToString("dd/MM/yyyy") ?? "onbekend";

            });
        }

        private Unit DoCreateLogger(Unit arg)
        {
            DeviceService.With(() =>
                {
                    //TODO MOVE CREATION OF LOGGER TO MANAGEMENT PAGE
                    var logger = DeviceService.Instance.CreateLogger(SerialNumber, LoggerName, DateTime.Now);
                    LoggerIsKnown = true;
                }
            );

            return Unit.Default;
        }


        void InitCommands()
        {
            //var can
            var canCreateLogger = this.WhenAnyValue(vm => vm.LoggerIsKnown, x => !x);
            CreateLogger = ReactiveCommand.Create<Unit, Unit>(DoCreateLogger, canCreateLogger);

        }


        public DeviceDetailsInfoModel(DeviceDetails details)
        {
            this.details = details;

            InitCommands();

            RetrieveInfoOnLogger();
        }


    }
}
