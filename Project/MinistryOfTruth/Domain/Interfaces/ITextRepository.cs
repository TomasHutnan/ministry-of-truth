using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Interfaces;

public interface ITextRepository
{
    Task<IReadOnlyCollection<TextEntry>> LoadAllAsync();
    Task SetAllAsync(IEnumerable<TextEntry> texts);
}
