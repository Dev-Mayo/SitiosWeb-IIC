<?php
session_start();
require_once __DIR__ . '/../clases/Tarea.php';

if (!isset($_GET['id']) || !isset($_GET['estado'])) {
    $_SESSION['mensaje'] = 'Datos incompletos.';
    header('Location: index.php');
    exit;
}

$id = (int)($_GET['id'] ?? 0);
$estado = $_GET['estado'];

$permitidos = ['pendiente', 'en_progreso', 'bloqueada', 'finalizada'];
if (!in_array($estado, $permitidos, true)) {
    $_SESSION['mensaje'] = 'Estado no válido.';
    header('Location: index.php');
    exit;
}

try {
    Tarea::cambiarEstado($id, $estado);
    $_SESSION['mensaje'] = 'Estado actualizado correctamente.';
} catch (PDOException $e) {
    $_SESSION['mensaje'] = 'Error al actualizar el estado.';
}

header('Location: index.php');
exit;
