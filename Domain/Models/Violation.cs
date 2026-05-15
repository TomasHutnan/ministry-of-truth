namespace MinistryOfTruth.Domain.Models;

public record class Violation(
    string TextId, 
    string RuleId, 
    string Justification)
{
}
