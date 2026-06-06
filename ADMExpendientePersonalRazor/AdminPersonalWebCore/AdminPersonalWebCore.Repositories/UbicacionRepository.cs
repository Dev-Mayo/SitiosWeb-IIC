using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AdminPersonalWebCore.Repository
{
    public class UbicacionRepository
    {
        private readonly string _connectionString;

        public UbicacionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void GuardarUbicacion(UbicacionCarga item)
        {
            using var db = new MySqlConnection(_connectionString);

            db.Execute(
                "SP_GEN_UBICACION_GUARDAR",
                new
                {
                    p_codigo_provincia = item.CodigoProvincia,
                    p_provincia = item.Provincia,
                    p_codigo_canton = item.CodigoCanton,
                    p_canton = item.Canton,
                    p_codigo_distrito = item.CodigoDistrito,
                    p_distrito = item.Distrito
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}