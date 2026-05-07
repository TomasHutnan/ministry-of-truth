namespace MinistryOfTruth.Domain.Models;

public record DayPackage(
    Rule Rule,
    Queue<TextEntry> DocumentStack,
    HashSet<string> ViolationIds
);
