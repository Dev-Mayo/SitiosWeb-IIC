<?php
// clases/Responsable.php

require_once __DIR__ . '/../config/db.php';

class Responsable {

    // ── Listar todos ────────────────────────────────────────
    public static function listar(): array {
        $sql = 'SELECT id, nombre, apellidos, identificacion
                FROM responsables
                ORDER BY apellidos, nombre';
        return getConexion()->query($sql)->fetchAll();
    }

    // ── Obtener uno por ID ───────────────────────────────────
    public static function obtener(int $id): ?array {
        $stmt = getConexion()->prepare(
            'SELECT id, nombre, apellidos, identificacion
             FROM responsables WHERE id = ?'
        );
        $stmt->execute([$id]);
        $fila = $stmt->fetch();
        return $fila ?: null;
    }

    // ── Crear ────────────────────────────────────────────────
    public static function crear(string $nombre, string $apellidos, string $identificacion): bool {
        $stmt = getConexion()->prepare(
            'INSERT INTO responsables (nombre, apellidos, identificacion)
             VALUES (?, ?, ?)'
        );
        return $stmt->execute([$nombre, $apellidos, $identificacion]);
    }

    // ── Editar ───────────────────────────────────────────────
    public static function editar(int $id, string $nombre, string $apellidos, string $identificacion): bool {
        $stmt = getConexion()->prepare(
            'UPDATE responsables
             SET nombre = ?, apellidos = ?, identificacion = ?
             WHERE id = ?'
        );
        return $stmt->execute([$nombre, $apellidos, $identificacion, $id]);
    }

    // ── Eliminar ─────────────────────────────────────────────
    // La FK de tareas usa ON DELETE SET NULL, así que las tareas
    // pasan a "Sin responsable asignado" automáticamente.
    public static function eliminar(int $id): bool {
        $stmt = getConexion()->prepare('DELETE FROM responsables WHERE id = ?');
        return $stmt->execute([$id]);
    }
}
