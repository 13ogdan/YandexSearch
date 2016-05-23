using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AutoQuest.ViewModels
{
    public class FilteredSteets:BaseViewModel
    {
        private CancellationTokenSource _cts;
        private readonly Street[] _streets;
        private ObservableCollection<Street> _orderedOrderedStreets;

        public FilteredSteets()
        {

        }

        public FilteredSteets(IEnumerable<AutoQuest.Street> streets)
        {
            _streets = streets.Select(street => new Street(street)).ToArray();
            OrderedStreets = new ObservableCollection<Street>(_streets);
        }
        
        public async Task OnFilterChanged(IFilter filter)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            var cts = _cts;

            await Task.Run(() =>
            {
                foreach (var streetVm in _streets)
                {
                    streetVm.UpdateStates(filter);
                }
                if (!cts.IsCancellationRequested)
                {
                    Debug.WriteLine($"(Line 52) : Max distance :{filter.MaxDistance}, " +
                                    $"IsCanceled: {_cts.IsCancellationRequested}, " +
                                    $"IsLocalCanceled {cts.IsCancellationRequested}");
                    OrderedStreets = new ObservableCollection<Street>(_streets.Where(vm => vm.IsVisible).OrderBy(vm => vm.Distance));
                }
            }, cts.Token);
        }

        public ObservableCollection<Street> OrderedStreets
        {
            get { return _orderedOrderedStreets; }
            private set
            {
                _orderedOrderedStreets = value;
                OnPropertyChanged();
            }
        }
    }
}
