namespace MinistryOfTruth.Domain.Models;

public record class Rule(
    string Id,
    string Keyword,
    bool IsPlural)
{
}
