create database SEG;
use SEG;
CREATE TABLE roles (
    id_rol INT AUTO_INCREMENT PRIMARY KEY,
    nombre_rol VARCHAR(40) NOT NULL UNIQUE
);
CREATE TABLE modulos (
    id_modulo INT AUTO_INCREMENT PRIMARY KEY,
    nombre_modulo VARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE roles_modulos (
    id_rol INT,
    id_modulo INT,
    PRIMARY KEY (id_rol, id_modulo),
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol),
    FOREIGN KEY (id_modulo) REFERENCES modulos(id_modulo)
);
create TABLE usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombreusuario VARCHAR(50) NOT NULL UNIQUE,
    nombre_completo VARCHAR(100) NOT NULL,
    correo VARCHAR(100) NOT NULL UNIQUE,
    password VARBINARY(255) NOT NULL, -- encrypted (AES-256-GCM)
    estado ENUM('Activo', 'Inactivo', 'Bloqueado') NOT NULL DEFAULT 'Activo'
);
create TABLE usuarios_roles (
    id_usuario INT,
    id_rol INT,
    PRIMARY KEY (id_usuario, id_rol),
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol)
);

create database OFE;
use OFE;
CREATE TABLE oferentes (
    identificacion VARCHAR(20) PRIMARY KEY,
    tipo_identificacion ENUM('Cedula', 'DIMEX', 'Pasaporte') NOT NULL,
    nombre_completo VARCHAR(100) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    contratado TINYINT(1) DEFAULT 0
);
CREATE TABLE oferente_emails (
    id INT AUTO_INCREMENT PRIMARY KEY,
    identificacion VARCHAR(20),
    email VARCHAR(100) NOT NULL,
    FOREIGN KEY (identificacion) REFERENCES oferentes(identificacion)
);
CREATE TABLE oferente_telefonos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    identificacion VARCHAR(20),
    telefono VARCHAR(20),
    FOREIGN KEY (identificacion) REFERENCES oferentes(identificacion)
);
CREATE TABLE concursos (
    codigo_concurso INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    estado ENUM('Vigente', 'Vencido') DEFAULT 'Vigente'
);
CREATE TABLE oferente_concursos (
    identificacion VARCHAR(20),
    codigo_concurso INT,
    PRIMARY KEY (identificacion, codigo_concurso),
    FOREIGN KEY (identificacion) REFERENCES oferentes(identificacion),
    FOREIGN KEY (codigo_concurso) REFERENCES concursos(codigo_concurso)
);
CREATE TABLE preparacion_acad (
    id INT AUTO_INCREMENT PRIMARY KEY,
    institucion VARCHAR(150) NOT NULL,
    oferente_id VARCHAR(20),
    titulo VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    FOREIGN KEY (oferente_id) REFERENCES oferentes(identificacion)
);
CREATE TABLE exp_laboral (
    id INT AUTO_INCREMENT PRIMARY KEY,
    empresa VARCHAR(100) NOT NULL,
    oferente_id VARCHAR(20),
    puesto VARCHAR(100) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    FOREIGN KEY (oferente_id) REFERENCES oferentes(identificacion)
);
CREATE TABLE entrevistas (
    entrevista_id INT AUTO_INCREMENT PRIMARY KEY,
    oferente_id VARCHAR(20),
    empleado_id INT,
    fecha_entrevista DATETIME,
    estado ENUM('Pendiente', 'Realizada', 'Eliminada') DEFAULT 'Pendiente',
    FOREIGN KEY (oferente_id) REFERENCES oferentes(identificacion),
    FOREIGN KEY (empleado_id) REFERENCES EMP.empleados(empleado_id)
);

create database EMP;
use EMP;
CREATE TABLE puestos (
    puesto_id INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    salario DECIMAL(10,2) NOT NULL,
    jefe_puesto_id INT NULL,
    FOREIGN KEY (jefe_puesto_id) REFERENCES puestos(puesto_id)
);
CREATE TABLE empleados (
    empleado_id INT AUTO_INCREMENT PRIMARY KEY,
    identificacion VARCHAR(20) UNIQUE,
    tipo_identificacion ENUM('Cedula', 'DIMEX', 'Pasaporte') NOT NULL,
    nombre_completo VARCHAR(100) NOT NULL,
    fecha_nacimiento DATE NOT NULL,
    puesto_id INT,
    FOREIGN KEY (puesto_id) REFERENCES puestos(puesto_id)
);
CREATE TABLE empleado_emails (
    id INT AUTO_INCREMENT PRIMARY KEY,
    empleado_id INT,
    email VARCHAR(100),
    FOREIGN KEY (empleado_id) REFERENCES empleados(empleado_id)
);
CREATE TABLE empleado_telefonos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    empleado_id INT,
    telefono VARCHAR(20),
    FOREIGN KEY (empleado_id) REFERENCES empleados(empleado_id)
);
CREATE TABLE requisitos_puestos (
    requisito_id INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);
CREATE TABLE admin_areas (
    codigo_area INT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    jefatura INT,
    FOREIGN KEY (jefatura) REFERENCES empleados(empleado_id)
);
CREATE TABLE acciones_personal (
    accion_id INT PRIMARY KEY,
    codigo_accion INT,
    fecha DATE NOT NULL,
    descripcion VARCHAR(500),
    empleado_id INT,
    jefatura_id INT,
    FOREIGN KEY (empleado_id) REFERENCES empleados(empleado_id),
    FOREIGN KEY (jefatura_id) REFERENCES empleados(empleado_id)
);
create database GEN;
use GEN;
CREATE TABLE parametros (
    codigo_parametro VARCHAR(50) PRIMARY KEY,
    valor VARCHAR(500) NOT NULL
);
CREATE TABLE compania (
    codigo_compania VARCHAR(50) PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL
);
CREATE TABLE inst_educativas (
    codigo_institucion VARCHAR(50) PRIMARY KEY,
    nombre VARCHAR(150) NOT NULL
);