namespace pb178.timetable
{
    class Timetable
    {
        private Dictionary<string, Enrollment> _enrollments = new Dictionary<string, Enrollment>();

        public List<Seminar> GetSeminars()
        {
            List<Seminar> seminars = new List<Seminar>();

            foreach (Enrollment enrollment in _enrollments.Values)
            {
                Course course = enrollment.course;

                if (course.Lecture != null)
                {
                    seminars.Add(course.Lecture);
                }

                foreach (string seminarId in enrollment.SeminarIds)
                {
                    seminars.Add(course.Seminars[seminarId]);
                }
            }

            return seminars;
        }

        public void EnrollCourse(Course course)
        {
            _enrollments.Add(course.Code, new Enrollment(course));
        }
        public void UnenrollCourse(string courseId)
        {
            _enrollments.Remove(courseId);
        }

        public void EnrollSeminarGroup(string courseId, string seminarId)
        {
            Enrollment enrollment = _enrollments[courseId];
            if (enrollment.course.Seminars.ContainsKey(seminarId))
            {
                enrollment.SeminarIds.Add(seminarId);
            }
        }
        public void UnenrollSeminarGroup(string courseId, string seminarId)
        {
            _enrollments[courseId].SeminarIds.Remove(seminarId);
        }
    }
}