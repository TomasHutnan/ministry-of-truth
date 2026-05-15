namespace MinistryOfTruth.Domain.Interfaces;

public interface ITextSetLoader
{
    Task LoadFromFileAsync(string filePath);
    Task LoadDefaultAsync();
    Task ResetToDefaultAsync();
}
