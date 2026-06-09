using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AdminPersonalWebCore.Repository
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///   Conexion con la DB segun el nombre
        /// </summary>
        /// <param name="connectionName"> SEG, BIT, GEN, EMP, OFE</param>
        /// <returns></returns>
        public IDbConnection CreateConnection(string connectionName)
        {
            return new MySqlConnection(_configuration.GetConnectionString(connectionName));
        }
    }
}
