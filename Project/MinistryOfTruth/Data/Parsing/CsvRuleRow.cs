using CsvHelper.Configuration.Attributes;

namespace MinistryOfTruth.Data.Parsing;

public class CsvRuleRow
{
    [Name("id")]
    public required string Id { get; set; }
    [Name("keyword")]
    public required string Keyword { get; set; }
    [Name("number")]
    public required string Number { get; set; }
}
