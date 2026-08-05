import { autenticar } from './authService';
import { listarPuestos } from './puestoService';
import { obtenerOferentesPorPuesto } from './oferenteService';
import { obtenerDetalleOferente } from './detalleOferenteService';
import { registrarEmpleado } from './empleadoService';

// Fachada de servicios: las vistas importan únicamente este módulo.
export const servicios = {
  autenticar,
  listarPuestos,
  obtenerOferentesPorPuesto,
  obtenerDetalleOferente,
  registrarEmpleado
};
