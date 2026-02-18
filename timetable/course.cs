namespace pb178.timetable
{
    public class Course(string title, string code)
    {
        public string Title { get; init; } = title;
        public string Code { get; init; } = code;
        public Dictionary<string, Seminar> Seminars { get; } = new();
        public Seminar? Lecture { get; set; }
    }
}