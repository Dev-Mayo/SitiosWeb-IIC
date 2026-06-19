<?php
require_once __DIR__ . '/../HU1/config/db.php';

if (!isset($_GET['id']) || !isset($_GET['estado'])) {
    header("Location: index.php?error=Datos incompletos");
    exit;
}

$id = intval($_GET['id']);
$nuevoEstado = $_GET['estado'];

try {
    $pdo = getConexion();

    $stmt = $pdo->prepare("CALL cambiar_estado(:id, :estado)");
    $stmt->execute([
        ':id' => $id,
        ':estado' => $nuevoEstado
    ]);

    header("Location: index.php");
    exit;

} catch (PDOException $e) {
    header("Location: index.php?error=" . urlencode($e->getMessage()));
    exit;
}