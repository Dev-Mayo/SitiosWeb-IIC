// ---------------------------------------------------------------------------
// CONFIGURACIÓN DE ENDPOINTS
// ---------------------------------------------------------------------------
// Punto único donde se definen las rutas de los servicios que consume la app.
//
// La app consume los microservicios a través del API Gateway (YARP) en
// http://localhost:5080. El gateway reenvía cada /api/* al microservicio
// correspondiente y ya expone CORS para el origen del dev server (5173).
//
// Autenticación: el login devuelve un token JWT que se guarda en
// localStorage (TOKEN_KEY); apiClient lo adjunta automáticamente como
// "Authorization: Bearer <token>" en el resto de llamadas.
// ---------------------------------------------------------------------------

//export const API_BASE_URL = 'https://sitiosweb-iic-3.onrender.com';
export const API_BASE_URL = 'https://admexpedientepersonalapigateway20260809214543.azurewebsites.net/';

// Claves de almacenamiento local (sesión).
export const USER_KEY = 'corereact_usuario';
export const TOKEN_KEY = 'corereact_token';

// Rutas relativas a API_BASE_URL
export const SERVICES = {
  autenticacion: '/api/auth/login',
  puestos: '/api/puestos',
  oferentesPorPuesto: '/api/oferentes',
  detalleOferente: '/api/oferentes',
  registrarEmpleado: '/api/empleados'
};
