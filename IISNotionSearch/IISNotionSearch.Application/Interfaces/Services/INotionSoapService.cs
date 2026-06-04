using System.ServiceModel;
using IISNotionSearch.Application.DTOs.Soap;

namespace IISNotionSearch.Application.Interfaces.Services;

[ServiceContract(Namespace = "http://tempuri.org/")]
public interface INotionSoapService
{
    [OperationContract]
    Task<SoapSearchResponse> Search(string term);
}
