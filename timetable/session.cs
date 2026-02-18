namespace pb178.timetable
{
    public readonly struct Session(DateTime start, DateTime end, string? classroomId)
    {
        public DateTime Start { get; } = start;
        public DateTime End { get; } = end;
        public string? ClassroomId { get; } = classroomId;
    }
}