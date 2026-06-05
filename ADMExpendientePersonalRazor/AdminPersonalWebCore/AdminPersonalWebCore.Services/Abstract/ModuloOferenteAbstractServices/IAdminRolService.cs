using AdminPersonalWebCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices
{
    public interface IAdminRolService
    {
        Task<bool> ValidarDuplicadosAsync(string nombreRol);

        Task<IEnumerable<Rol>> ObtenerRolesAsync(string usuario);

        Task<int> EliminarRolAsync(int idRol, string usuario);

        Task<int> InsertarRolAsync(string nombreRol, string usuario);

        Task<int> ActualizarRolAsync(int idRol, string nombreRol, string usuario);

        Task<Rol?> ObtenerRolAsync(int? idRol = null, string nombreRol = null);
    }
}