using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.DTOs.Soap;
using IISNotionSearch.Application.Interfaces.Services;

namespace IISNotionSearch.Infrastructure.Services;

public class NotionSoapService : INotionSoapService
{
    private readonly INotionService _notionService;

    public NotionSoapService(INotionService notionService)
    {
        _notionService = notionService;
    }

    public async Task<SoapSearchResponse> Search(string term)
    {
        var response = await _notionService.SearchAsync();
        var items = response.Data?.ToList() ?? new List<NotionObjectDto>();

        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            var serializer = new XmlSerializer(typeof(List<NotionObjectDto>),
                new XmlRootAttribute("NotionObjects"));
            serializer.Serialize(writer, items);
        }

        var doc = new XmlDocument();
        doc.LoadXml(sb.ToString());

        var xpath = string.IsNullOrWhiteSpace(term)
            ? "//NotionObjectDto"
            : $"//NotionObjectDto[contains(., '{term}')]";

        var nodes = doc.SelectNodes(xpath);
        var results = new List<SoapNotionObject>();
        if (nodes != null)
        {
            foreach (XmlNode node in nodes)
            {
                using var reader = new StringReader(node.OuterXml);
                var dto = (NotionObjectDto)new XmlSerializer(typeof(NotionObjectDto)).Deserialize(reader)!;
                results.Add(new SoapNotionObject
                {
                    NotionId = dto.NotionId,
                    ObjectType = dto.ObjectType,
                    Title = dto.Title,
                    Url = dto.Url,
                    Icon = dto.Icon,
                    Cover = dto.Cover,
                    CreatedTime = dto.CreatedTime,
                    LastEditedTime = dto.LastEditedTime,
                    InTrash = dto.InTrash
                });
            }
        }

        return new SoapSearchResponse
        {
            Results = results,
            Message = results.Count > 0 ? "OK" : "No results found"
        };
    }
}
