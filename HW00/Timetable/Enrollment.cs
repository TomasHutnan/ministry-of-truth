namespace PB178.Timetable
{
    public record Enrollment(Course Course)
    {
        public List<string> SeminarIds { get; } = [];
    }
}