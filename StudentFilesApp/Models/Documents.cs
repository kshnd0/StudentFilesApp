using System;

namespace StudentFilesApp.Models
{
    public class DocumentType
    {
        public int DocumentTypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class Attachment
    {
        public int AttachmentId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentType DocumentType { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class CertificateType
    {
        public int CertificateTypeId { get; set; }
        public string TypeName { get; set; }
    }

    public class Certificate
    {
        public int CertificateId { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int CertificateTypeId { get; set; }
        public CertificateType CertificateType { get; set; }
        public int? DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public DateTime IssueDate { get; set; }
    }
}