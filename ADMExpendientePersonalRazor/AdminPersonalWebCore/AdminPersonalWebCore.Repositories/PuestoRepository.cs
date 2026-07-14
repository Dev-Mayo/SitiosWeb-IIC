using AdminPersonalWebCore.Entities;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace AdminPersonalWebCore.Repository
{
    public class PuestoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PuestoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public List<Puesto> ObtenerTodos()
        {
            using var db = _connectionFactory.CreateConnection("EMP");

            return db.Query<Puesto>(
                "SP_EMP_PUESTO_LISTAR",
                commandType: CommandType.StoredProcedure
            ).ToList();
        }

        public Puesto ObtenerPorId(int puestoId)
        {
            using var db = _connectionFactory.CreateConnection("EMP");

            return db.QueryFirstOrDefault<Puesto>(
                "SP_EMP_PUESTO_OBTENER",
                new { p_puesto_id = puestoId },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Insertar(Puesto puesto)
        {
            using var db = _connectionFactory.CreateConnection("EMP");

            db.Execute(
                "SP_EMP_PUESTO_INSERTAR",
                new
                {
                    p_nombre = puesto.Nombre,
                    p_salario = puesto.Salario,
                    p_jefe_puesto_id = puesto.JefePuestoId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Actualizar(Puesto puesto)
        {
            using var db = _connectionFactory.CreateConnection("EMP");

            db.Execute(
                "SP_EMP_PUESTO_ACTUALIZAR",
                new
                {
                    p_puesto_id = puesto.PuestoId,
                    p_nombre = puesto.Nombre,
                    p_salario = puesto.Salario,
                    p_jefe_puesto_id = puesto.JefePuestoId
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void Eliminar(int puestoId)
        {
            using var db = _connectionFactory.CreateConnection("EMP");

            db.Execute(
                "SP_EMP_PUESTO_ELIMINAR",
                new { p_puesto_id = puestoId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}