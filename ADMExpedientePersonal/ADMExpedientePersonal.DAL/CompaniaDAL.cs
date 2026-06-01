using ADMExpedientePersonal.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class CompaniaDAL
    {
        public List<Compania> Listar()
        {
            using (var cn = ConexionGEN.ObtenerConexion())
            {
                return cn.Query<Compania>(
                    "SP_GEN_COMPANIA_LISTAR",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        public Compania Obtener(string codigo)
        {
            using (var cn = ConexionGEN.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_compania", codigo);

                return cn.QueryFirstOrDefault<Compania>(
                    "SP_GEN_COMPANIA_OBTENER",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Insertar(Compania item)
        {
            using (var cn = ConexionGEN.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_compania", item.codigo_compania);
                p.Add("p_nombre", item.nombre);

                cn.Execute(
                    "SP_GEN_COMPANIA_INSERTAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Actualizar(Compania item)
        {
            using (var cn = ConexionGEN.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_compania", item.codigo_compania);
                p.Add("p_nombre", item.nombre);

                cn.Execute(
                    "SP_GEN_COMPANIA_ACTUALIZAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Eliminar(string codigo)
        {
            using (var cn = ConexionGEN.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_compania", codigo);

                cn.Execute(
                    "SP_GEN_COMPANIA_ELIMINAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }
    }
}