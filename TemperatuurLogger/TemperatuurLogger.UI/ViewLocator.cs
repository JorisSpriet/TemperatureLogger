using Avalonia.Controls;
using Avalonia.Controls.Templates;
using System;
using TemperatuurLogger.UI.ViewModels;

namespace TemperatuurLogger.UI
{
	public class ViewLocator : IDataTemplate
	{
		public bool SupportsRecycling => false;

		public bool Match(object data)
		{
			return data is ViewModelBase;
		}

        Control? ITemplate<object?, Control?>.Build(object? data)
        {
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }
            else
            {
                return new TextBlock { Text = "Not Found: " + name };
            }
            throw new NotImplementedException();
        }
    }
}
