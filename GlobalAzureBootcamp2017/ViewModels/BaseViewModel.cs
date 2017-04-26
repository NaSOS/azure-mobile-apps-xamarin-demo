using System;
namespace GlobalAzureBootcamp2017
{
	public abstract class BaseViewModel : BaseNotify
	{
		private bool _isBusy;
		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				if (_isBusy == value) return;
				_isBusy = value;
				SetPropertyChanged("IsBusy");
			}
		}
	}
}
