using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATALogger
{
	public class FileLog
	{
		public FileLogHeader Header { get; private set; }

		public FileLogItem[] Items { get; private set; }

		public FileLog(FileLogHeader header, FileLogItem[] items)
		{
			Header = header;
			Items = items;
		}
	}
}
