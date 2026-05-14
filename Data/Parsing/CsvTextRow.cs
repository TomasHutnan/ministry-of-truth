using CsvHelper.Configuration.Attributes;

namespace MinistryOfTruth.Data.Parsing;

public class CsvTextRow
{
    [Name("id")]
    public required string Id { get; set; }
    [Name("content")]
    public required string Content { get; set; }
}
