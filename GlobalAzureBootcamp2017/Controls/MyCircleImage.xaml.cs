using Xamarin.Forms;

namespace GlobalAzureBootcamp2017
{
	public partial class MyCircleImage : ContentView
	{
		public MyCircleImage()
		{
			InitializeComponent();
		}

		public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(MyCircleImage), Color.Transparent);
		public Color FillColor
		{
			get { return (Color)GetValue(FillColorProperty); }
			set
			{
				SetValue(FillColorProperty, value);
			}
		}

		public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(MyCircleImage), default(ImageSource));
		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set
			{
				SetValue(SourceProperty, value);
			}
		}

		public static readonly BindableProperty ImageMarginProperty = BindableProperty.Create(nameof(ImageMargin), typeof(Thickness), typeof(MyCircleImage), default(Thickness));
		public Thickness ImageMargin
		{
			get { return (Thickness)GetValue(ImageMarginProperty); }
			set
			{
				SetValue(ImageMarginProperty, value);
			}
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if(propertyName == FillColorProperty.PropertyName){
				CircleImage.FillColor = FillColor;
			}
			else if(propertyName == SourceProperty.PropertyName){
				Image.Source = Source;
			}
			else if (propertyName == PaddingProperty.PropertyName)
			{
				Image.Margin = ImageMargin;
			}
		}
	}
}
