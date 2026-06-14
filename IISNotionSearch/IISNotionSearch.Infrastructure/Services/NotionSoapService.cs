using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.DTOs.Soap;
using IISNotionSearch.Application.Interfaces.Services;

namespace IISNotionSearch.Infrastructure.Services;

public class NotionSoapService : INotionSoapService
{
    private readonly INotionService _notionService;
    private readonly string _schemaPath;

    public NotionSoapService(INotionService notionService)
    {
        _notionService = notionService;
        _schemaPath = Path.Combine(AppContext.BaseDirectory, "Schemas", "notion-object-schema.xsd");
    }

    public async Task<SoapSearchResponse> Search(string term)
    {
        var response = await _notionService.SearchAsync();
        var items = response.Data?.ToList() ?? new List<NotionObjectDto>();

        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            var serializer = new XmlSerializer(typeof(List<NotionObjectDto>),
                new XmlRootAttribute("ArrayOfNotionObjectDto"));
            serializer.Serialize(writer, items);
        }

        var xmlContent = sb.ToString();
        var validationErrors = new List<string>();

        try
        {
            var schemaSet = new XmlSchemaSet();
            using (var schemaReader = XmlReader.Create(_schemaPath))
            {
                schemaSet.Add(null, schemaReader);
            }

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                Schemas = schemaSet,
                ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (_, args) => validationErrors.Add(args.Message);

            using (var validationReader = XmlReader.Create(new StringReader(xmlContent), settings))
            {
                while (validationReader.Read()) { }
            }
        }
        catch (Exception ex)
        {
            validationErrors.Add($"Strukturalna XML pogreška: {ex.Message}");
        }

        if (validationErrors.Count > 0)
        {
            return new SoapSearchResponse
            {
                Message = $"XML Validation Failed: {string.Join("; ", validationErrors)}"
            };
        }

        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        string xpath;
        if (string.IsNullOrWhiteSpace(term))
        {
            xpath = "//NotionObjectDto";
        }
        else if (term.StartsWith('/') || term.StartsWith('.'))
        {
            xpath = term;
        }
        else
        {
            xpath = $"//NotionObjectDto[contains(Title, '{term}')]";
        }

        var nodes = doc.SelectNodes(xpath);

        var resultDoc = new XmlDocument();
        var root = resultDoc.CreateElement("Results");
        
        if (nodes != null)
        {
            XmlNode lastSourceParent = null;
            XmlElement currentWrapper = null;

            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    if (node.ParentNode != null && node.ParentNode.Name == "NotionObjectDto")
                    {
                        if (node.ParentNode != lastSourceParent)
                        {
                            lastSourceParent = node.ParentNode;
                            currentWrapper = resultDoc.CreateElement("NotionObjectDto");
                            root.AppendChild(currentWrapper);
                        }

                        currentWrapper.AppendChild(resultDoc.ImportNode(node, true));
                    }
                    else
                    {
                        root.AppendChild(resultDoc.ImportNode(node, true));
                        lastSourceParent = null;
                    }
                }
                else if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.Attribute)
                {
                    var textElement = resultDoc.CreateElement("Value");
                    textElement.InnerText = node.Value ?? node.InnerText;
                    root.AppendChild(textElement);
                    lastSourceParent = null;
                }
            }
        }

        return new SoapSearchResponse
        {
            RawResponseXml = root,
            Message = nodes is { Count: > 0 } ? "OK" : "No results found for: " + term
        };
    }
}