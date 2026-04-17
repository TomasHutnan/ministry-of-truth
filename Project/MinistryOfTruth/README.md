# Ministry of Truth
Ministry of Truth is a small 2D text-based decision‑making game built as a .NET MAUI course project. Inspired by the dystopian bureaucracy of George Orwell's 1984, the player takes on the role of a state censor tasked with approving or censoring documents under shifting ideological rules.
Each round presents the player with a short text and a single active rule. The challenge lies in making fast, precise decisions: texts may violate multiple rules, but only violations of the currently active rule matter. Over‑censorship and missed violations are both penalized, encouraging restraint, attention, and deliberate compliance rather than vigilance.

## Game Features
- Time‑pressured decision making with asynchronous round timers.
- Doublethink mechanics, where players must ignore irrelevant transgressions.
- Data‑driven content, with texts, rules, and violations loaded from CSV files.
- Deterministic gameplay, decoupled from UI and persistence concerns.
- Player progress is scored per session, with the highest score stored persistently. Custom content packs can be imported, and the application can be reset to its default state at any time.
- Clean architecture, MVVM separation, and explicit domain logic.


## Solution Structure
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