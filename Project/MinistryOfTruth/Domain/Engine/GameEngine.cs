using MinistryOfTruth.Domain.Interfaces;
using MinistryOfTruth.Domain.Models;
using MinistryOfTruth.Domain.Enums;
using System.Diagnostics;

namespace MinistryOfTruth.Domain.Engine;

public class GameEngine(IDocumentGenerator documentGenerator) : IGameEngine
{
    private const double _millisecondsPerFrame = 1000D / 60D;
    private const double _passiveDangerGrowth = 1D / (120 * 1000);  // Fills up the whole meter in two minutes

    private const double _incorrectApproveDangerGrowth = 0.10D;
    private const double _incorrectApproveDangerGrowthRatio = 1.10D;
    private const double _incorrectCensorDangerGrowth = 0.15D;
    private const double _incorrectCensorDangerGrowthRatio = 1.15D;

    private const double _dayOneRoundTime = 1000D * 60;
    private const double _absoluteDailyRoundTimeDecrese = 1000D * 5;
    private const double _minimalRoundTime = 1000D * 20;

    private IDocumentGenerator _documentGenerator = documentGenerator;

    private static readonly object locker = new object();
    private bool _isRunning = false;
    private CancellationTokenSource? _gameLoopCancelation;

    private double _totalRoundTime;
    private double _currentRoundTime;

    private double _dangerRatio;

    private ScoreResult _scoreResult = new ScoreResult();
    private DayPackage? _dayPackage;
    private int _currentDay;

    private TextEntry? _currentText;
    private LastAction? _lastAction;

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
        lock (locker)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Game is already running.");
            }
            _isRunning = true;
        }

        await documentGenerator.InitializeAsync();

        _dangerRatio = 0;
        _scoreResult = new ScoreResult();
        _currentDay = 1;
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

    public void Approve()
    {
        if (!_isProcessing)
        {
            _lastAction = LastAction.Approve;
        }
    }

    public void Censor()
    {
        if (!_isProcessing)
        {
            _lastAction = LastAction.Censor;
        }
    }

    private void Update(double deltaTime)
    {
        _isProcessing = true;
        _currentRoundTime += deltaTime;
        _dangerRatio += deltaTime * _passiveDangerGrowth * 1000;

        ResolveLastAction();

        if (_currentRoundTime >= _totalRoundTime)
        {
            FailRemainingTexts();
        }

        if (_dangerRatio >= 1)
        {
            EndGame();
        }
        else if (_currentRoundTime >= _totalRoundTime)
        {
            EndDay();
            StartDay();
        }

        PublishState();
        _isProcessing = false;
    }

    private void ResolveLastAction()
    {
        if (_lastAction == null)
        {
            return;
        }

        if (_lastAction == LastAction.Approve)
        {
            if (!_dayPackage!.ViolationIds.Contains(_currentText!.Id))
            {
                _scoreResult = _scoreResult with { CorrectApprovals = _scoreResult.CorrectApprovals + 1 };
            }
            else
            {
                _dangerRatio = _dangerRatio * _incorrectApproveDangerGrowthRatio + _incorrectApproveDangerGrowth;
                _scoreResult = _scoreResult with { IncorrectApprovals = _scoreResult.IncorrectApprovals + 1 };
            }
        }
        else
        {
            if (_dayPackage!.ViolationIds.Contains(_currentText!.Id))
            {
                _scoreResult = _scoreResult with { CorrectCensors = _scoreResult.CorrectCensors + 1 };
            }
            else
            {
                _dangerRatio = _dangerRatio * _incorrectCensorDangerGrowthRatio + _incorrectCensorDangerGrowth;
                _scoreResult = _scoreResult with { IncorrectCensors = _scoreResult.IncorrectCensors + 1 };
            }
        }

        _lastAction = null;
        if (_dayPackage.DocumentStack.Count > 0)
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
        _totalRoundTime = Math.Max(_dayOneRoundTime - (_currentDay - 1) * _absoluteDailyRoundTimeDecrese, _minimalRoundTime);
        _dayPackage = _documentGenerator.CreateDayPackage(_currentDay, 3);

        NextText();
    }

    private void NextText()
    {
        _currentText = _dayPackage!.DocumentStack.Dequeue();
    }

    private void EndDay()
    {
        _currentDay++;
    }

    private void FailRemainingTexts()
    {
        _scoreResult = _scoreResult with { Missed = _scoreResult.Missed + _dayPackage!.DocumentStack.Count };
        _dayPackage.DocumentStack.Clear();
    }

    private void EndGame()
    {
        if (_gameLoopCancelation != null)
        {
            _gameLoopCancelation!.Cancel();
            _isRunning = false;

            OnGameEnded(_scoreResult);
        }
    }

    private void PublishState()
    {
        OnGameStateChanged(new GameState(
            _currentText!.Content,
            _dayPackage!.RuleDescription,
            _currentDay,
            _scoreResult.Score,
            _dayPackage!.DocumentStack.Count + 1,
            _currentRoundTime / _totalRoundTime,
            _dangerRatio,
            true,
            ""));
    }
}
