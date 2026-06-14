using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using IISNotionSearch.Application.DTOs.Notion;
using IISNotionSearch.Application.Interfaces.Services;
using IISNotionSearch.Application.Mappers;
using IISNotionSearch.Application.Models;
using IISNotionSearch.Domain.Interfaces;
using Json.Schema;

namespace IISNotionSearch.Infrastructure.Services;

public class ImportService : IImportService
{
    private readonly INotionObjectRepository _repository;
    
    private readonly XmlSchemaSet _xmlSchemaSet;
    private readonly JsonSchema _jsonSchema;

    private static readonly XmlSerializer ArraySerializer = new(typeof(NotionObjectDto[]));
    private static readonly XmlSerializer SingleSerializer = new(typeof(NotionObjectDto));

    public ImportService(INotionObjectRepository repository)
    {
        _repository = repository;
        var schemaDir = Path.Combine(AppContext.BaseDirectory, "Schemas");

        _xmlSchemaSet = new XmlSchemaSet();
        using (var schemaReader = XmlReader.Create(Path.Combine(schemaDir, "notion-object-schema.xsd")))
        {
            _xmlSchemaSet.Add(null, schemaReader);
        }

        var jsonSchemaPath = Path.Combine(schemaDir, "notion-object-schema.json");
        var schemaText = File.ReadAllText(jsonSchemaPath);
        _jsonSchema = JsonSchema.FromText(schemaText);
    }

    public async Task<StandardResponse<List<NotionObjectDto>>> ImportXmlAsync(string xmlContent)
    {
        var errors = new List<string>();

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = _xmlSchemaSet,
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings,
            Async = true
        };

        settings.ValidationEventHandler += (_, args) => errors.Add(args.Message);

        List<NotionObjectDto> dtos = [];
        try
        {
            using (var validationReader = XmlReader.Create(new StringReader(xmlContent), settings))
            {
                while (await validationReader.ReadAsync()) { }
            }

            if (errors.Count > 0)
            {
                return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                    errors: errors, message: "XML Schema validation failed");
            }

            var rootName = GetXmlRootName(xmlContent);
            using var contentReader = XmlReader.Create(new StringReader(xmlContent));

            if (string.Equals(rootName, "ArrayOfNotionObjectDto", StringComparison.OrdinalIgnoreCase))
            {
                dtos = (ArraySerializer.Deserialize(contentReader) as NotionObjectDto[])?.ToList() ?? [];
            }
            else
            {
                var dto = SingleSerializer.Deserialize(contentReader) as NotionObjectDto;
                dtos = dto != null ? [dto] : [];
            }
        }
        catch (XmlException ex)
        {
            errors.Add($"Invalid XML: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError, errors: errors, message: "XML Validation failed");
        }
        catch (InvalidOperationException ex)
        {
            errors.Add($"Deserialization error: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError, errors: errors, message: "XML Deserialization failed");
        }

        return await SaveAndReturnDtosAsync(dtos);
    }

    public async Task<StandardResponse<List<NotionObjectDto>>> ImportJsonAsync(string jsonContent)
    {
        var errors = new List<string>();

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(jsonContent);
        }
        catch (JsonException ex)
        {
            errors.Add($"Invalid JSON: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError, errors: errors, message: "JSON Parsing failed");
        }

        var results = _jsonSchema.Evaluate(doc.RootElement, new EvaluationOptions
        {
            OutputFormat = OutputFormat.List
        });

        if (!results.IsValid)
        {
            ExtractJsonErrors(results, errors);
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors.Count > 0 ? errors : ["Schema validation returned no details"],
                message: "JSON Schema validation failed");
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        List<NotionObjectDto> dtos = [];
        
        try
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                dtos = JsonSerializer.Deserialize<List<NotionObjectDto>>(jsonContent, options) ?? [];
            }
            else
            {
                var dto = JsonSerializer.Deserialize<NotionObjectDto>(jsonContent, options);
                dtos = dto != null ? [dto] : [];
            }
        }
        catch (JsonException ex)
        {
            errors.Add($"Deserialization error: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError, errors: errors, message: "JSON Deserialization failed");
        }

        return await SaveAndReturnDtosAsync(dtos);
    }

    #region Private methods

    private async Task<StandardResponse<List<NotionObjectDto>>> SaveAndReturnDtosAsync(List<NotionObjectDto> dtos)
    {
        if (dtos.Count == 0)
        {
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: ["No valid NotionObjectDto elements found"], message: "Data import failed");
        }

        var imported = new List<NotionObjectDto>();
        foreach (var dto in dtos)
        {
            var entity = dto.ToEntity();
            await _repository.AddAsync(entity);
            imported.Add(entity.ToDto());
        }

        await _repository.SaveChangesAsync();
        return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.Ok, imported);
    }

    private static void ExtractJsonErrors(EvaluationResults node, List<string> errList)
    {
        if (node.Errors is { Count: > 0 })
        {
            var path = string.IsNullOrEmpty(node.InstanceLocation.ToString()) ? "Root" : node.InstanceLocation.ToString();
            foreach (var kvp in node.Errors)
            {
                errList.Add($"[{path}] {kvp.Value}");
            }
        }

        if (node.Details != null)
        {
            foreach (var child in node.Details)
            {
                ExtractJsonErrors(child, errList);
            }
        }
    }

    private static string GetXmlRootName(string xml)
    {
        try
        {
            using var reader = XmlReader.Create(new StringReader(xml));
            reader.MoveToContent();
            return reader.Name;
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion
}