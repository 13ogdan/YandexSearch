// <copyright>"☺ Raccoon corporation ©  1989"</copyright>

using System.ComponentModel;
using System.Runtime.CompilerServices;
using AutoQuest.Annotations;
using Xamarin.Forms;

namespace AutoQuest.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Device.BeginInvokeOnMainThread(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); });
        }
    }
}