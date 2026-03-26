using CsvHelper.Configuration.Attributes;

namespace StitchingDesigner.Models;

public class Floss
{
    private string _hex;

    [Name("floss")]
    public required string Id { get; set; }

    [Name("name")]
    public required string Name { get; set; }

    [Name("r")]
    public required int R { get; set; }

    [Name("g")]
    public required int G { get; set; }

    [Name("b")]
    public required int B { get; set; }

    [Name("hex")]
    public required string Hex
    { 
        get => _hex; 
        set
        {
            try
            {
                DisplayColor = Color.FromArgb(value);
            }
            catch
            {
                DisplayColor = Colors.White;
            }
        }
    }

    public Color DisplayColor { get; private set; } = Colors.White;
}
