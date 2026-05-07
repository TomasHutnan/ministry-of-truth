# Ministry of Truth
Ministry of Truth is a small 2D text-based decision‑making game implemented as a .NET MAUI course project. The player acts as a state censor charged with approving or censoring documents under an active ideological rule.

## Key ideas
- Fast, time‑pressured decisions: each round presents a short text and a single active rule.
- Mistakes and over‑censorship are both penalized to encourage careful decision making.
- Game logic is deterministic and separated from UI and persistence so it is easy to test.

## What’s implemented
- Data driven: texts, rules and violations are loaded from CSV files. CSV parsing supports quoted fields so text content can contain commas and escaped quotes.
- Text set management: the app bundles a default text set archive (default_text_set.zip) and can load custom text‑set ZIPs. Text sets are extracted and stored into application local storage for the repositories to use.
- Consolidated repository: a TextSetRepository implements the repository interfaces and the text‑set loader API. It centralizes CSV parsing, reading/writing and ZIP import/export using a single CSV configuration.
- Startup behavior: when the menu is shown and no text set exists in app storage, the default text set is loaded automatically.
- MVVM and DI: views are constructed via dependency injection and receive their view models. View models hold presentation state and command logic; navigation is handled by a platform navigation service.
- Navigation: the navigation service supports passing runtime payloads (the results screen receives the final GameState and ScoreResult).
- UI feedback: loading operations use an IsLoading property bound to a loading overlay; gameplay shows a time/health style fill bar; incorrect decisions briefly flash the background.

## Solution structure (overview)
- App (mostly pure frontend, views, navigation, assets)
- ViewModels
- Domain (game engine, models)
- Data (DAL - csv text set repositories, highscore storage)