using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Interfaces;

public interface IViolationRepository
{
    Task<IReadOnlyCollection<Violation>> LoadAllAsync();
    Task SetAllAsync(IEnumerable<Violation> violations);
}

