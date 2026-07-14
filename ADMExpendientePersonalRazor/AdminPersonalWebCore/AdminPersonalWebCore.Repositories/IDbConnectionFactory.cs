using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Repository
{
    public interface IDbConnectionFactory
    {
        /// <summary>
        ///   Conexion con la DB segun el nombre
        /// </summary>
        /// <param name="connectionName"> SEG, BIT, GEN, EMP, OFE</param>
        /// <returns></returns>
        IDbConnection CreateConnection(string connectionName);
    }
}
