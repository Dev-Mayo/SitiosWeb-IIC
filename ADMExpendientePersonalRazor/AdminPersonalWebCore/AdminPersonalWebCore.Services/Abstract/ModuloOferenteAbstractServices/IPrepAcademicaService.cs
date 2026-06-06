using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices
{
    public interface IPrepAcademicaService
    {
        /// <summary>
        ///     Método genérico para crear entradas en la bitácora de auditoría.
        /// </summary>
        /// 
        void GenericoCrearBitacora(string usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null);
        Task<IEnumerable<PreparacionAcad>> ObtenerPreparacionAcadAsync(string identificacion, string usuario);
        Task<PreparacionAcad?> ObtenerPreparacionAcadPorIdAsync(int id, string usuario);
        Task<int> CrearPreparacionAcadAsync(PreparacionAcad prep, string usuario);
        Task<int> ModificarPreparacionAcadAsync(PreparacionAcad prep, string usuario);
        Task<int> EliminarPreparacionAcadAsync(int id, string usuario);
    }
}
