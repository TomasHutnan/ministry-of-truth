namespace pb178.timetable
{
    public class Seminar(string id, bool isLecture, string[] teacherNames)
    {
        public readonly string Id = id;
        public readonly bool IsLecture = isLecture;
        public string[] TeacherNames = teacherNames;
        public List<Session> Sessions = [];

        public void AddWeeklySession(DateTime firstSessionStart, DateTime firstSessionEnd, string? classroomId, int repetitionCount)
        {
            TimeSpan week = new(7, 0, 0, 0);

            for (int i = 0; i < repetitionCount; i++)
            {
                Sessions.Add(new Session(firstSessionStart + week * i, firstSessionEnd + week * i, classroomId));
            }
        }
    }
}