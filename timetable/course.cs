namespace pb178.timetable
{
    class Course
    {
        public string Title { init; get; }
        public string Code { init; get; }
        public Dictionary<string, Seminar> Seminars = new Dictionary<string, Seminar>();
        public Seminar? Lecture = null;

        public Course(string title, string code)
        {
            Title = title;
            Code = code;
        }
    }
}