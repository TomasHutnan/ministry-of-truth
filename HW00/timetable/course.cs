using System.Collections.ObjectModel;

namespace pb178.timetable
{
    public class Course
    {
        private Dictionary<string, Seminar> _seminars = new();
        private ReadOnlyDictionary<string, Seminar> _seminarsView;
        private Seminar? _lecture;

        public string Title { get; }
        public string Code { get; }

        public IReadOnlyDictionary<string, Seminar> Seminars => _seminarsView;
        public Seminar? Lecture 
        { 
            get
            {
                return _lecture;
            }
            set
            {
                if (value != null)
                {
                    if (value.Id != Code)
                    {
                        throw new ArgumentException("Lecture code and Course code must match.");
                    }
                    if (!value.IsLecture)
                    {
                        throw new ArgumentException("Lecture must have IsLecture set to true.");
                    }
                }
                _lecture = value;
            }
        }

        public Course(string title, string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            Title = title;
            Code = code;

            _seminarsView = _seminars.AsReadOnly();
        }

        public void AddSeminar(Seminar seminar)
        {
            ArgumentNullException.ThrowIfNull(seminar);

            if (_seminars.ContainsKey(seminar.Id))
            {
                throw new ArgumentException($"Course already has a seminar with id {seminar.Id}.", nameof(seminar));
            }
            if (!seminar.Id.StartsWith(Code + "/"))
            {
                throw new ArgumentException("Seminar id must start with <course code>/");
            }
            if (seminar.IsLecture)
            {
                throw new ArgumentException("Seminars must have IsLecture set to false.");
            }

            _seminars.Add(seminar.Id, seminar);
        }

        public void RemoveSeminar(string id)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);

            if (!_seminars.ContainsKey(id))
            {
                throw new ArgumentException($"Seminar with id {id} does not exist.", nameof(id));
            }

            _seminars.Remove(id);
        }
    }
}