namespace pb178.timetable
{
    class Seminar(string id, bool isLecture, string[] teacherNames)
    {
        public readonly string Id = id;
        public readonly bool IsLecture = isLecture;
        public string[] TeacherNames = teacherNames;
        public List<Session> Sessions = new List<Session>();

        public void AddWeeklySession(DateTime firstSessionStart, DateTime firstSessionEnd, string? classroomId, int repetiotionCount)
        {
            TimeSpan week = new TimeSpan(7, 0, 0, 0);

            for (int i = 0; i < repetiotionCount; i++)
            {
                Sessions.Add(new Session(firstSessionStart + week * i, firstSessionEnd + week * i, classroomId));
            }
        }
    }
}