namespace PB178.Timetable
{
    public record Enrollment
    {
        private readonly HashSet<string> _seminarIds = [];

        public Course Course { get; }
        public IReadOnlySet<string> SeminarIds => _seminarIds;

        public Enrollment(Course course)
        {
            ArgumentNullException.ThrowIfNull(course);
            Course = course;
        }

        public void AddSeminar(string seminarId)
        {
            if (!Course.Seminars.ContainsKey(seminarId))
            {
                throw new ArgumentException($"Seminar with id {seminarId} does not exist.", nameof(seminarId));
            }
            _seminarIds.Add(seminarId);
        }

        public void RemoveSeminar(string seminarId)
        {
            if (!Course.Seminars.ContainsKey(seminarId))
            {
                throw new ArgumentException($"Seminar with id {seminarId} does not exist.", nameof(seminarId));
            }
            if (!_seminarIds.Contains(seminarId))
            {
                throw new ArgumentException($"Seminar with id {seminarId} is not enrolled.", nameof(seminarId));
            }
            _seminarIds.Remove(seminarId);
        }
    }
}