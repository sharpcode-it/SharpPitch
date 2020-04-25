using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using GRPC.Demo.WcfService.DataContract;
using GRPC.Demo.WcfToGrpc.DataLayer;
using Microsoft.Extensions.Configuration;

namespace GRPC.Demo.WcfService.ServiceContract
{
    public class ServiceContract:IService
    {
        public async Task<List<LibroWcf>> GetLibros()
        {
            try
            {
                var dataManager =
                    new DapperDalManager(ConfigurationManager.ConnectionStrings["ConnString"].ConnectionString);

                var result = await dataManager.GetAll();

                var libros = result.ToList();

                var returnList = new List<LibroWcf>();

                foreach (var libro in libros)
                {
                    returnList.Add(new LibroWcf
                    {
                        Autore = new ScrittoreWcf
                        {
                            Cognome = libro.Autore.Cognome,
                            Nome = libro.Autore.Nome,
                            Id = libro.Autore.Id
                        },
                        Id = libro.Id,
                        Titolo = libro.Titolo
                    });
                }

                return returnList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

    }
}