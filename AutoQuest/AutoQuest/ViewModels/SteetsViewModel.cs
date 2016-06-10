// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private IEnumerable<StreetCommand> _availableCommands;

        public SteetsViewModel() { }

        public SteetsViewModel(IEnumerable<Street> streets, INavigationService navigationService)
        {
            _navigationService = navigationService;
            _streetsViewModel = streets.Select(street => new StreetViewModel(street)).ToArray();
            foreach (var streetViewModel in _streetsViewModel)
                streetViewModel.NavigateCommand = new Command<StreetViewModel>(SelectStreet);

            OrderedStreets = new ObservableCollection<StreetViewModel>(_streetsViewModel);
        }

        private async void SelectStreet(StreetViewModel streetViewModel)
        {
            if (_availableCommands == null)
                _availableCommands = GetAvailableCommands().ToArray();

            if (!_availableCommands.Any())
            {
                await Application.Current.MainPage.DisplayAlert("Ой", "Установите яндекс навигатор или карту.", null);
                return;
            }
                
            var result = await Application.Current.MainPage.DisplayActionSheet(streetViewModel.Name, "Назад", null,
                    _availableCommands.Select(command => command.Name).ToArray());

            var selectedCommand = _availableCommands.FirstOrDefault(command => command.Name == result);
            selectedCommand?.Execute(streetViewModel);
        }

        private IEnumerable<StreetCommand> GetAvailableCommands()
        {
            var service = DependencyService.Get<IClipboardService>();
            if (service != null)
                yield return new StreetCommand("Копировать", model => service.SaveToClipboard(model.Name));

            var navigationService = DependencyService.Get<INavigationService>();
            if (navigationService == null || !navigationService.IsAvailable)
                yield break;

            yield return new StreetCommand("Найти на карте", model => navigationService.ShowOnMap(model.CenterStreetPoint));
            yield return new StreetCommand("Поехали", model => navigationService.NavigateTo(model.CenterStreetPoint));
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

        public class StreetCommand
        {
            private readonly Action<StreetViewModel> _action;

            public StreetCommand(string name, Action<StreetViewModel> action)
            {
                Name = name;
                _action = action;
            }

            public string Name { get; }

            public void Execute(StreetViewModel street)
            {
                _action.Invoke(street);
            }
        }
    }
}