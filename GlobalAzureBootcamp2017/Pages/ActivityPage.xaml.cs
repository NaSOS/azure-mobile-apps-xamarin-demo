using System;
using Acr.UserDialogs;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class ActivityPage : ContentPage
	{
		// create a bindable property to handle socials
		readonly static BindableProperty SpeakerSocialsProperty = BindableProperty.Create(
			nameof(ActivityViewModel.SpeakerSocials), 
			typeof(string), 
			typeof(ActivityViewModel), 
			propertyChanged: SpeakerSocialsPropertyChanged
		);

		private static void SpeakerSocialsPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var page = bindable as ActivityPage;
			if (page == null) return;

			var socialsString = newValue as string;
			if (string.IsNullOrEmpty(socialsString)) return;

			// clear previous
			page.SocialsLayout.Children.Clear();

			var socials = socialsString.Split(',');
			foreach (var social in socials)
			{
				if (string.IsNullOrWhiteSpace(social)) continue;

				var uri = new Uri(social);

				var img = new MyCircleImage
				{
					HeightRequest = 25,
					WidthRequest = 25,
					ImageMargin = new Thickness(3),
					FillColor = Colors.PrimaryColor,
				};
				img.GestureRecognizers.Add(new TapGestureRecognizer{Command = new Command((obj) => Device.OpenUri(uri))});

				switch(uri.GetUriType()){
					case UriType.Facebook: img.Source = Images.FacebookImageSource; break;
					case UriType.Twitter: img.Source = Images.TwitterImageSource; break;
					case UriType.LinkedIn: img.Source = Images.LinkedInImageSource; break;
					case UriType.Github: img.Source = Images.GithubImageSource; break;
					case UriType.Website:
					default: img.Source = Images.GlobeImageSource; break;

				}

				page.SocialsLayout.Children.Add(img);
			}
		}

		readonly static BindableProperty IsBookmarkedProperty = BindableProperty.Create(
			nameof(ActivityViewModel.IsBookmarked),
			typeof(bool),
			typeof(ActivityViewModel),
			false,
			propertyChanged: IsBookmarkedPropertyPropertyChanged
		);

		private static void IsBookmarkedPropertyPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var page = bindable as ActivityPage;
			if (page == null) return;

			bool isBookmarked = false;
			bool.TryParse(""+newValue, out isBookmarked);
			page.BookmarkItem.Icon = isBookmarked ? Images.BookmarkFilled : Images.Bookmark;
		}

		public ActivityPage()
		{
			InitializeComponent();

			Title = Messages.Talk;

			// bind property to viewmodel
			SetBinding(SpeakerSocialsProperty, new Binding{
				Path = nameof(ActivityViewModel.SpeakerSocials),
				Source = this.GetViewModel<ActivityViewModel>()
			});
			SetBinding(IsBookmarkedProperty, new Binding{
				Path = nameof(ActivityViewModel.IsBookmarked),
				Source = this.GetViewModel<ActivityViewModel>()
			});
		}

		public ActivityPage(Activity activity) : this()
		{
			this.GetViewModel<ActivityViewModel>().Activity = activity;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// show login alert
			if(loginResult.HasValue)
			{				
				UserDialogs.Instance.Toast(loginResult.Value ? Messages.Welcome : Messages.SomethingWentWrong);

				this.GetViewModel<ActivityViewModel>().SetPropertyChanged(nameof(ActivityViewModel.IsBookmarked));

				// reset
				loginResult = null;
			}
		}

		bool? loginResult;

		async void BookmarkItem_Clicked(object sender, EventArgs e)
		{
			if (AuthenticationHelper.Instance.IsAuthenticated)
			{
				using (UserDialogs.Instance.Loading())
				{
					var result = await this.GetViewModel<ActivityViewModel>().BookmarkAsync();
					if (result)
					{
                        UserDialogs.Instance.Toast(this.GetViewModel<ActivityViewModel>().IsBookmarked ? Messages.Bookmarked : Messages.Unbookmarked);
					}
                    else
                    {
                        UserDialogs.Instance.Confirm(new ConfirmConfig
                        {
                            Message = Messages.MultiplePlacesAtTheSameTimeAlert,
                            OkText = Messages.Sure,
                            CancelText = Messages.Nope,
                            OnAction = async (ok) => 
                            {
                                if (!ok) return;

                                using (UserDialogs.Instance.Loading())
                                {
                                    await this.GetViewModel<ActivityViewModel>().BookmarkAsync(true);
                                }
                            }                                           
                        });
                    }
				}
			}
			else
			{
				UserDialogs.Instance.Confirm(new ConfirmConfig
				{
					Message = Messages.YouHaveToBeLoggedToBookmark,
					OkText = Messages.LogIn,
					CancelText = Messages.Nope,
					OnAction = async (ok) =>
					{
						if (!ok) return;

						loginResult = await AuthenticationHelper.Instance.AuthenticateAsync();
					}
				});
			}
		}

	}
}
