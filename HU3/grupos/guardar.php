<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$id = $_POST["id"] ?? null;
$nombre = trim($_POST["nombre"] ?? "");

if ($nombre === "") {
    $_SESSION["mensaje"] = "El nombre del grupo es obligatorio.";
    header("Location: index.php");
    exit;
}

if ($id) {
    Grupo::editar((int)$id, $nombre);
    $_SESSION["mensaje"] = "Grupo actualizado correctamente.";
} else {
    Grupo::crear($nombre);
    $_SESSION["mensaje"] = "Grupo creado correctamente.";
}

header("Location: index.php");
exit;