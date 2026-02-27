namespace Expedition178.Utils
{
    public static class NameGenerator
    {
        private static readonly string[] NameAdjectives = new[]
        {
            "Brave", "Cunning", "Daring", "Eager", "Fierce", "Gallant", "Heroic", "Intrepid", "Jovial", "Keen",
            "Loyal", "Mighty", "Noble", "Optimistic", "Powerful", "Quick", "Resilient", "Steadfast", "Tenacious",
            "Valiant", "Wise"
        };
        private static readonly string[] NamePrefixes = new[]
        {
            "Ar", "Bel", "Cal", "Dor", "El", "Fen", "Gal", "Hal", "Il", "Jar",
            "Kel", "Lor", "Mor", "Nel", "Or", "Pel", "Quel", "Ral", "Sel", "Tor",
            "Ul", "Val", "Wel", "Xan", "Yel", "Zor"
        };
        private static readonly string[] NameSuffixes = new[]
        {
            "an", "bel", "cor", "dor", "el", "fen", "gal", "hal", "il", "jar",
            "kel", "lor", "mor", "nel", "or", "pel", "quel", "ral", "sel", "tor",
            "ul", "val", "wel", "xan", "yel", "zor"
        };
        public static string GenerateName()
        {
            string adjective = NameAdjectives[Random.Shared.Next(NameAdjectives.Length)];
            string prefix = NamePrefixes[Random.Shared.Next(NamePrefixes.Length)];
            string suffix = NameSuffixes[Random.Shared.Next(NameSuffixes.Length)];
            return adjective + " " + prefix + suffix;
        }
    }
}
