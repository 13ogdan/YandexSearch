// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoQuest.API;
using Xamarin.Forms;

namespace AutoQuest.ViewModels
{
    public class SteetsViewModel : BaseViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly StreetViewModel[] _streetsViewModel;
        private CancellationTokenSource _cts;
        private ObservableCollection<StreetViewModel> _orderedOrderedStreets;

        public SteetsViewModel() {}

        public SteetsViewModel(IEnumerable<Street> streets, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _streetsViewModel = streets.Select(street => new StreetViewModel(street)).ToArray();
            if (navigationService != null && navigationService.IsAvailable)
                foreach (var streetViewModel in _streetsViewModel)
                {
                    streetViewModel.NavigateCommand = new Command<StreetViewModel>(NavigateTo); 
                }
            OrderedStreets = new ObservableCollection<StreetViewModel>(_streetsViewModel);
        }

        private void NavigateTo(StreetViewModel streetViewModel)
        {
            _navigationService.NavigateTo(streetViewModel.CenterStreetPoint);
        }

        public ObservableCollection<StreetViewModel> OrderedStreets
        {
            get { return _orderedOrderedStreets; }
            private set
            {
                _orderedOrderedStreets = value;
                OnPropertyChanged();
            }
        }

        public async Task OnFilterChanged(IFilter filter)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var cts = _cts;
            try
            {
                await Task.Run(() =>
                {
                    foreach (var streetVm in _streetsViewModel)
                    {
                        streetVm.UpdateStates(filter);
                    }
                    if (!cts.IsCancellationRequested)
                    {
                        Debug.WriteLine($"Max distance :{filter.MaxDistance}, " +
                                        $"IsCanceled: {_cts.IsCancellationRequested}, " +
                                        $"IsLocalCanceled {cts.IsCancellationRequested}");
                        OrderedStreets =
                            new ObservableCollection<StreetViewModel>(_streetsViewModel.Where(vm => vm.IsVisible).OrderBy(vm => vm.Distance));
                    }
                }, cts.Token);
            }
            catch (TaskCanceledException canceledException)
            {
                //DO nothing
            }
        }
    }
}