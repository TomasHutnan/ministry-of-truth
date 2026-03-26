using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using StitchingDesigner.Models;
using System.Globalization;

namespace StitchingDesigner.ViewModels
{
    public partial class PalleteViewModel : ObservableObject
    {
        private const string FlossCsvAssetName = "threadcolors_dmc_rgb.csv";

        private readonly Dictionary<string, Floss> _idToFloss;

        [ObservableProperty]
        public partial Floss? SelectedFloss { get; set; } = null;

        public PalleteViewModel()
        {
            _idToFloss = new Dictionary<string, Floss>();

            using var stream = FileSystem.OpenAppPackageFileAsync(FlossCsvAssetName).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            foreach (Floss floss in csv.GetRecords<Floss>())
            {
                _idToFloss[floss.Id] = floss;
            }
        }

        public bool SelectFloss(string id)
        {
            if (_idToFloss.TryGetValue(id, out Floss? floss))
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
