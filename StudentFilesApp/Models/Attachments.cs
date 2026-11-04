using StudentFilesApp.Models;

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