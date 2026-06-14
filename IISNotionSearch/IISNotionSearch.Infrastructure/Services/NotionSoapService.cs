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
    private readonly XmlSchemaSet _xmlSchemaSet;

    private static readonly XmlSerializer ListSerializer = new(typeof(List<NotionObjectDto>), new XmlRootAttribute("ArrayOfNotionObjectDto"));

    public NotionSoapService(INotionService notionService)
    {
        _notionService = notionService;
        
        _xmlSchemaSet = new XmlSchemaSet();
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Schemas", "notion-object-schema.xsd");
        using var schemaReader = XmlReader.Create(schemaPath);
        _xmlSchemaSet.Add(null, schemaReader);
    }

    public async Task<SoapSearchResponse> Search(string term)
    {
        var response = await _notionService.SearchAsync();
        var items = response.Data?.ToList() ?? [];

        var sb = new StringBuilder();
        using (var writer = new StringWriter(sb))
        {
            ListSerializer.Serialize(writer, items);
        }
        var xmlContent = sb.ToString();

        var validationErrors = new List<string>();
        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = _xmlSchemaSet,
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            Async = true
        };
        settings.ValidationEventHandler += (_, args) => validationErrors.Add(args.Message);

        try
        {
            using var validationReader = XmlReader.Create(new StringReader(xmlContent), settings);
            while (await validationReader.ReadAsync()) { }
        }
        catch (Exception ex)
        {
            validationErrors.Add($"XML Structural Error: {ex.Message}");
        }

        if (validationErrors.Count > 0)
        {
            return new SoapSearchResponse { Message = $"XML Validation Failed: {string.Join("; ", validationErrors)}" };
        }

        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        var xpath = ResolveXPath(term);
        var nodes = doc.SelectNodes(xpath);

        var resultDoc = new XmlDocument();
        var root = resultDoc.CreateElement("Results");

        if (nodes == null || nodes.Count == 0)
        {
            return new SoapSearchResponse { RawResponseXml = root, Message = $"No results found for: {term}" };
        }
        
        ProcessXmlNodes(nodes, resultDoc, root);

        return new SoapSearchResponse { RawResponseXml = root, Message = "OK" };
    }

    #region Private Helpers

    private static string ResolveXPath(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return "//NotionObjectDto";
        if (term.StartsWith('/') || term.StartsWith('.')) return term;
        return $"//NotionObjectDto[contains(Title, '{term}')]";
    }

    private static void ProcessXmlNodes(XmlNodeList nodes, XmlDocument resultDoc, XmlElement root)
    {
        XmlNode lastSourceParent = null;
        XmlElement currentWrapper = null;

        foreach (XmlNode node in nodes)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element when node.ParentNode?.Name == "NotionObjectDto":
                    if (node.ParentNode != lastSourceParent)
                    {
                        lastSourceParent = node.ParentNode;
                        currentWrapper = resultDoc.CreateElement("NotionObjectDto");
                        root.AppendChild(currentWrapper);
                    }
                    currentWrapper.AppendChild(resultDoc.ImportNode(node, true));
                    break;

                case XmlNodeType.Element:
                    root.AppendChild(resultDoc.ImportNode(node, true));
                    lastSourceParent = null;
                    break;

                case XmlNodeType.Text:
                case XmlNodeType.Attribute:
                    var textElement = resultDoc.CreateElement("Value");
                    textElement.InnerText = node.Value ?? node.InnerText;
                    root.AppendChild(textElement);
                    lastSourceParent = null;
                    break;
            }
        }
    }

    #endregion
}