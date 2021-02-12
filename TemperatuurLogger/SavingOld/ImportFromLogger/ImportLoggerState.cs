using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TemperatuurLogger
{
	public enum ImportLoggerState
	{
		NotConnectedNoComPort,
		NotConnectedSearching,
		ConnectedGettingInfo,
		ConnectedReady,
		ConnectedDownloading,
		ConnectedDeleting,
		ConnectedDisconnecting
	}
}
