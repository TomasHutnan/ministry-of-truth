namespace MinistryOfTruth.Domain.Interfaces
{
    public interface INavigationService
    {
        Task GoToStartAsync();
        Task GoToMenuAsync();
        Task GoToGameAsync();
        Task GoToResultsAsync();
    }
}
