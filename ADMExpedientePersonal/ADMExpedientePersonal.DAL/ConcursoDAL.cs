using ADMExpedientePersonal.Entities;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ADMExpedientePersonal.DAL
{
    public class ConcursoDAL
    {
        public List<Concurso> Listar()
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                return cn.Query<Concurso>(
                    "SP_OFE_CONCURSO_LISTAR",
                    commandType: CommandType.StoredProcedure
                ).ToList();
            }
        }

        public Concurso Obtener(string codigo)
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_concurso", codigo);

                return cn.QueryFirstOrDefault<Concurso>(
                    "SP_OFE_CONCURSO_OBTENER",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public void Insertar(Concurso item)
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_concurso", item.codigo_concurso);
                p.Add("p_nombre", item.nombre);
                p.Add("p_fecha_inicio", item.fecha_inicio);
                p.Add("p_fecha_fin", item.fecha_fin);
                p.Add("p_estado", item.estado);

                cn.Execute("SP_OFE_CONCURSO_INSERTAR", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void Actualizar(Concurso item)
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_concurso", item.codigo_concurso);
                p.Add("p_nombre", item.nombre);
                p.Add("p_fecha_inicio", item.fecha_inicio);
                p.Add("p_fecha_fin", item.fecha_fin);
                p.Add("p_estado", item.estado);

                cn.Execute("SP_OFE_CONCURSO_ACTUALIZAR", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void Eliminar(string codigo)
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_concurso", codigo);

                cn.Execute("SP_OFE_CONCURSO_ELIMINAR", p, commandType: CommandType.StoredProcedure);
            }
        }

        public void CambiarEstado(string codigo)
        {
            using (var cn = ConexionOFE.ObtenerConexion())
            {
                var p = new DynamicParameters();
                p.Add("p_codigo_concurso", codigo);

                cn.Execute("SP_OFE_CONCURSO_CAMBIAR_ESTADO", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}