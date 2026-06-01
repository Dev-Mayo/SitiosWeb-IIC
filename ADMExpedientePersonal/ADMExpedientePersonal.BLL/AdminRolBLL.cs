using ADMExpedientePersonal.DAL;
using ADMExpedientePersonal.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADMExpedientePersonal.BLL
{
    public class AdminRolBLL
    {
        private AdminRolDAL adminRolDAL = new AdminRolDAL();
        private BitacoraBLL bitacoraBLL = new BitacoraBLL();

        public bool ValidarDuplicados(string nombre_rol) //Validacion para evitar execpcion de la DB
        {
            return adminRolDAL.ValidarDuplicados(nombre_rol);
        }

        public List<Rol> ObtenerRoles(string Usuario) //Recuperar todos los roles para mostrar, se registra la consulta en la bitácora
        {
            List<Rol> roles = adminRolDAL.ObtenerRol();
            Bitacora bitacora = new Bitacora
            {
                Usuario = Usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson = bitacoraBLL.CrearJsonConsulta("Roles") //Un json de solo mensaje
            };

            bitacoraBLL.RegistrarBitacora(bitacora);
            return roles;
        }

        public int eliminarRol(int id_rol, string usuario) //2: exito, 1: error, 0: rol en uso
        {
            Rol rol = adminRolDAL.ObtenerRol(id_rol:id_rol).FirstOrDefault(); // Obtener el rol antes de eliminarlo para registrar en la bitácora
            Bitacora bitacora = new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = bitacoraBLL.CrearJsonEliminacion(rol)
            };

            if (!adminRolDAL.RolEnUso_bit(id_rol))
            {
                if (adminRolDAL.eliminarRol(id_rol) > 0)
                {
                    bitacoraBLL.RegistrarBitacora(bitacora); //hacer el registro en la bitácora solo si la eliminación fue exitosa
                    return 2;
                }
                else
                    return 1;
            }
            else
                return 0;
        }

        public int InsertarRol(string nombre_rol, string usuario) //1: exito, 0: error o nombre duplicado
        {
            if (ValidarDuplicados(nombre_rol))
                return 0;
            int resultado = adminRolDAL.InsertarRol(nombre_rol);

            if (resultado > 0)
            {
                Rol rol = adminRolDAL.ObtenerRol(nombre_rol: nombre_rol).FirstOrDefault(); // Obtener el rol despues de insertarlo para registrar en la bitácora
                Bitacora bitacora = new Bitacora
                {
                    Usuario = usuario,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = bitacoraBLL.CrearJsonNuevo(rol) //enviamos el objeto completo para crear el json
                };
                bitacoraBLL.RegistrarBitacora(bitacora);
                return 1;
            }
            else
                return 0;
        }

        public int ActualizarRol(int id_rol, string nombre_rol, string usuario) //1: exito, 0: error o nombre duplicado
        {
            Rol rolAnterior = adminRolDAL.ObtenerRol(id_rol: id_rol).FirstOrDefault(); // Recuperar rol antes de modificar

            if (ValidarDuplicados(nombre_rol)) // Validar que el nuevo nombre no exista ya en otro rol, si el nombre es igual al del mismo rol no se considera duplicado
            { //Esta demas, ya que se valida en el front para enviar un mensaje
                if (!nombre_rol.Equals(rolAnterior.nombre_rol, StringComparison.OrdinalIgnoreCase))
                    return 0; 
            }
            int resultado = adminRolDAL.ActualizarRol(id_rol, nombre_rol);

            if (resultado > 0)
            {
                Rol rolNuevo = adminRolDAL.ObtenerRol(id_rol: id_rol).FirstOrDefault(); // Datos nuevos ya guardados
                Bitacora bitacora = new Bitacora
                {
                    Usuario = usuario,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = bitacoraBLL.CrearJsonActualizacion(rolAnterior, rolNuevo) //solo enviamos ambos objetos
                };
                bitacoraBLL.RegistrarBitacora(bitacora);
                return 1;
            }
            else
                return 0;
        }

        public Rol ObtenerRol(int? id_rol = null, string nombre_rol = null)
        {
            if (id_rol == null && nombre_rol == null)
                return null; // No se proporcionó ningún criterio de búsqueda
            return adminRolDAL.ObtenerRol(id_rol: id_rol, nombre_rol: nombre_rol).FirstOrDefault();
        }


    }
}   
