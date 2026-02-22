namespace PB178.Timetable
{
    public class Timetable
    {
        private readonly Dictionary<string, Enrollment> _enrollments = [];

        public IReadOnlyList<Seminar> GetSeminars()
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

            return seminars.AsReadOnly();
        }

        public void EnrollCourse(Course course)
        {
            ArgumentNullException.ThrowIfNull(course);
            if (_enrollments.ContainsKey(course.Code))
            {
                throw new ArgumentException($"Course with code {course.Code} is already enrolled.");
            }

            _enrollments.Add(course.Code, new Enrollment(course));
        }

        public void UnenrollCourse(string courseId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(courseId);
            if (!_enrollments.ContainsKey(courseId))
            {
                throw new ArgumentException($"No course with code {courseId} is enrolled.");
            }

            _enrollments.Remove(courseId);
        }

        public void EnrollSeminarGroup(string courseId, string seminarId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(courseId);
            ArgumentException.ThrowIfNullOrWhiteSpace(seminarId);

            if (!_enrollments.TryGetValue(courseId, out var enrollment))
            {
                throw new ArgumentException($"No course with code {courseId} is enrolled.");
            }
            enrollment.AddSeminar(seminarId);
        }

        public void UnenrollSeminarGroup(string courseId, string seminarId)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(courseId);
            ArgumentException.ThrowIfNullOrWhiteSpace(seminarId);

            if (!_enrollments.TryGetValue(courseId, out var enrollment))
            {
                throw new ArgumentException($"No course with code {courseId} is enrolled.");
            }
            enrollment.RemoveSeminar(seminarId);
        }
    }
}