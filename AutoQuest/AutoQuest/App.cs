// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AutoQuest.API;
using AutoQuest.Controls;
using AutoQuest.ViewModels;
using Xamarin.Forms;

namespace AutoQuest
{
    public class App : Application
    {
        private FilterViewModel _filterViewModelViewModel;
        private ILocationService _locationService;
        private IEnumerable<Street> _streets;
        private SteetsViewModel _streetsViewModel;
        private INavigationService _navigationService;

        public App()
        {
            // The root page of your application
            MainPage = new ContentPage
            {
                Content = new ActivityIndicator {IsRunning = true}
                // Content = new CircleProgressBar()
            };
            LoadStreets();
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

        private async void LoadStreets()
        {
            // Load streets
            var loader = new StreetLoader();
            _streets = loader.LoadStreets().ToArray();

            // Create view models
            _navigationService = DependencyService.Get<INavigationService>();
            _streetsViewModel = new SteetsViewModel(_streets, _navigationService);
            _locationService = DependencyService.Get<ILocationService>();
            _filterViewModelViewModel = new FilterViewModel(_locationService);
            var streetTypeViewModels = _streets.Select(street => street.Type).Distinct();
            _filterViewModelViewModel.Types =
                new ObservableCollection<StreetTypeViewModel>(streetTypeViewModels.Select(name => new StreetTypeViewModel(name)));

            // Setup first run
            _filterViewModelViewModel.FilterChanged += OnFilterChanged;
            await _streetsViewModel.OnFilterChanged(_filterViewModelViewModel);

            //Init view
            var carouselPage = new CarouselPage();
            carouselPage.Children.Add(new SearchPage
            {
                BindingContext = new SearchPageViewModel
                {
                    Streets = _streetsViewModel,
                    FilterViewModel = _filterViewModelViewModel
                }
            });
            carouselPage.Children.Add(new FilterSettings {BindingContext = _filterViewModelViewModel});
            MainPage = carouselPage;
        }

        private async void OnFilterChanged(object sender, EventArgs e)
        {
            await _streetsViewModel.OnFilterChanged(_filterViewModelViewModel);
        }
    }
}