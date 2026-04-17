using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Interfaces;

public interface IRuleRepository
{
    Task<IReadOnlyCollection<Rule>> LoadAllAsync();
}
