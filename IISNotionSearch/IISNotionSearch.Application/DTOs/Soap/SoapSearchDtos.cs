using System.Runtime.Serialization;

namespace IISNotionSearch.Application.DTOs.Soap;

[DataContract]
public class SoapSearchResponse
{
    [DataMember]
    public List<SoapNotionObject> Results { get; set; } = new();

    [DataMember]
    public string Message { get; set; } = string.Empty;
}

[DataContract]
public class SoapNotionObject
{
    [DataMember] public string NotionId { get; set; } = null!;
    [DataMember] public string ObjectType { get; set; } = null!;
    [DataMember] public string Title { get; set; } = null!;
    [DataMember] public string Url { get; set; } = null!;
    [DataMember] public string? Icon { get; set; }
    [DataMember] public string? Cover { get; set; }
    [DataMember] public DateTimeOffset CreatedTime { get; set; }
    [DataMember] public DateTimeOffset LastEditedTime { get; set; }
    [DataMember] public bool InTrash { get; set; }
}
