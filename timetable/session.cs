namespace pb178.timetable
{
    public readonly struct Session(DateTime start, DateTime end, string? classroomId)
    {
        public readonly DateTime Start = start; 
        public readonly DateTime End = end;
        public readonly string? ClassroomId = classroomId;
    }
}