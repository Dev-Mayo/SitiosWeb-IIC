using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace ADMExpedientePersonal.WebServices.Repositories.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection(string connectionName);
    }
}