using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services
{
    public class AdminRolService
    {
        private readonly AdminRolRepository _adminRolRepository;
        private readonly BitacoraService _bitacoraBLL;

        public AdminRolService(AdminRolRepository adminRolRepository, BitacoraService bitacoraBLL)
        {
            _adminRolRepository = adminRolRepository;
            _bitacoraBLL = bitacoraBLL;
        }

        public async Task<bool> ValidarDuplicadosAsync(string nombreRol)
        {
            return await _adminRolRepository.ValidarDuplicadosAsync(nombreRol);
        }

        public async Task<IEnumerable<Rol>> ObtenerRolesAsync(string usuario)
        {
            var roles = await _adminRolRepository.ObtenerRolAsync();
            var bitacora = new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.READ,
                DescripcionJson = _bitacoraBLL.CrearJsonConsulta("Roles")
            };
            _bitacoraBLL.Registrar(bitacora);
            return roles;
        }

        public async Task<int> EliminarRolAsync(int idRol, string usuario)
        {
            var rol = (await _adminRolRepository.ObtenerRolAsync(idRol: idRol)).FirstOrDefault();
            var bitacora = new Bitacora
            {
                Usuario = usuario,
                Accion = AccionBitacora.DELETE,
                DescripcionJson = _bitacoraBLL.CrearJsonEliminacion(rol)
            };

            if (!await _adminRolRepository.RolEnUsoBitAsync(idRol))
            {
                var filas = await _adminRolRepository.EliminarRolAsync(idRol);
                if (filas > 0)
                {
                    _bitacoraBLL.Registrar(bitacora);
                    return 2; // éxito
                }
                return 1; // error
            }
            return 0; // rol en uso
        }

        public async Task<int> InsertarRolAsync(string nombreRol, string usuario)
        {
            if (await ValidarDuplicadosAsync(nombreRol))
                return 0;

            var resultado = await _adminRolRepository.InsertarRolAsync(nombreRol);
            if (resultado > 0)
            {
                var rol = (await _adminRolRepository.ObtenerRolAsync(nombreRol: nombreRol)).FirstOrDefault();
                var bitacora = new Bitacora
                {
                    Usuario = usuario,
                    Accion = AccionBitacora.CREATE,
                    DescripcionJson = _bitacoraBLL.CrearJsonNuevo(rol)
                };
                _bitacoraBLL.Registrar(bitacora);
                return 1;
            }
            return 0;
        }

        public async Task<int> ActualizarRolAsync(int idRol, string nombreRol, string usuario)
        {
            var rolAnterior = (await _adminRolRepository.ObtenerRolAsync(idRol: idRol)).FirstOrDefault();

            if (await ValidarDuplicadosAsync(nombreRol))
            {
                if (!nombreRol.Equals(rolAnterior?.nombre_rol, StringComparison.OrdinalIgnoreCase))
                    return 0;
            }

            var resultado = await _adminRolRepository.ActualizarRolAsync(idRol, nombreRol);
            if (resultado > 0)
            {
                var rolNuevo = (await _adminRolRepository.ObtenerRolAsync(idRol: idRol)).FirstOrDefault();
                var bitacora = new Bitacora
                {
                    Usuario = usuario,
                    Accion = AccionBitacora.UPDATE,
                    DescripcionJson = _bitacoraBLL.CrearJsonActualizacion(rolAnterior, rolNuevo)
                };
                _bitacoraBLL.Registrar(bitacora);
                return 1;
            }
            return 0;
        }

        public async Task<Rol?> ObtenerRolAsync(int? idRol = null, string nombreRol = null)
        {
            if (idRol == null && nombreRol == null)
                return null;

            return (await _adminRolRepository.ObtenerRolAsync(idRol: idRol, nombreRol: nombreRol)).FirstOrDefault();
        }
    }
}