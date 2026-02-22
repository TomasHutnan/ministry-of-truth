using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace pb178.timetable
{
    public class Seminar
    {
        private readonly List<Session> _sessions = [];
        private readonly ReadOnlyCollection<Session> _sessionsView;

        public string Id { get; }
        public bool IsLecture { get; }

        public ImmutableArray<string> TeacherNames { get; }
        public IReadOnlyList<Session> Sessions => _sessionsView;

        public Seminar(string id, bool isLecture, string[] teacherNames)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentNullException.ThrowIfNull(teacherNames);

            Id = id;
            IsLecture = isLecture;
            TeacherNames = [.. teacherNames];

            _sessionsView = _sessions.AsReadOnly();
        }

        public void AddWeeklySession(DateTime firstSessionStart, DateTime firstSessionEnd, string? classroomId, int repetitionCount)
        {
            AddWeeklySession(new Session(firstSessionStart, firstSessionEnd, classroomId), repetitionCount);
        }

        public void AddWeeklySession(Session firstSession, int repetitionCount)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(repetitionCount, 1, nameof(repetitionCount));

            TimeSpan week = new(7, 0, 0, 0);

            _sessions.Add(firstSession);

            for (int i = 1; i < repetitionCount; i++)
            {
                _sessions.Add(new Session(firstSession.Start + week * i, firstSession.End + week * i, firstSession.ClassroomId));
            }
        }

        public void AddSession(Session session)
        {
            _sessions.Add(session);
        }
    }
}