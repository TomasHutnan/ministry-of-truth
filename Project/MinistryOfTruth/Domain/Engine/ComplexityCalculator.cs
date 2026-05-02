using System.Collections.Immutable;

namespace MinistryOfTruth.Domain.Engine;

public class ComplexityCalculator
{
    private ImmutableHashSet<char> _sentenceBoundaries = ['.', '!', '?'];
    private ImmutableHashSet<char> _punctuationMarks = [',', ';', ':', '(', ')', '—', '-'];

    private const int _complexityGroupCount = 5;
    public int[] ComplexityGroupsFromLeastComplex;

    public ComplexityCalculator()
    {
        ComplexityGroupsFromLeastComplex = new int[_complexityGroupCount];
        for (int i = 1; i < _complexityGroupCount; i++)
        {
            ComplexityGroupsFromLeastComplex[i] = i + 1;
        }
    }

    public int CalculateGroup(string text)
    {
        int totalWords = 0;
        int totalWordChars = 0;
        int totalSentences = 0;
        int punctCount = 0;
        int nonAlphaGroupCount = 0;
        int capitalizedWordCount = 0;

        bool inWord = false;
        bool inNonAlpha = false;
        int currentWordChars = 0;
        bool wordStartsWithCapital = false;

        // Scan
        foreach (char c in text)
        {
            bool isPunctuationMark = _punctuationMarks.Contains(c);
            bool isSentenceBoundary = !isPunctuationMark && _sentenceBoundaries.Contains(c);

            if (c == ' ' || isPunctuationMark || isSentenceBoundary)  // Word boundary
            {
                if (inWord)
                {
                    totalWords++;
                    totalWordChars += currentWordChars;
                    if (wordStartsWithCapital)
                    {
                        capitalizedWordCount++;
                    }
                }
                else if (inNonAlpha)
                {
                    nonAlphaGroupCount++;
                }

                inWord = false;
                inNonAlpha = false;
                currentWordChars = 0;
                wordStartsWithCapital = false;

                if (isPunctuationMark)
                {
                    punctCount++;
                }
                else if (isSentenceBoundary)
                {
                    totalSentences++;
                }
            }
            else if (char.IsLetter(c))
            {
                if (!inWord)
                {
                    inWord = true;
                    wordStartsWithCapital = char.IsUpper(c);
                }
                currentWordChars++;
            }
            else
            {
                inWord = false;
                inNonAlpha = true;
            }
        }

        // Include last word
        if (inWord)
        {
            totalWords++;
            totalWordChars += currentWordChars;
            if (wordStartsWithCapital)
            {
                capitalizedWordCount++;
            }
        }
        else if (inNonAlpha)
        {
            nonAlphaGroupCount++;
        }

        // Normalize
        int avgWordLen = totalWords == 0 ? 0 : totalWordChars / totalWords;
        int avgSentenceLen = totalSentences == 0 ? 0 : totalWords / totalSentences;
        int punctDensity = totalWords == 0 ? 0 : punctCount / totalWords;
        int numDensity = totalWords == 0 ? 0 : (nonAlphaGroupCount + capitalizedWordCount) / totalWords;
        int wordCount = totalWords;

        // Clamp
        double A = Math.Clamp((avgWordLen - 3) / 9, 0, 1);         // 3–12 chars
        double S = Math.Clamp((avgSentenceLen - 5) / 35, 0, 1);      // 5–40 words
        double P = Math.Clamp(punctDensity / 0.6, 0, 1);            // dense prose
        double N = Math.Clamp(numDensity / 0.4, 0, 1);              // numbers/caps
        double W = Math.Clamp((wordCount - 8) / 60, 0, 1);           // volume

        // Get group
        double complexity =
            0.35 * A +
            0.20 * S +
            0.15 * P +
            0.10 * N +
            0.20 * W;

        int group = (int)Math.Ceiling(complexity * _complexityGroupCount);
        return Math.Clamp(group, 1, 5);
    }
}
