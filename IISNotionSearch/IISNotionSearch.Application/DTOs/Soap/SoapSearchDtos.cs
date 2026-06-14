using System.Runtime.Serialization;
using System.Xml;

namespace IISNotionSearch.Application.DTOs.Soap;

[DataContract]
public class SoapSearchResponse
{
    [DataMember]
    public XmlElement? RawResponseXml { get; set; }

    [DataMember]
    public string Message { get; set; } = string.Empty;
}
