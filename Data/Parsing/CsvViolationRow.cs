using CsvHelper.Configuration.Attributes;

namespace MinistryOfTruth.Data.Parsing;

public class CsvViolationRow
{
    [Name("text_id")]
    public required string TextId { get; set; }
    [Name("rule_id")]
    public required string RuleId { get; set; }
    [Name("justification")]
    public required string Justification { get; set; }
}
