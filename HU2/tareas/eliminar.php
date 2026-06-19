<?php
session_start();
require_once __DIR__ . '/../clases/Tarea.php';

$id = (int)($_GET['id'] ?? 0);

if ($id > 0) {
    try {
        Tarea::eliminar($id);
        $_SESSION['mensaje'] = 'Tarea eliminada correctamente.';
    } catch (PDOException $e) {
        $_SESSION['mensaje'] = 'Error al eliminar la tarea.';
    }
}

header('Location: index.php');
exit;
