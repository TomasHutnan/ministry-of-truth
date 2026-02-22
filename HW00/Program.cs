using PB178.Timetable;


Timetable timetable = new();

Course PB178 = new("Introduction to Development in C#/.NET", "PB178")
{
    Lecture = new Seminar("PB178", true, ["RNDr. Martin Macák, Ph.D."])
};
PB178.Lecture.AddWeeklySession(new DateTime(2026, 2, 19, 14, 0, 0), new DateTime(2026, 2, 19, 15, 50, 0), "A217", 13);

Seminar seminar = new("PB178/10", false, ["T. Cvejn", "E. Hatalčíková"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 17, 10, 0, 0), new DateTime(2026, 2, 17, 11, 50, 0), "C119", 13);
PB178.AddSeminar(seminar);

seminar = new Seminar("PB178/07", false, ["K. Švadlenková", "M. Tvarožek"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 18, 12, 0, 0), new DateTime(2026, 2, 18, 13, 50, 0), "C119", 13);
PB178.AddSeminar(seminar);

timetable.EnrollCourse(PB178);
timetable.EnrollSeminarGroup("PB178", "PB178/10");

Course IV003 = new("Algorithms II", "IV003")
{
    Lecture = new Seminar("IV003", true, ["prof. RNDr. Ivana Černá, CSc."])
};
IV003.Lecture.AddWeeklySession(new DateTime(2026, 2, 18, 14, 0, 0), new DateTime(2026, 2, 18, 15, 50, 0), "A217", 13);

seminar = new Seminar("IV003/A", false, ["N. Beneš"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 19, 16, 0, 0), new DateTime(2026, 2, 19, 17, 50, 0), "A218", 13);
IV003.AddSeminar(seminar);

seminar = new Seminar("IV003/02_CZ", false, ["M. Jonáš"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 19, 8, 0, 0), new DateTime(2026, 2, 19, 9, 50, 0), "A218", 13);
IV003.AddSeminar(seminar);

seminar = new Seminar("IV003/03_EN", false, ["M. Jonáš"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 20, 10, 0, 0), new DateTime(2026, 2, 20, 11, 50, 0), "A218", 13);
IV003.AddSeminar(seminar);

seminar = new Seminar("IV003/2EN_lecture", false, ["I. Černá"]);
seminar.AddWeeklySession(new DateTime(2026, 2, 17, 16, 0, 0), new DateTime(2026, 2, 17, 17, 50, 0), "A218", 13);
IV003.AddSeminar(seminar);

timetable.EnrollCourse(IV003);
timetable.EnrollSeminarGroup("IV003", "IV003/02_CZ");
timetable.EnrollSeminarGroup("IV003", "IV003/2EN_lecture");
