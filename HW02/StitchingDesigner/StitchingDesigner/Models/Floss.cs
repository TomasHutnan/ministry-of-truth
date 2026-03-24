namespace StitchingDesigner.Models;

public readonly struct Floss(int id, string name, string hex)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string Hex { get; } = hex;

}
