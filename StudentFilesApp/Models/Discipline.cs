using System;

namespace StudentFilesApp.Models
{
    public class Discipline
    {
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public int TotalHours { get; set; }
        public int Semester { get; set; }
        public string Teacher { get; set; }
    }

    public class StudentDiscipline
    {
        public int StudentDisciplineId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public int? GradeValue { get; set; }
        public DateTime? GradeDate { get; set; }
        public bool IsDebt { get; set; } = false;
    }
}