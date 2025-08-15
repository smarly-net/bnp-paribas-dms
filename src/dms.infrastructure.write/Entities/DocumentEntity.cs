namespace DMS.Infrastructure.Write.Entities;

public class DocumentEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
}