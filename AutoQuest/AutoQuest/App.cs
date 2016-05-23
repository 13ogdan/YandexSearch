using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoQuest.API;
using AutoQuest.Controls;
using AutoQuest.ViewModels;
using Xamarin.Forms;
using Filter = AutoQuest.ViewModels.Filter;

namespace AutoQuest
{
    public class App : Application
    {
        private IEnumerable<Street> _streets;
        private FilteredSteets _streetsViewModel;
        private Filter _filterViewModel;
        private ILocationService _locationService;

        public App()
        {
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new CircleProgressBar()
            };
            LoadStreets();
        }

        private void LoadStreets()
        {
            var loader = new StreetLoader();
            _streets = loader.LoadStreets();
            
            _streetsViewModel = new FilteredSteets(_streets);

            _locationService = DependencyService.Get<ILocationService>();
            _filterViewModel = new Filter(_locationService);

            var streetView = new StreetView {BindingContext = _streetsViewModel};
            var filterView = new Controls.Filter() {BindingContext = _filterViewModel};
            Grid.SetRow(streetView,1);
            var grid = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = new GridLength(1,GridUnitType.Star)}
                }
            };
            grid.Children.Add(filterView);
            grid.Children.Add(streetView);
            ((ContentPage) MainPage).Content = grid;

            _filterViewModel.FilterChanged += OnFilterChanged;
            _streetsViewModel.OnFilterChanged(_filterViewModel);
        }

        private async void OnFilterChanged(object sender, EventArgs e)
        {
            await _streetsViewModel.OnFilterChanged(_filterViewModel);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            _locationService?.StartListen();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            _locationService?.StopListen();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            _locationService?.StartListen();
        }
    }
}
