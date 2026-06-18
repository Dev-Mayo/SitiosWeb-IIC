<?php
// responsables/eliminar.php
session_start();
require_once __DIR__ . '/../clases/Responsable.php';

$id = (int)($_GET['id'] ?? 0);

if ($id > 0) {
    $responsable = Responsable::obtener($id);
    if ($responsable) {
        Responsable::eliminar($id);
        $_SESSION['mensaje'] = 'Responsable eliminado. Las tareas que tenía asignadas quedaron sin responsable.';
    } else {
        $_SESSION['mensaje'] = 'Responsable no encontrado.';
    }
}

header('Location: index.php');
exit;
