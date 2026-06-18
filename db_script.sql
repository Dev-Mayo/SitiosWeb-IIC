-- ============================================================
-- TAREA CORTA 2 - Colegio Universitario de Cartago
-- Sistema de Control de Tareas
-- Script de Base de Datos - MySQL
-- ============================================================

CREATE DATABASE control_tareas;

USE control_tareas;

-- ============================================================
-- TABLA: responsables
-- Almacena las personas que pueden ser asignadas a tareas.
-- ============================================================
CREATE TABLE responsables (
  id            INT UNSIGNED    NOT NULL AUTO_INCREMENT,
  nombre        VARCHAR(100)    NOT NULL,
  apellidos     VARCHAR(150)    NOT NULL,
  identificacion VARCHAR(30)   NOT NULL,
  creado_en     DATETIME        NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  UNIQUE KEY uq_responsable_identificacion (identificacion)
) ENGINE=InnoDB;

-- ============================================================
-- TABLA: grupos
-- Agrupa tareas bajo un nombre común.
-- ============================================================
CREATE TABLE grupos (
  id        INT UNSIGNED  NOT NULL AUTO_INCREMENT,
  nombre    VARCHAR(150)  NOT NULL,
  creado_en DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id)
) ENGINE=InnoDB;

-- ============================================================
-- TABLA: tareas
-- Núcleo del sistema; almacena cada tarea con su estado,
-- prioridad, responsable y grupo opcionales.
-- ============================================================
CREATE TABLE tareas (
  id               INT UNSIGNED   NOT NULL AUTO_INCREMENT,
  detalle          TEXT           NOT NULL,
  prioridad        ENUM('baja','media','alta') NOT NULL DEFAULT 'media',
  estado           ENUM('pendiente','en_progreso','bloqueada','finalizada') NOT NULL DEFAULT 'pendiente',
  fecha_limite     DATE                   NULL,
  fecha_finalizacion DATETIME             NULL,
  responsable_id   INT UNSIGNED           NULL,
  grupo_id         INT UNSIGNED           NULL,
  creado_en        DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP,
  actualizado_en   DATETIME       NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT fk_tarea_responsable
    FOREIGN KEY (responsable_id) REFERENCES responsables(id)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT fk_tarea_grupo
    FOREIGN KEY (grupo_id) REFERENCES grupos(id)
    ON DELETE SET NULL
    ON UPDATE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- TABLA: historial_estados
-- Registra cada cambio de estado de una tarea (auditoría).
-- ============================================================
CREATE TABLE historial_estados (
  id           INT UNSIGNED NOT NULL AUTO_INCREMENT,
  tarea_id     INT UNSIGNED NOT NULL,
  estado_desde ENUM('pendiente','en_progreso','bloqueada','finalizada') NULL,
  estado_hasta ENUM('pendiente','en_progreso','bloqueada','finalizada') NOT NULL,
  cambiado_en  DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (id),
  CONSTRAINT fk_historial_tarea
    FOREIGN KEY (tarea_id) REFERENCES tareas(id)
    ON DELETE CASCADE
) ENGINE=InnoDB;

-- ============================================================
-- TRIGGER: al insertar una tarea, registra el estado inicial
-- ============================================================
DELIMITER $$
CREATE TRIGGER trg_tarea_insert
AFTER INSERT ON tareas
FOR EACH ROW
BEGIN
  INSERT INTO historial_estados (tarea_id, estado_desde, estado_hasta, cambiado_en)
  VALUES (NEW.id, NULL, NEW.estado, NOW());
END$$
DELIMITER ;

-- ============================================================
-- TRIGGER: al cambiar el estado, valida la transición y
--          gestiona fecha_finalizacion.
-- ============================================================
DELIMITER $$
CREATE TRIGGER trg_tarea_update
BEFORE UPDATE ON tareas
FOR EACH ROW
BEGIN
  -- Validar transiciones de estado permitidas
  IF OLD.estado != NEW.estado THEN
    IF NOT (
         (OLD.estado = 'pendiente'    AND NEW.estado = 'en_progreso')
      OR (OLD.estado = 'en_progreso'  AND NEW.estado = 'pendiente')
      OR (OLD.estado = 'en_progreso'  AND NEW.estado = 'bloqueada')
      OR (OLD.estado = 'bloqueada'    AND NEW.estado = 'en_progreso')
      OR (OLD.estado = 'en_progreso'  AND NEW.estado = 'finalizada')
      OR (OLD.estado = 'finalizada'   AND NEW.estado = 'en_progreso')  -- reactivar
    ) THEN
      SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Transición de estado no permitida.';
    END IF;

    -- Asignar fecha de finalización automáticamente
    IF NEW.estado = 'finalizada' THEN
      SET NEW.fecha_finalizacion = NOW();
    END IF;

    -- Limpiar fecha de finalización al reactivar
    IF OLD.estado = 'finalizada' AND NEW.estado != 'finalizada' THEN
      SET NEW.fecha_finalizacion = NULL;
    END IF;
  END IF;
END$$
DELIMITER ;

-- ============================================================
-- TRIGGER: después de actualizar estado, guarda historial
-- ============================================================
DELIMITER $$
CREATE TRIGGER trg_tarea_update_historial
AFTER UPDATE ON tareas
FOR EACH ROW
BEGIN
  IF OLD.estado != NEW.estado THEN
    INSERT INTO historial_estados (tarea_id, estado_desde, estado_hasta, cambiado_en)
    VALUES (NEW.id, OLD.estado, NEW.estado, NOW());
  END IF;
END$$
DELIMITER ;

-- ============================================================
-- VISTA: vista_tareas
-- Presenta las tareas con nombre completo del responsable,
-- nombre del grupo y etiqueta "Sin responsable asignado".
-- ============================================================
CREATE VIEW vista_tareas AS
SELECT
  t.id,
  t.detalle,
  t.prioridad,
  t.estado,
  t.fecha_limite,
  t.fecha_finalizacion,
  t.creado_en,
  t.actualizado_en,
  COALESCE(
    CONCAT(r.nombre, ' ', r.apellidos),
    'Sin responsable asignado'
  ) AS responsable,
  t.responsable_id,
  g.nombre  AS grupo,
  t.grupo_id
FROM tareas t
LEFT JOIN responsables r ON r.id = t.responsable_id
LEFT JOIN grupos       g ON g.id = t.grupo_id
ORDER BY
  (t.estado = 'finalizada') ASC,   -- finalizadas al final
  t.creado_en DESC;

-- ============================================================
-- PROCEDIMIENTO: cambiar_estado
-- Centraliza el cambio de estado con validación.
-- ============================================================
DELIMITER $$
CREATE PROCEDURE cambiar_estado(IN p_tarea_id INT, IN p_nuevo_estado VARCHAR(20))
BEGIN
  UPDATE tareas
  SET estado = p_nuevo_estado
  WHERE id = p_tarea_id;
END$$
DELIMITER ;

-- ============================================================
-- PROCEDIMIENTO: eliminar_responsable
-- Elimina un responsable; las tareas quedan sin responsable.
-- ============================================================
DELIMITER $$
CREATE PROCEDURE eliminar_responsable(IN p_id INT)
BEGIN
  -- La FK usa ON DELETE SET NULL, así que solo hace falta borrar
  DELETE FROM responsables WHERE id = p_id;
END$$
DELIMITER ;

-- ============================================================
-- FUNCIÓN: contar_tareas_responsable
-- Devuelve cuántas tareas activas tiene un responsable.
-- ============================================================
DELIMITER $$
CREATE FUNCTION contar_tareas_responsable(p_responsable_id INT)
RETURNS INT
READS SQL DATA
DETERMINISTIC
BEGIN
  DECLARE total INT;
  SELECT COUNT(*) INTO total
  FROM tareas
  WHERE responsable_id = p_responsable_id
    AND estado != 'finalizada';
  RETURN total;
END$$
DELIMITER ;

-- ============================================================
-- Datos de ejemplo (opcionales)
-- ============================================================
INSERT INTO responsables (nombre, apellidos, identificacion) VALUES
  ('Ana',   'Mora Jiménez',  '101110111'),
  ('Luis',  'Vargas Solano', '202220222'),
  ('María', 'Salas Quesada', '303330333');

INSERT INTO grupos (nombre) VALUES
  ('Sprint 1'),
  ('Backlog'),
  ('Urgentes');

INSERT INTO tareas (detalle, prioridad, estado, fecha_limite, responsable_id, grupo_id) VALUES
  ('Diseñar pantalla principal', 'alta',  'pendiente',   '2026-06-30', 1, 1),
  ('Configurar servidor',        'media', 'en_progreso', NULL,         2, 1),
  ('Revisar documentación',      'baja',  'pendiente',   '2026-07-10', NULL, 2);
