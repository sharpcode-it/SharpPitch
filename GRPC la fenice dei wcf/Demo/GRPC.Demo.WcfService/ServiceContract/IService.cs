using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using GRPC.Demo.WcfService.DataContract;
using GRPC.Demo.WcfToGrpc.DomainModel;

namespace GRPC.Demo.WcfService.ServiceContract
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        Task<List<LibroWcf>> GetLibros();

    }

}
