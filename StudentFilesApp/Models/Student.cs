using System;
using System.Collections.Generic;

namespace StudentFilesApp.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; } 
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; }
        public string? Citizenship { get; set; } 
        public string? Address { get; set; } 
        public string? Phone { get; set; } 
        public string? Email { get; set; } 
        public string Status { get; set; } = "обучается";
        public DateTime EnrollmentDate { get; set; }
        public string? PhotoPath { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<OrderStudent> OrderStudents { get; set; } = new List<OrderStudent>();

        public string FullName => $"{LastName} {FirstName} {MiddleName}";
    }
}