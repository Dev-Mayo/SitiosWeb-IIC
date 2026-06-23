<?php
session_start();
require_once __DIR__ . '/../clases/Grupo.php';

$errores = [];
$datos = ['nombre' => ''];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $datos['nombre'] = trim($_POST['nombre'] ?? '');

    if ($datos['nombre'] === '') {
        $errores[] = 'El nombre del grupo es obligatorio.';
    }

    if (empty($errores)) {
        Grupo::crear($datos['nombre']);
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
  <title>Crear Grupo</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:540px">
  <h2 class="mb-4">Nuevo Grupo</h2>

  <?php if ($errores): ?>
    <div class="alert alert-danger">
      <ul class="mb-0">
        <?php foreach ($errores as $e): ?>
          <li><?= htmlspecialchars($e) ?></li>
        <?php endforeach; ?>
      </ul>
    </div>
  <?php endif; ?>

  <form method="post">
    <div class="mb-3">
      <label class="form-label">Nombre <span class="text-danger">*</span></label>
      <input type="text" name="nombre" class="form-control"
             value="<?= htmlspecialchars($datos['nombre']) ?>" required>
    </div>

    <div class="d-flex gap-2">
      <button type="submit" class="btn btn-primary">Guardar</button>
      <a href="index.php" class="btn btn-secondary">Cancelar</a>
    </div>
  </form>
</div>
</body>
</html>