using System.Collections.Concurrent;

namespace JiwaCustomerPortal
{
    public interface IColourModeServices
    {
        public event Action? OnChange;
        string ColourMode { get; set; }
    }

    public class ColourModeServices : IColourModeServices
    {
        public event Action? OnChange;

        private string _ColorMode = "dark";

        public string ColourMode
        {
            get
            {
                return _ColorMode;
            }
            set
            {
                _ColorMode = value;
                NotifyColourChanged();
            }
        }

        private void NotifyColourChanged() => OnChange?.Invoke();
    }
}
