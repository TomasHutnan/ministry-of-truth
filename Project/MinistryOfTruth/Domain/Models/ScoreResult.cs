namespace MinistryOfTruth.Domain.Models;

public record class ScoreResult(
    int Days,
    int CorrectApprovals,
    int CorrectCensors,
    int IncorrectApprovals,
    int IncorrectCensors)
{
    public int TextsProcessed =>
        CorrectApprovals + CorrectCensors
      + IncorrectApprovals + IncorrectCensors;

    public int Score =>
        (CorrectApprovals * 10)
      + (CorrectCensors * 10)
      - (IncorrectApprovals * 15)
      - (IncorrectCensors * 10);
}
