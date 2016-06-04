// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using Xamarin.Forms;

namespace AutoQuest.Controls
{
    public partial class FilterSettings : ContentPage
    {
        public FilterSettings()
        {
            InitializeComponent();
        }

        /// <summary>Application developers can override this method to provide behavior when the back button is pressed.</summary>
        /// <returns>To be added.</returns>
        /// <remarks>To be added.</remarks>
        protected override bool OnBackButtonPressed()
        {
            var caruselPage = (App.Current.MainPage as CarouselPage);
            if (caruselPage == null)
                return base.OnBackButtonPressed();

            caruselPage.CurrentPage = caruselPage.Children[0];
            return true;
        }
    }
}