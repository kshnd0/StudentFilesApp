namespace StudentFilesApp.Models
{
    public class Parent
    {
        public int ParentId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Relation { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}