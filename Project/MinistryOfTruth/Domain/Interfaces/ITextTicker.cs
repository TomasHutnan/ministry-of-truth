namespace MinistryOfTruth.Domain.Interfaces
{
    public interface ITextTicker
    {
        public event EventHandler<string>? TextUpdated;

        public Task StartTickerAsync();
        public void StopTicker();
    }
}
