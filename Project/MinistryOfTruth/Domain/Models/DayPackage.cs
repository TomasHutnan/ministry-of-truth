namespace MinistryOfTruth.Domain.Models;

public record DayPackage(
    string RuleDescription,
    Queue<TextEntry> DocumentStack,
    HashSet<string> ViolationIds
);
