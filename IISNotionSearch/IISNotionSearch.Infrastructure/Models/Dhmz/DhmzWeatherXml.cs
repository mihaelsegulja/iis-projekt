using System.Xml.Serialization;

namespace IISNotionSearch.Infrastructure.Models.Dhmz;

[XmlRoot("Hrvatska")]
public class DhmzWeatherXml
{
    [XmlElement("DatumTermin")]
    public DatumTermin DatumTermin { get; set; } = null!;

    [XmlElement("Grad")]
    public List<Grad> Gradovi { get; set; } = new();
}

public class DatumTermin
{
    [XmlElement("Datum")]
    public string Datum { get; set; } = null!;

    [XmlElement("Termin")]
    public string Termin { get; set; } = null!;
}

public class Grad
{
    [XmlAttribute("autom")]
    public string IsAutomatic { get; set; } = null!;

    [XmlElement("GradIme")]
    public string GradIme { get; set; } = null!;

    [XmlElement("Lat")]
    public string Lat { get; set; } = null!;

    [XmlElement("Lon")]
    public string Lon { get; set; } = null!;

    [XmlElement("Podatci")]
    public Podatci Podatci { get; set; } = null!;
}

public class Podatci
{
    [XmlElement("Temp")]
    public string Temp { get; set; } = null!;

    [XmlElement("Vlaga")]
    public string Vlaga { get; set; } = null!;

    [XmlElement("Tlak")]
    public string Tlak { get; set; } = null!;

    [XmlElement("TlakTend")]
    public string TlakTend { get; set; } = null!;

    [XmlElement("VjetarSmjer")]
    public string VjetarSmjer { get; set; } = null!;

    [XmlElement("VjetarBrzina")]
    public string VjetarBrzina { get; set; } = null!;

    [XmlElement("Vrijeme")]
    public string Vrijeme { get; set; } = null!;

    [XmlElement("VrijemeZnak")]
    public string VrijemeZnak { get; set; } = null!;
}
