
-- =========================================
-- Oferentes
-- =========================================
use OFE;

DELIMITER $$
CREATE PROCEDURE sp_obtener_oferentes(IN p_identificacion VARCHAR(20))
BEGIN
    IF p_identificacion IS NOT NULL THEN
        -- Un solo oferente
        SELECT o.identificacion as Identificacion, o.tipo_identificacion as TipoIdentificacion, o.nombre_completo as NombreCompleto, 
               o.fecha_nacimiento as FechaNacimiento, o.contratado as Contratado,
               e.email as Email, t.telefono as Telefono, c.codigo_concurso as CodigoConcurso
        FROM oferentes o
        LEFT JOIN oferente_emails e ON o.identificacion = e.identificacion
        LEFT JOIN oferente_telefonos t ON o.identificacion = t.identificacion
        LEFT JOIN oferente_concursos c ON o.identificacion = c.identificacion
        WHERE o.identificacion = p_identificacion;
    ELSE
        -- Todos los oferentes
        SELECT o.identificacion as Identificacion, o.tipo_identificacion as TipoIdentificacion, o.nombre_completo as NombreCompleto, 
               o.fecha_nacimiento as FechaNacimiento, o.contratado as Contratado,
               e.email as Email, t.telefono as Telefono, c.codigo_concurso as CodigoConcurso
        FROM oferentes o
        LEFT JOIN oferente_emails e ON o.identificacion = e.identificacion
        LEFT JOIN oferente_telefonos t ON o.identificacion = t.identificacion
        LEFT JOIN oferente_concursos c ON o.identificacion = c.identificacion;
    END IF;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_obtener_nombre_oferentes()
BEGIN
        -- Todos los oferentes
        SELECT identificacion, nombre_completo
        FROM oferentes;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_GestionarOferente( -- devuelve 0:error 1:exito 2:ya asignado 3:ya existe
    IN accion INT, -- 0:Crear 1:Modificar 2:Eliminar
    IN p_identificacion VARCHAR(20),
    IN p_tipo_identificacion ENUM('Cedula','DIMEX','Pasaporte'),
    IN p_nombre_completo VARCHAR(100),
    IN p_fecha_nacimiento DATE,
    IN p_contratado TINYINT,
    IN p_emails TEXT,       -- lista separada por comas
    IN p_telefonos TEXT,    -- lista separada por comas
    IN p_concursos TEXT     -- lista separada por comas
)
proc: BEGIN
    DECLARE existe INT;
    DECLARE utilizado INT;

    -- CREAR
    IF accion = 0 THEN
        SELECT COUNT(*) INTO existe FROM oferentes WHERE identificacion = p_identificacion;
        IF existe > 0 THEN
            SELECT 3; -- Ya existe
        ELSE
            INSERT INTO oferentes(identificacion,tipo_identificacion,nombre_completo,fecha_nacimiento,contratado)
            VALUES(p_identificacion,p_tipo_identificacion,p_nombre_completo,p_fecha_nacimiento,p_contratado);

            -- Insertar correos
            IF p_emails IS NOT NULL AND p_emails <> '' THEN
                INSERT INTO oferente_emails(identificacion,email)
                SELECT p_identificacion, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_emails, ',', n.n), ',', -1))
                FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
                WHERE n.n <= 1 + LENGTH(p_emails) - LENGTH(REPLACE(p_emails, ',', ''));
            END IF;

            -- Insertar teléfonos
            IF p_telefonos IS NOT NULL AND p_telefonos <> '' THEN
                INSERT INTO oferente_telefonos(identificacion,telefono)
                SELECT p_identificacion, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_telefonos, ',', n.n), ',', -1))
                FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
                WHERE n.n <= 1 + LENGTH(p_telefonos) - LENGTH(REPLACE(p_telefonos, ',', ''));
            END IF;

            -- Insertar concursos
            IF p_concursos IS NOT NULL AND p_concursos <> '' THEN
                INSERT INTO oferente_concursos(identificacion,codigo_concurso)
                SELECT p_identificacion, CAST(TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_concursos, ',', n.n), ',', -1)) AS UNSIGNED)
                FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
                WHERE n.n <= 1 + LENGTH(p_concursos) - LENGTH(REPLACE(p_concursos, ',', ''));
            END IF;

            SELECT 1; -- Éxito
        END IF;

    -- MODIFICAR
    ELSEIF accion = 1 THEN
        UPDATE oferentes
        SET tipo_identificacion = p_tipo_identificacion,
            nombre_completo = p_nombre_completo,
            fecha_nacimiento = p_fecha_nacimiento,
            contratado = p_contratado
        WHERE identificacion = p_identificacion;

        -- Limpiar y volver a insertar correos
        DELETE FROM oferente_emails WHERE identificacion = p_identificacion;
        IF p_emails IS NOT NULL AND p_emails <> '' THEN
            INSERT INTO oferente_emails(identificacion,email)
            SELECT p_identificacion, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_emails, ',', n.n), ',', -1))
            FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
            WHERE n.n <= 1 + LENGTH(p_emails) - LENGTH(REPLACE(p_emails, ',', ''));
        END IF;

        -- Limpiar y volver a insertar teléfonos
        DELETE FROM oferente_telefonos WHERE identificacion = p_identificacion;
        IF p_telefonos IS NOT NULL AND p_telefonos <> '' THEN
            INSERT INTO oferente_telefonos(identificacion,telefono)
            SELECT p_identificacion, TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_telefonos, ',', n.n), ',', -1))
            FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
            WHERE n.n <= 1 + LENGTH(p_telefonos) - LENGTH(REPLACE(p_telefonos, ',', ''));
        END IF;

        -- Limpiar y volver a insertar concursos
        DELETE FROM oferente_concursos WHERE identificacion = p_identificacion;
        IF p_concursos IS NOT NULL AND p_concursos <> '' THEN
            INSERT INTO oferente_concursos(identificacion,codigo_concurso)
            SELECT p_identificacion, CAST(TRIM(SUBSTRING_INDEX(SUBSTRING_INDEX(p_concursos, ',', n.n), ',', -1)) AS UNSIGNED)
            FROM (SELECT 1 n UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) n
            WHERE n.n <= 1 + LENGTH(p_concursos) - LENGTH(REPLACE(p_concursos, ',', ''));
        END IF;

        SELECT 1;

    -- ELIMINAR
    ELSEIF accion = 2 THEN
        SELECT COUNT(*) INTO utilizado FROM oferente_concursos WHERE identificacion = p_identificacion;
		IF utilizado > 0 THEN SELECT 2; leave proc; END IF;
		SELECT COUNT(*) INTO utilizado FROM entrevistas WHERE oferente_id = p_identificacion;
		IF utilizado > 0 THEN SELECT 2; leave proc; END IF;
		SELECT COUNT(*) INTO utilizado FROM exp_laboral WHERE oferente_id = p_identificacion;
		IF utilizado > 0 THEN SELECT 2; leave proc; END IF;
		SELECT COUNT(*) INTO utilizado FROM preparacion_acad WHERE oferente_id = p_identificacion;
		IF utilizado > 0 THEN SELECT 2; leave proc; END IF;
        
        Delete from oferente_telefonos where identificacion = p_identificacion;
        delete from oferente_emails where identificacion = p_identificacion;
        DELETE FROM oferentes WHERE identificacion = p_identificacion;
        IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;

    ELSE
        SELECT 0;
    END IF;
END$$
DELIMITER $$

-- =========================================
-- Concursos
-- =========================================
DELIMITER $$
CREATE PROCEDURE sp_ObtenerConcursos(IN p_identificacion VARCHAR(20))
BEGIN
    IF p_identificacion IS NULL THEN
        SELECT * FROM concursos;
    ELSE
        SELECT c.* FROM concursos c
        INNER JOIN oferente_concursos oc ON c.codigo_concurso = oc.codigo_concurso
        WHERE oc.identificacion = p_identificacion;
    END IF;
END$$
DELIMITER $$

-- =========================================
-- Preparación Académica
-- =========================================
DELIMITER $$
CREATE PROCEDURE sp_ObtenerPreparacionAcad(IN p_identificacion VARCHAR(20))
BEGIN
    SELECT pa.id,
           pa.codigo_institucion,
           i.nombre AS institucion,
           pa.titulo,
           pa.fecha_inicio,
           pa.fecha_fin
    FROM preparacion_acad pa
    INNER JOIN GEN.inst_educativas i 
        ON pa.codigo_institucion = i.codigo_institucion
    WHERE pa.oferente_id = p_identificacion
    ORDER BY pa.fecha_inicio ASC;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_ObtenerPreparacionAcadPorId(IN p_id INT)
BEGIN
    SELECT pa.id,
           pa.codigo_institucion,
           i.nombre AS institucion,
           pa.oferente_id,
           pa.titulo,
           pa.fecha_inicio,
           pa.fecha_fin
    FROM preparacion_acad pa
    INNER JOIN GEN.inst_educativas i 
        ON pa.codigo_institucion = i.codigo_institucion
    WHERE pa.id = p_id;
END$$
DELIMITER ;


DELIMITER $$
CREATE PROCEDURE sp_CrearPreparacionAcad( 
    IN p_codigo_institucion VARCHAR(50),
    IN p_identificacion VARCHAR(20),
    IN p_titulo VARCHAR(100),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
	IF p_titulo REGEXP '^[A-Za-z ]+$' AND p_fecha_fin >= p_fecha_inicio THEN
		INSERT INTO preparacion_acad(codigo_institucion,oferente_id,titulo,fecha_inicio,fecha_fin)
		VALUES(p_codigo_institucion,p_identificacion,p_titulo,p_fecha_inicio,p_fecha_fin);
		SELECT 1;
	ELSE
		SELECT 0;
	end if;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_ModificarPreparacionAcad( -- separado para evitar enrredo de parametros
    IN p_id INT,
    in p_codigo_institucion VARCHAR(50),
    IN p_titulo VARCHAR(100),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    IF p_titulo REGEXP '^[A-Za-z ]+$' AND p_fecha_fin >= p_fecha_inicio THEN
        UPDATE preparacion_acad
        SET titulo = p_titulo, fecha_inicio = p_fecha_inicio, fecha_fin = p_fecha_fin, codigo_institucion = p_codigo_institucion
        WHERE id = p_id;
        IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
    ELSE
        SELECT 0;
    END IF;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_EliminarPreparacionAcad(IN p_id INT) -- separado por comodidad
BEGIN
    DECLARE v_count INT;
    SELECT COUNT(*) INTO v_count FROM preparacion_acad WHERE id = p_id AND oferente_id IS NOT NULL;
    IF v_count > 0 THEN SELECT 2;
    ELSE
        DELETE FROM preparacion_acad WHERE id = p_id;
        IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
    END IF;
END$$
DELIMITER $$

-- =========================================
-- Experiencia Laboral
-- =========================================
DELIMITER $$
CREATE PROCEDURE sp_ObtenerExpLaboral(IN p_identificacion VARCHAR(20))
BEGIN
    SELECT * 
    FROM exp_laboral 
    WHERE oferente_id = p_oferente_id
    ORDER BY fecha_inicio ASC;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_CrearExpLaboral(
    IN p_empresa VARCHAR(100),
    IN p_identificacion VARCHAR(20),
    IN p_puesto VARCHAR(100),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    IF p_empresa REGEXP '^[A-Za-z0-9 ]+$' AND p_puesto REGEXP '^[A-Za-z0-9 ]+$' AND p_fecha_fin >= p_fecha_inicio THEN
        INSERT INTO exp_laboral(empresa,oferente_id,puesto,fecha_inicio,fecha_fin)
        VALUES(p_empresa,p_identificacion,p_puesto,p_fecha_inicio,p_fecha_fin);
        SELECT 1;
    ELSE
        SELECT 0;
    END IF;
END$$

CREATE PROCEDURE sp_ModificarExpLaboral(
    IN p_id INT,
    IN p_empresa VARCHAR(100),
    IN p_puesto VARCHAR(100),
    IN p_fecha_inicio DATE,
    IN p_fecha_fin DATE
)
BEGIN
    IF p_empresa REGEXP '^[A-Za-z0-9 ]+$' AND p_puesto REGEXP '^[A-Za-z0-9 ]+$' AND p_fecha_fin >= p_fecha_inicio THEN
        UPDATE exp_laboral
        SET empresa = p_empresa, puesto = p_puesto, fecha_inicio = p_fecha_inicio, fecha_fin = p_fecha_fin
        WHERE id = p_id;
        IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
    ELSE
        SELECT 0;
    END IF;
END$$

CREATE PROCEDURE sp_EliminarExpLaboral(IN p_id INT)
BEGIN
    DECLARE v_count INT;
    SELECT COUNT(*) INTO v_count FROM exp_laboral WHERE id = p_id AND oferente_id IS NOT NULL;
    IF v_count > 0 THEN SELECT 2;
    ELSE
        DELETE FROM exp_laboral WHERE id = p_id;
        IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
    END IF;
END$$
DELIMITER $$

-- =========================================
-- Entrevistas
-- =========================================
DELIMITER $$
CREATE PROCEDURE sp_ObtenerEntrevistas(IN p_EntrevistaId int)
BEGIN
    IF p_EntrevistaId IS NULL THEN
        SELECT * 
        FROM entrevistas 
        ORDER BY fecha_entrevista ASC;
    ELSE
        SELECT * 
        FROM entrevistas 
        WHERE entrevista_id = p_EntrevistaId
        ORDER BY fecha_entrevista ASC;
    END IF;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_CrearEntrevista(
    IN p_identificacion VARCHAR(20),
    IN p_empleado_id INT,
    IN p_fecha DATETIME
)
BEGIN
    INSERT INTO entrevistas(oferente_id,empleado_id,fecha_entrevista)
    VALUES(p_identificacion,p_empleado_id,p_fecha);
    SELECT 1;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE ModificarEntrevista(
    IN p_entrevista_id INT,
    IN p_empleado_id INT,
    IN p_fecha DATETIME
)
BEGIN
    UPDATE entrevistas
    SET empleado_id = p_empleado_id, fecha_entrevista = p_fecha
    WHERE entrevista_id = p_entrevista_id;
    IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_EliminarEntrevista(IN p_entrevista_id INT)
BEGIN
    DELETE FROM entrevistas WHERE entrevista_id = p_entrevista_id;
    IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
END$$
DELIMITER $$

DELIMITER $$
CREATE PROCEDURE sp_ModificarEstadoEntrevista(IN p_EntrevistaId INT)
BEGIN
	if ((select estado from entrevistas where entrevista_id = p_EntrevistaId) = 'Realizada') then 
		select 2;
    else
		UPDATE entrevistas SET estado = 'Realizada' WHERE entrevista_id = p_EntrevistaId;
		IF ROW_COUNT() > 0 THEN SELECT 1; ELSE SELECT 0; END IF;
    end if;
END$$
DELIMITER $$








