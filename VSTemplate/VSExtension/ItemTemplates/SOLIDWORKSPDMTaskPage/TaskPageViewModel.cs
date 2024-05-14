using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace $rootnamespace$
{
    public class $safeitemname$ : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
