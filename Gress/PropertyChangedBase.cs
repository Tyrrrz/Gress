using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Gress
{
    /// <summary>
    /// Basic implementation of <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        /// <inheritdoc />
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Triggers an event informing that the value of the property with given name has been changed.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}