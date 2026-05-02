namespace MinistryOfTruth.Domain.Models;

public record class ScoreResult(
    int Days = 0,
    int CorrectApprovals = 0,
    int CorrectCensors = 0,
    int IncorrectApprovals = 0,
    int IncorrectCensors = 0,
    int Missed = 0)
{
    public int TextsProcessed =>
        CorrectApprovals + CorrectCensors
      + IncorrectApprovals + IncorrectCensors;

    public int Score =>
        (CorrectApprovals * 10)
      + (CorrectCensors * 10)
      - (IncorrectApprovals * 15)
      - (IncorrectCensors * 10)
      - (Missed * 12);
}
