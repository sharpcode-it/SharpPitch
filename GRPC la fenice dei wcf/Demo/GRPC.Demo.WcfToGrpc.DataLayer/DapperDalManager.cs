using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GRPC.Demo.WcfToGrpc.DomainModel;
using Microsoft.Data.SqlClient;

namespace GRPC.Demo.WcfToGrpc.DataLayer
{
    public class DapperDalManager
    {
        private readonly DataProvider _myDal;

        public DapperDalManager(string connectionString)
        {
            _myDal = new DataProvider { ConnectionString = connectionString };
        }

        public async Task<Libro> GetById(Guid id)
        {
            const string sqlSt = "SELECT * FROM Libro l LEFT JOIN Scrittore s on l.AutoreId = s.Id WHERE l.ID = @id";

            using (var conn = _myDal.Connect())
            {
                var item = await conn.QueryAsync<Libro,Scrittore,Libro>(sqlSt, (i, k) =>
                {
                    i.Autore = k;
                    return i;
                } ,new {id});
                return item?.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Libro>> GetAll()
        {
            const string sqlSt = "SELECT * FROM Libro l LEFT JOIN Scrittore s on l.AutoreId = s.Id";

            using (var conn = _myDal.Connect())
            {
                var item = await conn.QueryAsync<Libro,Scrittore,Libro>(sqlSt, (i, k) =>
                {
                    i.Autore = k;
                    return i;
                });
                return item;
            }
        }


   
    }

    public class DataProvider : IDisposable
    {
        private SqlConnection _internalConn;
        public string ConnectionString { get; set; }

        public SqlConnection Connect(string externalConnectionString = null)
        {
            if (_internalConn?.State == ConnectionState.Open)
                return _internalConn;

            SqlConnection cnDbConnection = null;

            try
            {
                cnDbConnection = new SqlConnection(externalConnectionString ?? ConnectionString);
                cnDbConnection.Open();
                _internalConn = cnDbConnection;
                return cnDbConnection;
            }
            catch (Exception ex)
            {
                cnDbConnection?.Dispose();

                throw;
            }
        }

        public void Dispose()
        {
            _internalConn?.Dispose();
        }
    }
}
