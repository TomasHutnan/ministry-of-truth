using CommunityToolkit.Mvvm.ComponentModel;
using StitchingDesigner.Models;

namespace StitchingDesigner.ViewModels
{
    public partial class Pallete : ObservableObject
    {
        private Dictionary<int, Floss> _idToFloss;
        [ObservableProperty] public partial Floss? SelectedFloss { get; set; } = null;

        public Pallete()
        {
            // Load pallete
        }

        public bool SelectFloss(int id)
        {
            if (_idToFloss.TryGetValue(id, out Floss floss))
            {
                SelectedFloss = floss;
                return true;
            }

            return false;
        }

        public void ResetFloss()
        {
            SelectedFloss = null;
        }
    }
}
