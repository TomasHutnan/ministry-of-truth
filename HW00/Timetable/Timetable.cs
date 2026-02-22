namespace PB178.Timetable
{
    public class Timetable
    {
        private readonly Dictionary<string, Enrollment> _enrollments = [];

        public List<Seminar> GetSeminars()
        {
            List<Seminar> seminars = [];

            foreach (Enrollment enrollment in _enrollments.Values)
            {
                Course course = enrollment.Course;

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
            if (enrollment.Course.Seminars.ContainsKey(seminarId))
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