using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CsvHelper;
using StitchingDesigner.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace StitchingDesigner.ViewModels
{
    public partial class PalleteViewModel : ObservableObject
    {
        private const string FlossCsvAssetName = "threadcolors_dmc_rgb.csv";
        private const int SearchDebounceMilliseconds = 300;

        private readonly Dictionary<string, FlossModel> _idToFloss;
        private readonly List<FlossModel> _allFlosses;
        private CancellationTokenSource? _searchDebounceCts;

        [ObservableProperty]
        public partial FlossModel? SelectedFloss { get; set; }

        [ObservableProperty]
        public partial string SearchText { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<FlossModel> SearchResults { get; set; }

        public PalleteViewModel()
        {
            _idToFloss = [];
            _allFlosses = [];
            SearchText = string.Empty;
            SearchResults = [];

            using var stream = FileSystem.OpenAppPackageFileAsync(FlossCsvAssetName).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

            foreach (FlossModel floss in csv.GetRecords<FlossModel>())
            {
                _idToFloss[floss.Id] = floss;
                _allFlosses.Add(floss);
            }

            foreach (var floss in _allFlosses.OrderBy(f => f.Id, StringComparer.OrdinalIgnoreCase))
            {
                SearchResults.Add(floss);
            }

            SelectedFloss = _allFlosses[0];
        }

        partial void OnSearchTextChanged(string value)
        {
            DebounceSearch(value);
        }

        [RelayCommand]
        private void SelectFloss(FlossModel? floss)
        {
            if (floss is null)
            {
                return;
            }

            SelectedFloss = floss;
        }

        public bool SelectFloss(string id)
        {
            if (_idToFloss.TryGetValue(id, out FlossModel? floss))
            {
                SelectedFloss = floss;
                return true;
            }

            return false;
        }

        public FlossModel? GetFlossById(string id)
        {
            return _idToFloss.TryGetValue(id, out var floss) ? floss : null;
        }

        public void ResetFloss()
        {
            SelectedFloss = null;
        }

        private void DebounceSearch(string query)
        {
            _searchDebounceCts?.Cancel();
            _searchDebounceCts?.Dispose();

            var cts = new CancellationTokenSource();
            _searchDebounceCts = cts;
            _ = PerformSearchAsync(query, cts.Token);
        }

        private async Task PerformSearchAsync(string query, CancellationToken token)
        {
            try
            {
                await Task.Delay(SearchDebounceMilliseconds, token);

                var normalized = query.Trim();
                var matches = string.IsNullOrWhiteSpace(normalized)
                    ? _allFlosses
                    : _allFlosses.Where(f =>
                        f.Id.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                        f.Name.Contains(normalized, StringComparison.OrdinalIgnoreCase)).ToList();

                var orderedMatches = matches.OrderBy(f => f.Id, StringComparer.OrdinalIgnoreCase).ToList();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    SearchResults.Clear();
                    foreach (var floss in orderedMatches)
                    {
                        SearchResults.Add(floss);
                    }
                });
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
}
