using CsvHelper.Configuration.Attributes;

namespace StitchingDesigner.Models;

public class Floss
{
    private string _hex = "ffffff";
    private string _name = "";
    private string _id = "";

    [Name("floss")]
    public required string Id
    { 
        get => _id; 
        set
        {
            _id = value;
            Description = $"{value} | {Name}";
        }
    }

    [Name("name")]
    public required string Name
    {
        get => _name;
        set
        {
            _name = value;
            Description = $"{Id} | {value}";
        }
    }

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
            _hex = value;
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
    public string Description { get; private set; } = "";
}
