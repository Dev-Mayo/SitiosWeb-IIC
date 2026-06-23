<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$tareaId = (int)($_GET['id'] ?? 0);
$grupoId = (int)($_GET['grupo'] ?? 0);

if ($tareaId > 0) {
    Grupo::quitarTarea($tareaId);
    $_SESSION['mensaje'] = 'Tarea quitada del grupo correctamente.';
}

header('Location: ver.php?id=' . $grupoId);
exit;