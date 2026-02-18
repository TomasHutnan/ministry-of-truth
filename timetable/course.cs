namespace pb178.timetable
{
    public class Course(string title, string code)
    {
        public string Title { init; get; } = title;
        public string Code { init; get; } = code;
        public Dictionary<string, Seminar> Seminars = new Dictionary<string, Seminar>();
        public Seminar? Lecture = null;
    }
}