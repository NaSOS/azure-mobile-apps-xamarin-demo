namespace GlobalAzureBootcamp2017
{
    public class EventUpdateViewModel : BaseViewModel
    {
        private EventUpdate _eventUpdate;
		public EventUpdate EventUpdate
		{
			get
			{
				return _eventUpdate;
			}
			set
			{
				_eventUpdate = value;
				SetPropertyChanged("EventUpdate");
				SetPropertyChanged("Time");
				SetPropertyChanged("Message");
			}
		}

        public string Time => $"{EventUpdate?.UpdatedAt?.ToString("dd/MM/yyyy hh:mm")}";

        public string Message => EventUpdate?.Message ?? "";
    }
}
