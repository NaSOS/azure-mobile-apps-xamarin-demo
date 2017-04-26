﻿using System.Threading.Tasks;
using GlobalAzureBootcamp2017.Helpers;
using Xamarin.Forms;
using System.Linq;

namespace GlobalAzureBootcamp2017
{
	public class ActivityViewModel : BaseViewModel
	{
		private Activity _activity;
		public Activity Activity
		{
			get
			{
				return _activity;
			}
			set
			{
				_activity = value;
				SetPropertyChanged("Activity");
				SetPropertyChanged("Name");
				SetPropertyChanged("Time");
                SetPropertyChanged("Place");
				SetPropertyChanged("Image");
				SetPropertyChanged("Info");
				SetPropertyChanged("SpeakerName");
				SetPropertyChanged("SpeakerInfo");
				SetPropertyChanged("SpeakerSocials");
				SetPropertyChanged("IsBookmarked");
			}
		}

		private Speaker _speaker;
		public Speaker Speaker
		{
			get
			{
				if(_speaker==null && _activity?.SpeakerId!=null){
					Task.Run(async () =>
					{
						_speaker = await AzureService.Instance.SpeakerRepository.GetByIdAsync(_activity.SpeakerId);
					}).Wait();
				}
				return _speaker;
			}
		}

		private UserActivity _userActivity;
		public UserActivity UserActivity
		{
			get
			{
				if(AuthenticationHelper.Instance.IsAuthenticated && _userActivity ==null && _activity!=null)
				{
					Task.Run(async () =>
					{
						_userActivity = await AzureService.Instance.UserActivityRepository.GetByActivityIdAsync(Activity.Id, true);
                    }).Wait();
				}
				return _userActivity;
			}
		}

		public string Time => $"{Activity?.From.ToString("hh:mm")}{Activity?.To?.ToString(" - hh:mm")}";

        public string Place => Activity?.Place ?? "";

		public string Name => Activity?.Name ?? "";

		private FormattedString _formattedInfo;
		public FormattedString Info
		{
			get
			{
				var info = Activity?.Info ?? "";
				if (_formattedInfo == null && !string.IsNullOrEmpty(info))
				{
					Task.Run(async () =>
					{
						_formattedInfo = FormatHelper.FormatStringWithKeyPhrases(info, await AzureService.Instance.TextAnalyticsRepository.GetKeyPhrasesAsync(info));
						SetPropertyChanged("Info");
					});
				}
				if (_formattedInfo != null) return _formattedInfo;
				var fs = new FormattedString();
				fs.Spans.Add(new Span { Text = info });
				return fs;
			}
		}

		public string SpeakerName => Speaker?.Name ?? "";

		public string SpeakerInfo => Speaker?.Info ?? "";

		public string SpeakerSocials => Speaker?.Socials ?? "";

		public bool IsBookmarked => UserActivity != null;

		public ImageSource Image
		{
			get
			{
				switch (Activity?.Type ?? ActivityType.Other)
				{
					case ActivityType.Registration: return Images.BuildingImageSource;
					case ActivityType.Talk: return Images.UserImageSource;
					case ActivityType.Break: return Images.CoffeeImageSource;
					case ActivityType.Launch: return Images.CutleryImageSource;
					case ActivityType.Workshop: return Images.FlaskImageSource;
					case ActivityType.Closing: return Images.GiftImageSource;
					default: return null;
				}
			}
		}

		internal async Task<bool> BookmarkAsync(bool forceSave = false)
		{
            if (IsBookmarked)
            {
                var result = await AzureService.Instance.UserActivityRepository.DeleteAsync(_userActivity);
                if (result) _userActivity = null;
            }
            else
            {
                // check overlapping
                if (!forceSave)
                {
                    var activities = await AzureService.Instance.UserActivityRepository.GetAsync();
                    if (activities.Any(a => !(a.Activity.From < Activity.From && a.Activity.To < Activity.From) || (a.Activity.From > Activity.To && a.Activity.To > Activity.To)))
                    {
                        return false;
                    }
                }

                // save
				_userActivity = new UserActivity
				{
					UserId = AuthenticationHelper.Instance.User.UserId,
					ActivityId = Activity.Id
				};

                await AzureService.Instance.UserActivityRepository.SaveAsync(_userActivity);
			}

			SetPropertyChanged("IsBookmarked");

            return true;
		}
	}
}
