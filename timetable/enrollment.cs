namespace pb178.timetable
{
    record Enrollment(Course course)
    {
        public Course Course = course;
        public List<string> SeminarIds = new List<string>();
    }
}