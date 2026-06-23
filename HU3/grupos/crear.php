<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$errores = [];
$tareas = Grupo::tareasPendientes();

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $nombre = trim($_POST['nombre'] ?? '');
    $tareasSeleccionadas = $_POST['tareas'] ?? [];

    if ($nombre === '') {
        $errores[] = 'El nombre del grupo es obligatorio.';
    }

    if (empty($errores)) {
        Grupo::crear($nombre, $tareasSeleccionadas);
        $_SESSION['mensaje'] = 'Grupo creado correctamente.';
        header('Location: index.php');
        exit;
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>Crear grupo</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:700px">

  <h2>Nuevo grupo</h2>

  <?php if ($errores): ?>
    <div class="alert alert-danger">
      <?php foreach ($errores as $e): ?>
        <div><?= htmlspecialchars($e) ?></div>
      <?php endforeach; ?>
    </div>
  <?php endif; ?>

  <form method="post">
    <div class="mb-3">
      <label class="form-label">Nombre del grupo</label>
      <input type="text" name="nombre" class="form-control" required>
    </div>

    <div class="mb-3">
      <label class="form-label">Tareas pendientes para asociar</label>

      <?php if (empty($tareas)): ?>
        <div class="alert alert-info">No hay tareas pendientes.</div>
      <?php else: ?>
        <div class="border rounded bg-white p-3">
          <?php foreach ($tareas as $t): ?>
            <div class="form-check mb-2">
              <input type="checkbox"
                     class="form-check-input"
                     name="tareas[]"
                     value="<?= $t['id'] ?>"
                     id="t<?= $t['id'] ?>">

              <label class="form-check-label" for="t<?= $t['id'] ?>">
                <?= htmlspecialchars($t['detalle']) ?>
              </label>
            </div>
          <?php endforeach; ?>
        </div>
      <?php endif; ?>
    </div>

    <button type="submit" class="btn btn-primary">Guardar</button>
    <a href="index.php" class="btn btn-secondary">Cancelar</a>
  </form>

</div>
</body>
</html>