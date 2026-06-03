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
    private readonly string _schemaDir;

    public ImportService(INotionObjectRepository repository)
    {
        _repository = repository;
        _schemaDir = Path.Combine(AppContext.BaseDirectory, "Schemas");
    }

    public async Task<StandardResponse<List<NotionObjectDto>>> ImportXmlAsync(string xmlContent)
    {
        var errors = new List<string>();

        var schemaSet = new XmlSchemaSet();
        using (var schemaReader = XmlReader.Create(Path.Combine(_schemaDir, "notion-object-schema.xsd")))
        {
            schemaSet.Add(null, schemaReader);
        }

        var settings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schemaSet,
            ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings
        };

        settings.ValidationEventHandler += (_, args) => errors.Add(args.Message);

        NotionObjectDto? dto;
        try
        {
            using var reader = XmlReader.Create(new StringReader(xmlContent), settings);
            var serializer = new XmlSerializer(typeof(NotionObjectDto));
            dto = serializer.Deserialize(reader) as NotionObjectDto;
        }
        catch (XmlException ex)
        {
            errors.Add($"Invalid XML: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "XML validation failed");
        }
        catch (InvalidOperationException ex)
        {
            errors.Add($"Deserialization error: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "XML validation failed");
        }

        if (errors.Count > 0)
        {
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "XML validation failed");
        }

        if (dto == null)
        {
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: ["Failed to deserialize XML content"],
                message: "XML validation failed");
        }

        var entity = dto.ToEntity();
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var result = new List<NotionObjectDto> { entity.ToDto() };
        return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.Ok, result);
    }

    public async Task<StandardResponse<List<NotionObjectDto>>> ImportJsonAsync(string jsonContent)
    {
        var errors = new List<string>();

        var schemaText = await File.ReadAllTextAsync(
            Path.Combine(_schemaDir, "notion-object-schema.json"));
        var schema = JsonSchema.FromText(schemaText);

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(jsonContent);
        }
        catch (JsonException ex)
        {
            errors.Add($"Invalid JSON: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "JSON validation failed");
        }

        var results = schema.Evaluate(doc.RootElement, new EvaluationOptions
        {
            OutputFormat = OutputFormat.List
        });

        if (!results.IsValid)
        {
            foreach (var error in results.Errors ?? new Dictionary<string, string>())
            {
                errors.Add($"{error.Key}: {error.Value}");
            }

            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "JSON validation failed");
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        NotionObjectDto? dto;
        try
        {
            dto = JsonSerializer.Deserialize<NotionObjectDto>(jsonContent, options);
        }
        catch (JsonException ex)
        {
            errors.Add($"Deserialization error: {ex.Message}");
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: errors, message: "JSON validation failed");
        }

        if (dto == null)
        {
            return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.InternalError,
                errors: ["Failed to deserialize JSON content"],
                message: "JSON validation failed");
        }

        var entity = dto.ToEntity();
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var result = new List<NotionObjectDto> { entity.ToDto() };
        return StandardResponse<List<NotionObjectDto>>.Create(ResultStatus.Ok, result);
    }
}
