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

create database BIT;
use BIT;
CREATE TABLE bitacoras (
  id BIGINT AUTO_INCREMENT PRIMARY KEY,
  fecha DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  usuario VARCHAR(100) NOT NULL,
  accion ENUM('CREATE','READ','UPDATE','DELETE','ERROR') NOT NULL,
  descripcion JSON NOT NULL
);


USE SEG;

-- Roles
INSERT INTO roles (nombre_rol) VALUES 
('Administrador'),
('Reclutador');

-- Modulos
INSERT INTO modulos (nombre_modulo) VALUES
('Contratar'),
('Roles'),
('Pantallas'),
('Usuarios'),
('Oferentes'),
('Entrevistas'),
('Puestos'),
('Áreas'),
('Acciones Personal'),
('Bitácora'),
('Parámetros'),
('Compañías'),
('Ubicaciones'),
('Inst. Educativas');

-- Roles_Modulos
INSERT INTO roles_modulos (id_rol, id_modulo)
SELECT 1, id_modulo FROM modulos; -- Admin has all
INSERT INTO roles_modulos (id_rol, id_modulo)
SELECT 22, id_modulo FROM modulos
WHERE nombre_modulo IN (
    'Inst. Educativas',
    'Oferentes',
    'Concursos',
    'Entrevistas',
    'Contratar Empleado',
    'Puestos',
    'Áreas',
    'Acciones Personal'
);

-- Usuarios
INSERT INTO usuarios (nombreusuario, nombre_completo, correo, password, estado) VALUES
('CarPerez','Carlos Perez','admin@shiro.com',AES_ENCRYPT('1234','key'),'Activo'),
('MarLopez','Maria Lopez','maria@shiro.com',AES_ENCRYPT('1234','key'),'Activo');

-- Usuarios_Roles
INSERT INTO usuarios_roles VALUES
(3,1),
(2,2);

USE OFE;

-- Oferentes
INSERT INTO oferentes VALUES
('101010101','Cedula','Juan Ramirez','1995-05-10',0),
('102020202','DIMEX','Ana Torres','1998-08-20',0);

-- Emails
INSERT INTO oferente_emails (identificacion,email) VALUES
('101010101','juan@gmail.com'),
('102020202','ana@gmail.com');

-- Telefonos
INSERT INTO oferente_telefonos (identificacion,telefono) VALUES
('101010101','88888888'),
('102020202','77777777');

-- Concursos
INSERT INTO concursos VALUES
(1,'Concurso TI','2026-01-01','2026-06-01','Vigente'),
(2,'Concurso RRHH','2026-02-01','2026-07-01','Vigente');

-- Oferente_Concursos
INSERT INTO oferente_concursos VALUES
('101010101',1),
('102020202',2);

-- Preparacion Academica
INSERT INTO preparacion_acad (institucion,oferente_id,titulo,fecha_inicio,fecha_fin) VALUES
('UCR','101010101','Ingenieria Sistemas','2015-01-01','2020-01-01'),
('TEC','102020202','Administracion','2016-01-01','2021-01-01');

-- Experiencia Laboral
INSERT INTO exp_laboral (empresa,oferente_id,puesto,fecha_inicio,fecha_fin) VALUES
('IBM','101010101','Developer','2020-02-01','2023-01-01'),
('HP','102020202','HR Assistant','2021-02-01','2024-01-01');

USE EMP;

-- Puestos
INSERT INTO puestos VALUES
(1,'Gerente',2000,NULL),
(2,'Desarrollador',1200,1);

-- Empleados
INSERT INTO empleados (identificacion,tipo_identificacion,nombre_completo,fecha_nacimiento,puesto_id) VALUES
('201010222','Cedula','Luis Gomez','1990-03-10',1),
('202020333','Cedula','Sofia Vargas','1995-07-15',2);

-- Emails
INSERT INTO empleado_emails (empleado_id,email) VALUES
(1,'luis@empresa.com'),
(2,'sofia@empresa.com');

-- Telefonos
INSERT INTO empleado_telefonos (empleado_id,telefono) VALUES
(1,'66666666'),
(2,'55555555');

-- Requisitos
INSERT INTO requisitos_puestos VALUES
(1,'Titulo universitario'),
(2,'Experiencia minima 2 años');

-- Areas
INSERT INTO admin_areas VALUES
(1,'Tecnologia',1),
(2,'Recursos Humanos',2);

-- Acciones Personal
INSERT INTO acciones_personal VALUES
(1,1001,'2026-01-10','Contratacion inicial',1,1),
(2,1002,'2026-02-15','Cambio de puesto',2,1);

USE OFE;

INSERT INTO entrevistas (oferente_id,empleado_id,fecha_entrevista,estado) VALUES
('101010101',1,'2026-06-01 10:00:00','Pendiente'),
('102020202',2,'2026-06-02 14:00:00','Pendiente');

USE GEN;

-- Compañia
INSERT INTO compania VALUES
('C01','Servicios Medicos SA');

-- Instituciones
INSERT INTO inst_educativas VALUES
('UCR','Universidad de Costa Rica'),
('TEC','Tecnologico de Costa Rica');

-- Procedimientos almacenados

DELIMITER $$

CREATE PROCEDURE sp_validar_login (
    IN p_username VARCHAR(50),
    IN p_password VARCHAR(100)
)
BEGIN
    SELECT 
        id_usuario,
        nombreusuario,
        nombre_completo,
        correo,
        estado
    FROM usuarios
    WHERE nombreusuario = p_username
      AND password = AES_ENCRYPT(p_password, 'SEG_KEY_2026_32CHARS!!')
      AND estado = 'Activo';
END$$

DELIMITER ;

DELIMITER ;
USE SEG;
DELIMITER $$

CREATE PROCEDURE sp_obtener_modulos_por_usuario(IN p_id_usuario INT)
BEGIN
    SELECT DISTINCT m.id_modulo, m.nombre_modulo
    FROM modulos m
    INNER JOIN roles_modulos rm ON m.id_modulo = rm.id_modulo
    INNER JOIN usuarios_roles ur ON rm.id_rol = ur.id_rol
    WHERE ur.id_usuario = p_id_usuario;
END $$

DELIMITER ;

DELIMITER $$

CREATE PROCEDURE sp_crear_usuario (
    IN p_username VARCHAR(50),
    IN p_fullname VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_password VARCHAR(100),
    IN p_roles VARCHAR(200)
)
BEGIN
    DECLARE v_id_usuario INT;

    INSERT INTO usuarios (nombreusuario, nombre_completo, correo, password, estado)
    VALUES (
        p_username,
        p_fullname,
        p_email,
        AES_ENCRYPT(p_password, 'SEG_KEY_2026_32CHARS!!'),
        'Activo'
    );

    SET v_id_usuario = LAST_INSERT_ID();

    INSERT INTO usuarios_roles (id_usuario, id_rol)
    SELECT v_id_usuario, CAST(TRIM(value) AS UNSIGNED)
    FROM JSON_TABLE(
        CONCAT('["', REPLACE(p_roles, ',', '","'), '"]'),
        '$[*]' COLUMNS (value VARCHAR(10) PATH '$')
    ) AS roles_tabla;
END$$

CREATE PROCEDURE sp_listar_usuarios()
BEGIN
    SELECT 
        u.id_usuario,
        u.nombreusuario,
        u.nombre_completo,
        u.correo,
        u.estado,
        GROUP_CONCAT(r.nombre_rol ORDER BY r.nombre_rol SEPARATOR ', ') AS roles
    FROM usuarios u
    LEFT JOIN usuarios_roles ur ON u.id_usuario = ur.id_usuario
    LEFT JOIN roles r ON ur.id_rol = r.id_rol
    GROUP BY u.id_usuario, u.nombreusuario, u.nombre_completo, u.correo, u.estado
    ORDER BY u.nombre_completo;
END$$


CREATE PROCEDURE sp_obtener_usuario_por_id(IN p_id_usuario INT)
BEGIN
    SELECT 
        u.id_usuario,
        u.nombreusuario,
        u.nombre_completo,
        u.correo,
        u.estado,
        GROUP_CONCAT(ur.id_rol ORDER BY ur.id_rol SEPARATOR ',') AS id_roles
    FROM usuarios u
    LEFT JOIN usuarios_roles ur ON u.id_usuario = ur.id_usuario
    WHERE u.id_usuario = p_id_usuario
    GROUP BY u.id_usuario, u.nombreusuario, u.nombre_completo, u.correo, u.estado;
END$$

-- UPDATE
CREATE PROCEDURE sp_actualizar_usuario (
    IN p_id_usuario INT,
    IN p_username VARCHAR(50),
    IN p_fullname VARCHAR(100),
    IN p_email VARCHAR(100),
    IN p_estado ENUM('Activo', 'Inactivo', 'Bloqueado'),
    IN p_roles VARCHAR(200),
    IN p_password VARCHAR(100)
)
BEGIN
    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

        UPDATE usuarios
        SET
            nombreusuario   = COALESCE(NULLIF(p_username, ''), nombreusuario),
            nombre_completo = COALESCE(NULLIF(p_fullname, ''), nombre_completo),
            correo          = COALESCE(NULLIF(p_email, ''), correo),
            estado          = COALESCE(p_estado, estado),
            password        = IF(p_password IS NOT NULL AND p_password != '',
                                AES_ENCRYPT(p_password, 'SEG_KEY_2026_32CHARS!!'),
                                password)
        WHERE id_usuario = p_id_usuario;

        DELETE FROM usuarios_roles WHERE id_usuario = p_id_usuario;

        INSERT INTO usuarios_roles (id_usuario, id_rol)
        SELECT p_id_usuario, CAST(TRIM(value) AS UNSIGNED)
        FROM JSON_TABLE(
            CONCAT('["', REPLACE(p_roles, ',', '","'), '"]'),
            '$[*]' COLUMNS (value VARCHAR(10) PATH '$')
        ) AS roles_tabla;

    COMMIT;
END$$

DELIMITER $$

DELIMITER $$

CREATE PROCEDURE sp_eliminar_usuario(IN p_id_usuario INT)
BEGIN
    DECLARE v_nombre_completo VARCHAR(100);
    DECLARE v_count INT;

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    SELECT nombre_completo INTO v_nombre_completo
    FROM SEG.usuarios WHERE id_usuario = p_id_usuario;

    SELECT COUNT(*) INTO v_count
    FROM BIT.bitacoras WHERE usuario = v_nombre_completo LIMIT 1;

    IF v_count > 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se puede eliminar un registro con datos relacionados.';
    END IF;

    START TRANSACTION;
        DELETE FROM SEG.usuarios_roles WHERE id_usuario = p_id_usuario;
        DELETE FROM SEG.usuarios WHERE id_usuario = p_id_usuario;
    COMMIT;

END$$

DELIMITER ;
USE BIT;
DELIMITER $$

CREATE PROCEDURE sp_listar_bitacoras(
    IN p_usuario VARCHAR(100),
    IN p_descripcion VARCHAR(500),
    IN p_orden VARCHAR(50)
)
BEGIN
    SELECT id, fecha, usuario, accion, 
           CAST(descripcion AS CHAR) AS descripcion
    FROM BIT.bitacoras
    WHERE
        (p_usuario IS NULL OR usuario LIKE CONCAT('%', p_usuario, '%'))
        AND (p_descripcion IS NULL OR CAST(descripcion AS CHAR) 
             LIKE CONCAT('%', p_descripcion, '%'))
    ORDER BY
        CASE WHEN p_orden = 'fecha_asc'    THEN fecha   END ASC,
        CASE WHEN p_orden = 'usuario_asc'  THEN usuario END ASC,
        CASE WHEN p_orden = 'usuario_desc' THEN usuario END DESC,
        CASE WHEN p_orden = 'fecha_desc' 
              OR p_orden IS NULL            THEN fecha   END DESC
    LIMIT 100;
END$$

DELIMITER ;

USE GEN;
DELIMITER $$

CREATE PROCEDURE sp_listar_inst_educativas()
BEGIN
    SELECT codigo_institucion, nombre
    FROM GEN.inst_educativas
    ORDER BY nombre;
END$$

CREATE PROCEDURE sp_crear_inst_educativa(
    IN p_codigo VARCHAR(50),
    IN p_nombre VARCHAR(150)
)
BEGIN
    INSERT INTO GEN.inst_educativas (codigo_institucion, nombre)
    VALUES (p_codigo, p_nombre);
END$$

CREATE PROCEDURE sp_actualizar_inst_educativa(
    IN p_codigo VARCHAR(50),
    IN p_nombre VARCHAR(150)
)
BEGIN
    UPDATE GEN.inst_educativas
    SET nombre = COALESCE(NULLIF(p_nombre, ''), nombre)
    WHERE codigo_institucion = p_codigo;
END$$

DELIMITER $$

CREATE PROCEDURE sp_eliminar_inst_educativa(
    IN p_codigo VARCHAR(50)
)
BEGIN
    
    IF EXISTS (
        SELECT 1 FROM OFE.preparacion_acad
        WHERE codigo_institucion = p_codigo
        LIMIT 1
    ) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se puede eliminar un registro con datos relacionados.';
    END IF;

    DELETE FROM GEN.inst_educativas
    WHERE codigo_institucion = p_codigo;
END$$

DELIMITER ;