using AdminPersonalWebCore.Entities;
using AdminPersonalWebCore.Entities.ModuloOferenteEntities;
using AdminPersonalWebCore.Repository.ModuloOferenteRepository;
using AdminPersonalWebCore.Services.Abstract.ModuloOferenteAbstractServices;
using Mysqlx.Expr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPersonalWebCore.Services.ModuloOferenteServices
{
    public class ExpLaboralService : IExpLaboralService
    {
        private readonly ExpLaboralRepository _expLaboralrepository;
        private readonly BitacoraService _bitacoraService;
        private readonly CompaniaService _compania;

        public ExpLaboralService(ExpLaboralRepository repository, BitacoraService bitacoraService,  CompaniaService compania)
        {
            _expLaboralrepository = repository;
            _bitacoraService = bitacoraService;
            _compania = compania;
        }

        public void GenericoCrearBitacora(string usuario, int accion, int resultado, object objetoOriginal = null, object objetoAnterior = null, string detalles = null)
        {
            _bitacoraService.GenericoCrearBitacora(usuario, accion, resultado, objetoOriginal, objetoAnterior, detalles);
        }

        public List<Compania> ObtenerCompanias(string usuarioActual)
        {
            return _compania.ObtenerTodos(usuarioActual);
        }

        public async Task<IEnumerable<ExpLaboral>> ObtenerExpLaboralAsync(string oferenteIdentificacion, string usuarioActual)
        {
            if (string.IsNullOrWhiteSpace(oferenteIdentificacion))
                return Enumerable.Empty<ExpLaboral>();
            GenericoCrearBitacora(usuarioActual, 3, 1, detalles: "Experiancia Laboral");
            return await _expLaboralrepository.ObtenerExpLaboralAsync(oferenteIdentificacion);
        }

        public async Task<int> CrearExpLaboralAsync(ExpLaboral exp, string usuarioActual)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(exp.Empresa) || string.IsNullOrWhiteSpace(exp.Puesto))
                return 0;

            if (exp.FechaFin < exp.FechaInicio)
                return 0;
            int resultado = await _expLaboralrepository.CrearExpLaboralAsync(exp);
            GenericoCrearBitacora(usuarioActual, 0, resultado, objetoOriginal: exp);

            return resultado;
        }

        public async Task<int> ModificarExpLaboralAsync(ExpLaboral exp, string usuarioActual)
        {
            if (exp.Id <= 0)
                return 0;

            if (string.IsNullOrWhiteSpace(exp.Empresa) || string.IsNullOrWhiteSpace(exp.Puesto))
                return 0;

            if (exp.FechaFin < exp.FechaInicio)
                return 0;

            var anterior = await ObtenerExpLaboralPorId(exp.Id);
            int resultado = await _expLaboralrepository.ModificarExpLaboralAsync(exp);
            GenericoCrearBitacora(usuarioActual, 1, resultado, objetoOriginal: exp, objetoAnterior: anterior);
            return resultado;
        }

        public async Task<int> EliminarExpLaboralAsync(int id, string usuarioActual)
        {
            if (id <= 0)
                return 0;
            var anterior = await ObtenerExpLaboralPorId(id);
            int resultado = await _expLaboralrepository.EliminarExpLaboralAsync(id);
            GenericoCrearBitacora(usuarioActual, 2, resultado, objetoOriginal: anterior);
            return resultado;
        }

        //Helpers ==================================================================
        private async Task<ExpLaboral> ObtenerExpLaboralPorId(int Id)
        {
            return await _expLaboralrepository.ObtenerExpLaboralPorId(Id);
        }
    }
}