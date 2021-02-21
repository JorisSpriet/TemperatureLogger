using Avalonia.Input;

namespace TemperatuurLogger.UI
{
	public interface ICanClose : ICloseable
	{
		void Close();
	}
}
