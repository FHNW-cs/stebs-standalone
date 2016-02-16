using System.ComponentModel;

namespace Stebs.ViewModel
{
    /// <summary>
    /// Base clase for the View Models
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sends an event to the Views that a property has changed, so the Views can update the value
        /// </summary>
        /// <param name="propertyName">The changed property</param>
        protected virtual void OnPropertyChanged(string propertyName) {
            
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null) {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
