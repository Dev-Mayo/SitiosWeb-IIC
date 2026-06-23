<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$id = (int)($_GET['id'] ?? 0);
$grupo = Grupo::obtener($id);

if (!$grupo) {
    $_SESSION['mensaje'] = 'Grupo no encontrado.';
    header('Location: index.php');
    exit;
}

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $nombre = trim($_POST['nombre'] ?? '');

    if ($nombre !== '') {
        Grupo::editar($id, $nombre);
        $_SESSION['mensaje'] = 'Grupo actualizado correctamente.';
        header('Location: index.php');
        exit;
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Editar grupo</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:540px">

  <h2>Editar grupo</h2>

  <form method="post">
    <div class="mb-3">
      <label class="form-label">Nombre</label>
      <input type="text"
             name="nombre"
             class="form-control"
             value="<?= htmlspecialchars($grupo['nombre']) ?>"
             required>
    </div>

    <button type="submit" class="btn btn-primary">Actualizar</button>
    <a href="index.php" class="btn btn-secondary">Cancelar</a>
  </form>

</div>
</body>
</html>