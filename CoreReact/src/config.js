// ---------------------------------------------------------------------------
// CONFIGURACIÓN DE ENDPOINTS
// ---------------------------------------------------------------------------
// Punto único donde se definen las rutas de los servicios que consume la app.
//
// En desarrollo, vite.config.js reenvía "/servicios" hacia el WCF en
// http://localhost:63602 (sin CORS, como hacía PHP con cURL).
//
// Cuando se migre a APIs/microservicios:
//   1. Se puede cambiar API_BASE_URL por el dominio del API Gateway.
//   2. O reemplazar cada ruta de SERVICES por el endpoint del microservicio.
// Las vistas no deben tocarse: solo se modifica esta capa.
// ---------------------------------------------------------------------------

export const API_BASE_URL = '/servicios';

// Rutas relativas a API_BASE_URL
export const SERVICES = {
  autenticacion: '/AutenticacionService.svc/autenticar',
  puestos: '/PuestoService.svc/listarDisponibles',
  oferentesPorPuesto: '/OferenteService.svc/obtenerPorPuesto',
  detalleOferente: '/DetalleOferenteService.svc/obtener-detalle',
  registrarEmpleado: '/EmpleadoService.svc/registrar-empleado'
};
