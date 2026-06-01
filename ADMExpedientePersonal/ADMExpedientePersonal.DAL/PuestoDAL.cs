using ADMExpedientePersonal.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class PuestoDAL
    {
        public List<Puesto> Listar()
        {
            using (var cn = ConexionEMP.ObtenerConexion())
            {
                return cn.Query<Puesto>(
                    "SP_EMP_PUESTO_LISTAR",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        public Puesto Obtener(int puestoId)
        {
            using (var cn = ConexionEMP.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_puesto_id", puestoId);

                return cn.QueryFirstOrDefault<Puesto>(
                    "SP_EMP_PUESTO_OBTENER",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Insertar(Puesto item)
        {
            using (var cn = ConexionEMP.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_nombre", item.nombre);
                p.Add("p_salario", item.salario);
                p.Add("p_jefe_puesto_id", item.jefe_puesto_id);

                cn.Execute(
                    "SP_EMP_PUESTO_INSERTAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Actualizar(Puesto item)
        {
            using (var cn = ConexionEMP.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_puesto_id", item.puesto_id);
                p.Add("p_nombre", item.nombre);
                p.Add("p_salario", item.salario);
                p.Add("p_jefe_puesto_id", item.jefe_puesto_id);

                cn.Execute(
                    "SP_EMP_PUESTO_ACTUALIZAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Eliminar(int puestoId)
        {
            using (var cn = ConexionEMP.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_puesto_id", puestoId);

                cn.Execute(
                    "SP_EMP_PUESTO_ELIMINAR",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }
    }
}