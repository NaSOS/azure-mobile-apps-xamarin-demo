using System.Collections.Generic;

namespace GlobalAzureBootcamp2017
{
	public class ActivitiesViewModel : BaseCollectionViewModel<ActivityViewModel>
	{
		public ActivitiesViewModel(){}

		public ActivitiesViewModel(IEnumerable<ActivityViewModel> collecion){
			foreach(var item in collecion){
				Collection.Add(item);
			}
			IsEmptyMessageVisible = Collection.Count <= 0;
		}
	}
}
