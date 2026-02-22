namespace PB178.Timetable
{
    public readonly struct Session
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public string? ClassroomId { get; }

        public Session(DateTime start, DateTime end, string? classroomId)
        {
            if (end < start)
            {
                throw new ArgumentException("End must be greater than or equal to start.", nameof(end));
            }

            Start = start;
            End = end;
            ClassroomId = classroomId;
        }
    }
}