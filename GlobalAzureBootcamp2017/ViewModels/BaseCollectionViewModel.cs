using System.Collections.ObjectModel;

namespace GlobalAzureBootcamp2017
{
	public abstract class BaseCollectionViewModel<T> : BaseViewModel
	{
		ObservableCollection<T> _collection = new ObservableCollection<T>();
		public ObservableCollection<T> Collection
		{
			get
			{
				return _collection;
			}
			set
			{
				_collection = value;
				SetPropertyChanged("Collection");
				Collection?.Clear();
			}
		}

		private bool _isEmptyMessageVisible = true;
		public bool IsEmptyMessageVisible
		{
			get
			{
				return _isEmptyMessageVisible;
			}
			set
			{
				if (_isEmptyMessageVisible == value) return;
				_isEmptyMessageVisible = value;
				SetPropertyChanged("IsEmptyMessageVisible");
			}
		}
	}
}
