using MinistryOfTruth.Domain.Models;

namespace MinistryOfTruth.Domain.Interfaces;

public interface IDocumentGenerator
{
    Task InitializeAsync();
    DayPackage CreateDayPackage(int dayNumber, int targetComplexity);
}
