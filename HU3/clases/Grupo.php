<?php
require_once __DIR__ . '/../config/db.php';

class Grupo
{
    public static function listar(): array
    {
        $db = getConexion();
        return $db->query("
            SELECT id, nombre, creado_en
            FROM grupos
            ORDER BY id DESC
        ")->fetchAll();
    }

    public static function obtener(int $id): ?array
    {
        $db = getConexion();
        $stmt = $db->prepare("SELECT * FROM grupos WHERE id = ?");
        $stmt->execute([$id]);
        $grupo = $stmt->fetch();
        return $grupo ?: null;
    }

    public static function tareasPendientes(): array
    {
        $db = getConexion();
        return $db->query("
            SELECT id, detalle, prioridad, fecha_limite
            FROM tareas
            WHERE estado = 'pendiente'
            ORDER BY id DESC
        ")->fetchAll();
    }

    public static function crear(string $nombre, array $tareas = []): bool
    {
        $db = getConexion();
        $db->beginTransaction();

        try {
            $stmt = $db->prepare("INSERT INTO grupos(nombre) VALUES (?)");
            $stmt->execute([$nombre]);

            $grupoId = $db->lastInsertId();

            foreach ($tareas as $tareaId) {
                $stmt = $db->prepare("UPDATE tareas SET grupo_id = ? WHERE id = ?");
                $stmt->execute([$grupoId, (int)$tareaId]);
            }

            $db->commit();
            return true;
        } catch (Exception $e) {
            $db->rollBack();
            return false;
        }
    }

    public static function editar(int $id, string $nombre): bool
    {
        $db = getConexion();
        $stmt = $db->prepare("UPDATE grupos SET nombre = ? WHERE id = ?");
        return $stmt->execute([$nombre, $id]);
    }

    public static function eliminar(int $id): bool
    {
        $db = getConexion();
        $db->beginTransaction();

        try {
            $stmt = $db->prepare("UPDATE tareas SET grupo_id = NULL WHERE grupo_id = ?");
            $stmt->execute([$id]);

            $stmt = $db->prepare("DELETE FROM grupos WHERE id = ?");
            $stmt->execute([$id]);

            $db->commit();
            return true;
        } catch (Exception $e) {
            $db->rollBack();
            return false;
        }
    }

    public static function tareasDelGrupo(int $id): array
    {
        $db = getConexion();
        $stmt = $db->prepare("
            SELECT 
                t.id,
                t.detalle,
                t.prioridad,
                t.estado,
                t.fecha_limite,
                COALESCE(CONCAT(r.nombre, ' ', r.apellidos), 'Sin responsable asignado') AS responsable
            FROM tareas t
            LEFT JOIN responsables r ON r.id = t.responsable_id
            WHERE t.grupo_id = ?
            ORDER BY (t.estado = 'finalizada') ASC, t.id DESC
        ");
        $stmt->execute([$id]);
        return $stmt->fetchAll();
    }

    public static function quitarTarea(int $tareaId): bool
    {
        $db = getConexion();
        $stmt = $db->prepare("UPDATE tareas SET grupo_id = NULL WHERE id = ?");
        return $stmt->execute([$tareaId]);
    }
}