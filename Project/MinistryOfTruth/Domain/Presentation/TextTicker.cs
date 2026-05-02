using MinistryOfTruth.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace MinistryOfTruth.Domain.Presentation;

public class TextTicker(ITickerTextSource tickerTextSource) : ITextTicker
{
    public event EventHandler<string>? TextUpdated;

    private const int TargetLineLength = 70;
    private const int MaxVisibleLines = 3;
    private const int MaxCensoredWords = 5;
    private readonly Random _random = new();
    private readonly List<List<VisibleCharacter>> _completedLines = [];
    private readonly List<VisibleCharacter> _currentLine = [];

    private CancellationTokenSource? _tickerCancellation;
    private string _sourceText = string.Empty;
    private int _sourceIndex;
    private int _currentWordStartInCurrentLine = -1;

    public async Task StartTickerAsync()
    {
        if (_tickerCancellation is not null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_sourceText))
        {
            var rawText = await tickerTextSource.LoadTickerTextAsync();
            _sourceText = Normalize(rawText);

            if (string.IsNullOrWhiteSpace(_sourceText))
            {
                _sourceText = "To know and not to know, to be conscious of complete truthfulness while telling carefully constructed lies, to hold simultaneously two opinions which cancelled out, knowing them to be contradictory and believing in both of them, to use logic against logic ...";
            }

            _sourceIndex = _random.Next(0, _sourceText.Length);
        }

        _tickerCancellation = new CancellationTokenSource();
        _ = RunTickerAsync(_tickerCancellation.Token);
    }

    public void StopTicker()
    {
        _tickerCancellation?.Cancel();
        _tickerCancellation = null;
    }

    private async Task RunTickerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                AppendNextCharacter();
                OnTextUpdated(BuildVisibleText());

                await Task.Delay(_random.Next(35, 90), cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void AppendNextCharacter()
    {
        if (_sourceText.Length == 0)
        {
            return;
        }

        var nextChar = _sourceText[_sourceIndex];
        _sourceIndex = (_sourceIndex + 1) % _sourceText.Length;

        if (ShouldBreakLineBefore(nextChar))
        {
            FinalizeCurrentWord(_currentLine.Count - 1);
            CommitCurrentLine();

            if (char.IsWhiteSpace(nextChar))
            {
                TrimVisibleLines();
                return;
            }
        }

        _currentLine.Add(new VisibleCharacter(nextChar, false));

        if (IsWordCharacter(nextChar))
        {
            if (_currentWordStartInCurrentLine < 0)
            {
                _currentWordStartInCurrentLine = _currentLine.Count - 1;
            }
        }
        else
        {
            FinalizeCurrentWord(_currentLine.Count - 2);
        }

        TrimVisibleLines();
    }

    private bool ShouldBreakLineBefore(char nextChar)
        => _currentLine.Count >= TargetLineLength && char.IsWhiteSpace(nextChar);

    private void CommitCurrentLine()
    {
        if (_currentLine.Count == 0)
        {
            return;
        }

        _completedLines.Add([.. _currentLine]);
        _currentLine.Clear();
        _currentWordStartInCurrentLine = -1;
    }

    private void FinalizeCurrentWord(int endIndex)
    {
        if (_currentWordStartInCurrentLine < 0)
        {
            return;
        }

        if (endIndex < _currentWordStartInCurrentLine)
        {
            _currentWordStartInCurrentLine = -1;
            return;
        }

        var length = endIndex - _currentWordStartInCurrentLine + 1;
        if (length >= 5)
        {
            if (CountCensoredWords() < MaxCensoredWords && _random.NextDouble() <= 0.2)
            {
                for (var i = _currentWordStartInCurrentLine; i <= endIndex; i++)
                {
                    _currentLine[i] = _currentLine[i] with { IsCensored = true };
                }
            }
        }

        _currentWordStartInCurrentLine = -1;
    }

    private void TrimVisibleLines()
    {
        while (_completedLines.Count + (_currentLine.Count > 0 ? 1 : 0) > MaxVisibleLines)
        {
            _completedLines.RemoveAt(0);
        }
    }

    private string BuildVisibleText()
    {
        var lines = new List<string>(_completedLines.Count + (_currentLine.Count > 0 ? 1 : 0));

        foreach (var line in _completedLines)
        {
            lines.Add(ConvertLineToText(line));
        }

        if (_currentLine.Count > 0)
        {
            lines.Add(ConvertLineToText(_currentLine));
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string ConvertLineToText(IReadOnlyList<VisibleCharacter> line)
    {
        var chars = new char[line.Count];
        for (var i = 0; i < line.Count; i++)
        {
            chars[i] = line[i].IsCensored ? '█' : line[i].Character;
        }

        return new string(chars);
    }

    private int CountCensoredWords()
    {
        var count = 0;
        var inWord = false;
        var currentWordCensored = false;

        foreach (var item in EnumerateVisibleCharacters())
        {
            if (IsWordCharacter(item.Character))
            {
                if (!inWord)
                {
                    inWord = true;
                    currentWordCensored = false;
                }

                if (item.IsCensored)
                {
                    currentWordCensored = true;
                }
            }
            else
            {
                if (inWord && currentWordCensored)
                {
                    count++;
                }

                inWord = false;
                currentWordCensored = false;
            }
        }

        if (inWord && currentWordCensored)
        {
            count++;
        }

        return count;
    }

    private IEnumerable<VisibleCharacter> EnumerateVisibleCharacters()
    {
        foreach (var line in _completedLines)
        {
            foreach (var ch in line)
            {
                yield return ch;
            }

            yield return new VisibleCharacter(' ', false);
        }

        foreach (var ch in _currentLine)
        {
            yield return ch;
        }
    }

    private static bool IsWordCharacter(char character)
        => char.IsLetterOrDigit(character) || character == '\'';

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var flattened = value
            .Replace("\r", " ")
            .Replace("\n", " ");

        return Regex.Replace(flattened, "\\s+", " ").Trim();
    }

    protected virtual void OnTextUpdated(string newText)
    {
        TextUpdated?.Invoke(this, newText);
    }

    private readonly record struct VisibleCharacter(char Character, bool IsCensored);
}
