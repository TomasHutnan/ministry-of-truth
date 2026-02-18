namespace pb178.timetable
{
    public record Enrollment(Course Course)
    {
        public List<string> SeminarIds { get; } = [];
    }
}