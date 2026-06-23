<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$id = (int)($_GET['id'] ?? 0);

if ($id > 0) {
    $grupo = Grupo::obtener($id);

    if ($grupo) {
        Grupo::eliminar($id);
        $_SESSION['mensaje'] = 'Grupo eliminado. Las tareas quedaron sin grupo.';
    } else {
        $_SESSION['mensaje'] = 'Grupo no encontrado.';
    }
}

header('Location: index.php');
exit;