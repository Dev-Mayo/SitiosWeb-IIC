# SitiosWeb-IIC
Repositorio para trabajar las tareas y proyectos de sitios web

---











# Respuestas HTTP de las APIs que actualicé (Steven )

## API Oferentes

### GET /api/oferentes (STEVEN)

| Código | Descripción | Respuesta |
|--------|-------------|-----------|
| 200 OK | Consulta realizada correctamente. | Devuelve la lista de oferentes y la información de paginación. |
| 400 Bad Request | Parámetros inválidos. | `{ "mensaje": "Debe indicar un código de puesto válido." }` |
| 401 Unauthorized | Token no enviado o inválido. | Respuesta automática de ASP.NET Core. |
| 404 Not Found | El puesto solicitado no existe. | `{ "mensaje": "El puesto indicado no existe." }` |
| 500 Internal Server Error | Error interno del servidor. | `{ "mensaje": "Ocurrió un error interno del servidor." }` |

---

## API Empleados (STEVEN)

### POST /api/empleados

| Código | Descripción | Respuesta |
|--------|-------------|-----------|
| 201 Created | Empleado registrado correctamente. | `{ "exito": true, "mensaje": "Empleado creado con éxito.", "empleadoId": 15 }` |
| 400 Bad Request | Datos inválidos. | `{ "exito": false, "mensaje": "La identificación es requerida." }` |
| 401 Unauthorized | Token no enviado o inválido. | Respuesta automática de ASP.NET Core. |
| 404 Not Found | El puesto o el oferente no existen. | `{ "exito": false, "mensaje": "El puesto indicado no existe." }` |
| 409 Conflict | El empleado ya existe o el oferente ya fue contratado. | `{ "exito": false, "mensaje": "Ya existe un empleado con esa identificación." }` |
| 500 Internal Server Error | Error interno del servidor. | `{ "exito": false, "mensaje": "Ocurrió un error interno del servidor." }` |

---

## API Empleados - Health

### GET /api/empleados/health

| Código | Descripción |
|--------|-------------|
| 200 OK | La conexión con la base de datos es correcta. |
| 503 Service Unavailable | No fue posible conectarse a la base de datos. |
