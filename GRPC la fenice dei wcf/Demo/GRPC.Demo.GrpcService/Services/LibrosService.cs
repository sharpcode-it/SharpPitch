using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using GRPC.Demo.WcfToGrpc.DataLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharpCoding.SharpHelpers;

namespace GRPC.Demo.GrpcService.Services
{
    public class LibrosService : LibroService.LibroServiceBase
    {
        private readonly ILogger<LibrosService> _logger;
        private IConfiguration Configuration { get; }

        public LibrosService(ILogger<LibrosService> logger,IConfiguration config)
        {
            _logger = logger;
            Configuration = config;
        }

        public override Task<LibroGrpc> GetById(RequestGetByid request, ServerCallContext context)
        {
            var libros = GetLibros().Result;

            if((libros == null) || (!libros.Any()))
                throw new RpcException(new Status(StatusCode.NotFound, "No Data"));

            var myLibro = libros.FirstOrDefault(i => i.Id == request.Id.ToLower());

            if(myLibro == null)
                throw new RpcException(new Status(StatusCode.NotFound, "No Data"));

            return Task.FromResult(myLibro);
        }

        public override Task<LibroGrpc> GetByIdAuth(RequestGetByid request, ServerCallContext context)
        {
            if(!ValidateRequest(context))
                throw new RpcException(new Status(StatusCode.Unauthenticated,"NO AUTH"));

            var libros = GetLibros().Result;

            if((libros == null) || (!libros.Any()))
                throw new RpcException(new Status(StatusCode.NotFound, "No Data"));

            var myLibro = libros.FirstOrDefault(i => i.Id == request.Id.ToLower());

            if(myLibro == null)
                throw new RpcException(new Status(StatusCode.NotFound, "No Data"));

            return Task.FromResult(myLibro);
        }

        private bool ValidateRequest(ServerCallContext context)
        {
            var metadataEntry = context.RequestHeaders.FirstOrDefault(m =>
                string.Equals(m.Key, "authorization", StringComparison.Ordinal));

            if (metadataEntry == null)
                return false;

            return !metadataEntry.Equals(default(Metadata.Entry)) && metadataEntry.Value != null;
        }

        public override async Task GetAllStream(Empty request, IServerStreamWriter<LibroGrpc> responseStream, ServerCallContext context)
        {
            var libros = GetLibros().Result;

            foreach (var item in libros)
            {
                await responseStream.WriteAsync(item);
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
        }

        public override Task<GetAllReply> GetAll(Empty request, ServerCallContext context)
        {
            var response = new GetAllReply();
            var libros = GetLibros().Result;

            if((libros == null) || (!libros.Any()))
                throw new RpcException(new Status(StatusCode.NotFound, "No Data"));

            foreach (var libroGrpc in libros)
            {
                response.Items.Add(libroGrpc);
            }

            return Task.FromResult(response);
        }

        private async Task<List<LibroGrpc>> GetLibros()
        {
            try
            {
                var dataManager =
                    new DapperDalManager(Configuration.GetConnectionString("ConnString"));

                var result = await dataManager.GetAll();

                var libros = result.ToList();

                var returnList = new List<LibroGrpc>();

                foreach (var libro in libros)
                {
                    returnList.Add(new LibroGrpc()
                    {
                        Autore = new ScrittoreGrpc()
                        {
                            Cognome = libro.Autore.Cognome,
                            Nome = libro.Autore.Nome,
                            Id = libro.Autore.Id.ToString()
                        },
                        Id = libro.Id.ToString(),
                        Titolo = libro.Titolo
                    });
                }

                return returnList;
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }

}
