using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Domain.Enums;
using System.Diagnostics;

namespace MinistryOfTruth.Domain.Engine;

public class GameEngine(IDocumentGenerator documentGenerator) : IGameEngine
{
    private const double _millisecondsPerFrame = 1000D / 60D;
    private const double _passiveDangerGrowth = 1D / 120;  // Fills up the whole meter in two minutes
    private const double _dailyDangerDecline = 0.2D;

    private const double _incorrectApproveDangerGrowth = 0.10D;
    private const double _incorrectApproveDangerGrowthRatio = 1.10D;
    private const double _incorrectCensorDangerGrowth = 0.15D;
    private const double _incorrectCensorDangerGrowthRatio = 1.15D;

    private IDocumentGenerator _documentGenerator = documentGenerator;

    private readonly Lock _stateLocker = new();
    private bool _isRunning = false;
    private CancellationTokenSource? _gameLoopCancelation;

    private double _dangerRatio;

    private ScoreResult _scoreResult = new();
    private DayPackage? _dayPackage;
    private int _currentDay;

    private TextEntry? _currentText;
    private LastAction? _lastAction;
    private bool? _lastDecisionWasCorrect;

    private bool _isProcessing = false;

    public event EventHandler<EventArgs>? GameStarted;
    public event EventHandler<ScoreResult>? GameEnded;
    public event EventHandler<GameState>? GameStateChanged;

    protected virtual void OnGameStarted()
    {
        GameStarted?.Invoke(this, new EventArgs());
    }

    protected virtual void OnGameEnded(ScoreResult result)
    {
        GameEnded?.Invoke(this, result);
    }

    protected virtual void OnGameStateChanged(GameState newGameState)
    {
        GameStateChanged?.Invoke(this, newGameState);
    }

    public async Task StartGameLoop()
    {
        lock (_stateLocker)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Game is already running.");
            }
            _isRunning = true;
        }

        try
        {
            await _documentGenerator.InitializeAsync();

            lock (_stateLocker)
            {
                _dangerRatio = 0;
                _scoreResult = new ScoreResult();
                _currentDay = 1;
            }

            StartDay();

            _gameLoopCancelation = new CancellationTokenSource();

            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(_millisecondsPerFrame));
            var stopwatch = Stopwatch.StartNew();
            double lastTick = stopwatch.ElapsedMilliseconds;

            OnGameStarted();

            while (await timer.WaitForNextTickAsync(_gameLoopCancelation.Token))
            {
                double currentTick = stopwatch.ElapsedMilliseconds;
                double deltaTime = (currentTick - lastTick) / 1000D;
                lastTick = currentTick;

                Update(deltaTime);
            }
        }
        finally
        {
            lock (_stateLocker)
            {
                _isRunning = false;
            }
            _gameLoopCancelation?.Dispose();
        }
    }

    public void Approve()
    {
        lock (_stateLocker)
        {
            if (!_isProcessing)
            {
                _lastAction = LastAction.Approve;
            }
        }
    }

    public void Censor()
    {
        lock (_stateLocker)
        {
            if (!_isProcessing)
            {
                _lastAction = LastAction.Censor;
            }
        }
    }

    private void Update(double deltaTime)
    {
        lock (_stateLocker)
        {
            _isProcessing = true;
            _dangerRatio += deltaTime * _passiveDangerGrowth;

            ResolveLastAction();

            if (_dangerRatio >= 1)
            {
                EndGame();
            }

            PublishState();
            _isProcessing = false;
        }
    }

    private void ResolveLastAction()
    {
        if (_lastAction == null)
        {
            _lastDecisionWasCorrect = null;
            return;
        }

        if (_lastAction == LastAction.Approve)
        {
            if (_dayPackage is not null && _currentText is not null && !_dayPackage.ViolationIds.Contains(_currentText.Id))
            {
                _scoreResult = _scoreResult with { CorrectApprovals = _scoreResult.CorrectApprovals + 1 };
                _lastDecisionWasCorrect = true;
            }
            else
            {
                _dangerRatio = _dangerRatio * _incorrectApproveDangerGrowthRatio + _incorrectApproveDangerGrowth;
                _scoreResult = _scoreResult with { IncorrectApprovals = _scoreResult.IncorrectApprovals + 1 };
                _lastDecisionWasCorrect = false;
            }
        }
        else
        {
            if (_dayPackage is not null && _currentText is not null && _dayPackage.ViolationIds.Contains(_currentText.Id))
            {
                _scoreResult = _scoreResult with { CorrectCensors = _scoreResult.CorrectCensors + 1 };
                _lastDecisionWasCorrect = true;
            }
            else
            {
                _dangerRatio = _dangerRatio * _incorrectCensorDangerGrowthRatio + _incorrectCensorDangerGrowth;
                _scoreResult = _scoreResult with { IncorrectCensors = _scoreResult.IncorrectCensors + 1 };
                _lastDecisionWasCorrect = false;
            }
        }

        _lastAction = null;
        if (_dayPackage is not null && _dayPackage.DocumentStack.Count > 0)
        {
            NextText();
        }
        else
        {
            EndDay();
            StartDay();
        }
    }

    private void StartDay()
    {
        _dayPackage = _documentGenerator.CreateDayPackage(_currentDay, 3);
        NextText();
    }

    private void NextText()
    {
        if (_dayPackage is not null && _dayPackage.DocumentStack.Count > 0)
        {
            _currentText = _dayPackage.DocumentStack.Dequeue();
        }
    }

    private void EndDay()
    {
        _dangerRatio = _dangerRatio > _dailyDangerDecline ? _dangerRatio - _dailyDangerDecline : 0;
        _currentDay++;
    }

    private void EndGame()
    {
        if (_gameLoopCancelation != null)
        {
            _gameLoopCancelation.Cancel();

            OnGameEnded(_scoreResult);
        }
    }

    private void PublishState()
    {
        if (_currentText is null || _dayPackage is null)
        {
            return;
        }

        OnGameStateChanged(new GameState(
            _currentText.Content,
            _dayPackage.Rule.Keyword,
            _currentDay,
            _scoreResult.Score,
            _dayPackage.DocumentStack.Count + 1,
            _dangerRatio,
            _lastDecisionWasCorrect,
            ""));
    }
}
