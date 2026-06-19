<?php

require_once __DIR__ . '/../config/db.php';

class Tarea {
    public static function listar(): array {
        $sql = 'SELECT *
                FROM vista_tareas';
        return getConexion()->query($sql)->fetchAll();
    }

    public static function obtener(int $id): ?array {
        $stmt = getConexion()->prepare(
            'SELECT id, detalle, prioridad, estado, fecha_limite, responsable_id
             FROM tareas WHERE id = ?'
        );
        $stmt->execute([$id]);
        $fila = $stmt->fetch();
        return $fila ?: null;
    }

    public static function listarResponsables(): array {
        $sql = 'SELECT id, nombre, apellidos
                FROM responsables
                ORDER BY apellidos, nombre';
        return getConexion()->query($sql)->fetchAll();
    }

    public static function crear(string $detalle, string $prioridad, ?string $fecha_limite, ?int $responsable_id): bool {
        $stmt = getConexion()->prepare(
            'INSERT INTO tareas (detalle, prioridad, estado, fecha_limite, responsable_id)
             VALUES (?, ?, "pendiente", ?, ?)'
        );
        return $stmt->execute([
            $detalle,
            $prioridad,
            $fecha_limite,
            $responsable_id,
        ]);
    }

    public static function editar(int $id, string $detalle, string $prioridad, ?string $fecha_limite, ?int $responsable_id): bool {
        $stmt = getConexion()->prepare(
            'UPDATE tareas
             SET detalle = ?, prioridad = ?, fecha_limite = ?, responsable_id = ?
             WHERE id = ?'
        );
        return $stmt->execute([
            $detalle,
            $prioridad,
            $fecha_limite,
            $responsable_id,
            $id,
        ]);
    }

    public static function eliminar(int $id): bool {
        $stmt = getConexion()->prepare('DELETE FROM tareas WHERE id = ?');
        return $stmt->execute([$id]);
    }

    public static function cambiarEstado(int $id, string $estado): bool {
        $stmt = getConexion()->prepare(
            'CALL cambiar_estado(:id, :estado)'
        );
        return $stmt->execute([
            ':id' => $id,
            ':estado' => $estado,
        ]);
    }
}
