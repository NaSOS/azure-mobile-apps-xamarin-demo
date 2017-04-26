using System.ComponentModel;

namespace GlobalAzureBootcamp2017
{
	public abstract class BaseNotify : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		internal void SetPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

