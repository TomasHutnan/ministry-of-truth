namespace MinistryOfTruth.Domain.Models;

public record GameState(
    string Text,
    string Rule,
    int Day,
    int Score,
    int TextsRemaining,
    float TimeLeftRatio, // 0-1
    float DangerLevelRatio, // 0-1
    bool IsCorrectDecision = true,
    string StatusMessage = ""
);
