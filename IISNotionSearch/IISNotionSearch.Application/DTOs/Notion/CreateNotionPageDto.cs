namespace IISNotionSearch.Application.DTOs.Notion;

public class CreateNotionPageDto
{
    public string Title { get; set; } = null!;
    public string? Icon { get; set; }
    public string? Cover { get; set; }
}
