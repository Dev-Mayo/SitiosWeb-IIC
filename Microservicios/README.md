# ADMExpedientePersonal – Microservicios (.NET 10)

Migración del WCF `ServiciosWebCore` (`.NET Framework 4.8` + MySQL) a una solución de
microservicios con **ASP.NET Core Web API (.NET 10)**, un proyecto por cada WCF, protegidos
con **JWT** y accesibles a través de un **Gateway (YARP)**.

> **Independencia total**: cada microservicio es un proyecto autónomo sin dependencias
> entre sí (ni `ProjectReference`, ni código compartido). Cada uno incluye sus propias
> entidades/DTOs (`Models/`), acceso a datos (`Data/` + `Repositories/`) y seguridad JWT
> (`Security/`). Lo único en común es estar dentro de la misma solución
> `Microservicios.slnx`.

## Arquitectura

```
Cliente (React / Postman)
        │
        ▼
┌─────────────────────────────┐   Gateway YARP en http://localhost:5080
│     Api.Gateway (5080)      │   rutas /api/* → microservicios
└─────┬──────┬──────┬─────────┘
      │      │      │
      ▼      ▼      ▼
 Api.Auth  Api.Puestos  Api.Oferentes   ... (cada uno con su BD)
  5001      5002         5003
```

| Proyecto               | Puerto HTTP | BD (Aiven MySQL) | Responsabilidad |
|------------------------|-------------|-------------------|-----------------|
| `Api.Auth`             | 5001        | SEG, BIT, GEN     | Login + emisión de JWT |
| `Api.Puestos`          | 5002        | EMP               | Listar puestos disponibles |
| `Api.Oferentes`        | 5003        | OFE, BIT          | Oferentes por puesto |
| `Api.DetalleOferente`  | 5004        | OFE, BIT          | Detalle de un oferente |
| `Api.Empleados`        | 5005        | EMP, BIT          | Registrar empleado + health |
| `Api.Gateway`          | 5080        | –                 | Reverse proxy (YARP) |

> Todos los microservicios exigen token JWT, excepto `POST /api/auth/login`.

## Requisitos

- SDK **.NET 10** (`dotnet --version` → 10.0.x)
- Postman (o cualquier cliente HTTP)
- Acceso a internet (las BD están en Aiven, con SSL requerido)

## Cómo ejecutar

Compilar la solución:

```powershell
cd C:\Users\Rafae\Documents\GitHub\SitiosWeb-IIC\Microservicios
dotnet build Microservicios.slnx
```

Levantar cada proyecto (5 microservicios + gateway) en **terminales separadas**:

```powershell
dotnet run --project src\Services\ADMExpedientePersonal.Api.Auth
dotnet run --project src\Services\ADMExpedientePersonal.Api.Puestos
dotnet run --project src\Services\ADMExpedientePersonal.Api.Oferentes
dotnet run --project src\Services\ADMExpedientePersonal.Api.DetalleOferente
dotnet run --project src\Services\ADMExpedientePersonal.Api.Empleados
dotnet run --project src\Gateway\ADMExpedientePersonal.Api.Gateway
```

Los puertos se toman de `Properties/launchSettings.json` de cada proyecto
(5001–5005 y 5080). Espera a que cada consola muestre `Now listening on ...`.
Para probar desde Postman basta con el **gateway (5080)**; los servicios internos no
necesitan exponerse fuera de la máquina.

## Endpoints

### Vía Gateway (recomendado para Postman)

| Método | Ruta | Auth | Descripción |
|--------|------|------|-------------|
| POST | `http://localhost:5080/api/auth/login` | Sin token | Autenticar y obtener JWT |
| GET | `http://localhost:5080/api/auth/me` | Bearer | Datos del usuario del token |
| GET | `http://localhost:5080/api/puestos` | Bearer | Lista de puestos disponibles |
| GET | `http://localhost:5080/api/oferentes?codigoPuesto={id}` | Bearer | Oferentes de un puesto |
| GET | `http://localhost:5080/api/oferentes/{identificacion}` | Bearer | Detalle de un oferente |
| GET | `http://localhost:5080/api/empleados/health` | Bearer | Verifica conexión a BD EMP |
| POST | `http://localhost:5080/api/empleados` | Bearer | Registra un empleado |

### Códigos de respuesta relevantes

- `401` – Falta el token o el token es inválido/expirado.
- `423` – Usuario bloqueado (3 intentos fallidos de login en 15 min).
- `409` – Intento de registrar un empleado que ya existe.
- `400` – Validación de datos fallida.
- `503` – `GET /api/empleados/health` sin conexión a la BD.

## Pruebas en Postman

### 1. Login (obtener el token)

`POST http://localhost:5080/api/auth/login`

Body (JSON):

```json
{
  "usuario": "TU_USUARIO",
  "password": "TU_PASSWORD"
}
```

Respuesta esperada (200):

```json
{
  "exito": true,
  "mensaje": "OK",
  "idUsuario": 1,
  "nombreCompleto": "Nombre Apellido",
  "usuario": "TU_USUARIO",
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiraEn": "2026-08-04T..."
}
```

> Si no recuerdas las credenciales, pruébalo con el mismo usuario con el que
> entraba la aplicación web anterior (los datos viven en la BD `SEG`).

### 2. Usar el token en los endpoints protegidos

1. Copia el valor de `token`.
2. En el endpoint protegido, pestaña **Authorization** → **Bearer Token** → pega el token.
   O añade manualmente el header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

3. Ejecuta, por ejemplo:

```
GET http://localhost:5080/api/puestos
GET http://localhost:5080/api/oferentes?codigoPuesto=1
GET http://localhost:5080/api/oferentes/1234567890
GET http://localhost:5080/api/empleados/health
```

### 3. Registrar un empleado (con token)

`POST http://localhost:5080/api/empleados`

```json
{
  "identificacion": "1122334455",
  "tipoIdentificacion": "F",
  "nombreCompleto": "Juan Perez",
  "fechaNacimiento": "1990-05-10",
  "fechaInicio": "2026-08-01",
  "correos": ["juan.perez@correo.com"],
  "telefonos": ["8888-8888"],
  "usuario": ""
}
```

- `usuario` se ignora: se toma del token.
- La edad debe ser ≥ 18; correos/teléfonos/nombre se validan con expresiones regulares.

### 4. Verificación rápida de errores

- **401** en cualquier GET = el token no se envió o no es válido.
- **423** en login = la cuenta quedó bloqueada por intentos fallidos (espera 15 min).

## Notas de seguridad

- El JWT se firma con `SecretKey` y el mismo `Issuer`/`Audience` en los 5 microservicios
  (`appsettings.json` → sección `Jwt`), con expiración de 60 minutos.
- El `Usuario` de la bitácora se obtiene del token (`context.User.Identity.Name`),
  nunca del cuerpo de la petición.
- Conexiones a MySQL con `SslMode=Required`.

## Solución de problemas

- **`Address already in use`**: el puerto está ocupado. Cambia `applicationUrl` en el
  `Properties/launchSettings.json` del proyecto (y la `Address` del cluster en
  `src\Gateway\...\appsettings.json` si cambiaste el puerto de un servicio).
- **401 en login**: usuario/password incorrectos.
- **500 en login o timeout**: no hay conexión a MySQL (revisa red / credenciales Aiven).
- **Gateway devuelve 502/503**: alguno de los microservicios no está corriendo.

## Colección Postman

Hay una colección lista para importar en `postman/ADMExpedientePersonal.postman_collection.json`
(usa variables `{{baseUrl}}` = `http://localhost:5080` y `{{token}}`).
