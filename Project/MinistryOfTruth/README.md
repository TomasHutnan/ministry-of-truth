

# Solution Structure
The solution is organized by architectural responsibility, following a **UI → ViewModel → Domain → Data** dependency flow:
```
MinistryOfTruth.sln
│
├─ MinistryOfTruth.App/              # MAUI application & composition root
│  ├─ MauiProgram.cs                # Startup
│  ├─ App.xaml
│  ├─ Views/
│  │  ├─ LoadingView.xaml           # Async initialization & preload
│  │  ├─ MainMenuView.xaml          # High score, imports, game start
│  │  ├─ GameView.xaml              # Gameplay UI
│  │  └─ ResultsView.xaml           # Post-game evaluation
│  └─ Assets/                       # Embedded default content
│     ├─ texts.csv
│     ├─ rules.csv
│     └─ violations.csv
│
├─ MinistryOfTruth.ViewModels/       # Presentation logic
│  ├─ ViewModelBase.cs              # INotifyPropertyChanged helper
│  ├─ LoadingViewModel.cs           # Startup & preload orchestration
│  ├─ MainMenuViewModel.cs
│  ├─ GameViewModel.cs
│  └─ ResultsViewModel.cs
│
├─ MinistryOfTruth.Domain/           # Game logic
│  ├─ GameEngine.cs                 # Game orchestration & state changes
│  ├─ GameState.cs                  # Mutable runtime game state
│  ├─ Rule.cs                       # Immutable censorship rule
│  ├─ TextEntry.cs                  # Game text entry
│  ├─ Violation.cs                  # Rule–text conflict definition
│  ├─ ScoreResult.cs                # Immutable end-of-game summary
│  └─ RuleTextFormatter.cs          # Rule presentation helpers
│
├─ MinistryOfTruth.Data/              # Async persistence & CSV loading
│  ├─ CsvRuleRepository.cs           # rules.csv access
│  ├─ CsvTextRepository.cs           # texts.csv access
│  ├─ CsvViolationRepository.cs      # violations.csv access
│  └─ FileHighScoreStore.cs          # highscore.txt persistence
│
└─ README.md                         # Project documentation (current file)

```