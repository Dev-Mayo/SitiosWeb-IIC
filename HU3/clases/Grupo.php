<?php
require_once __DIR__ . '/../config/db.php';

class Grupo
{
    public static function listar(): array
    {
        $db = getConexion();
        $stmt = $db->query(
            "SELECT id, nombre, creado_en
             FROM grupos
             ORDER BY id DESC"
        );

        return $stmt->fetchAll();
    }

    public static function obtener(int $id): ?array
    {
        $db = getConexion();
        $stmt = $db->prepare(
            "SELECT id, nombre, creado_en
             FROM grupos
             WHERE id = ?"
        );
        $stmt->execute([$id]);

        $grupo = $stmt->fetch();
        return $grupo ?: null;
    }

    public static function crear(string $nombre): bool
    {
        $db = getConexion();
        $stmt = $db->prepare(
            "INSERT INTO grupos (nombre)
             VALUES (?)"
        );

        return $stmt->execute([$nombre]);
    }

    public static function editar(int $id, string $nombre): bool
    {
        $db = getConexion();
        $stmt = $db->prepare(
            "UPDATE grupos
             SET nombre = ?
             WHERE id = ?"
        );

        return $stmt->execute([$nombre, $id]);
    }

    public static function eliminar(int $id): bool
    {
        $db = getConexion();
        $stmt = $db->prepare(
            "DELETE FROM grupos
             WHERE id = ?"
        );

        return $stmt->execute([$id]);
    }
}