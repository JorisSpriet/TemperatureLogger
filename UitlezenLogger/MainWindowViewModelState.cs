using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UitlezenLogger
{
	public enum MainWindowViewModelState
	{
		Undefined=0,
		Start =1,

		LookingForLogger,
		LoggerFound,
		ReadingLoggerInfo,
		LoggerInfoRead,
		ReadingLoggerData,
		LoggerDataRead,
		StoringLoggerData,
		AskForClearingLogger,
		ClearLogger,
		CloseLogger,
		Done,
		Error
	}
}
