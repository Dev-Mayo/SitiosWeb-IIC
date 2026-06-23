<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$id = (int)($_GET['id'] ?? 0);

if ($id > 0) {
    Grupo::eliminar($id);
    $_SESSION['mensaje'] = 'Grupo eliminado. Las tareas quedaron sin grupo.';
}

header('Location: index.php');
exit;