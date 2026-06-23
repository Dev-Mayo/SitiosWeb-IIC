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

$errores = [];
$datos = $grupo;

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $datos['nombre'] = trim($_POST['nombre'] ?? '');

    if ($datos['nombre'] === '') {
        $errores[] = 'El nombre del grupo es obligatorio.';
    }

    if (empty($errores)) {
        Grupo::editar($id, $datos['nombre']);
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
  <title>Editar Grupo</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:540px">
  <h2 class="mb-4">Editar Grupo</h2>

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
      <button type="submit" class="btn btn-primary">Actualizar</button>
      <a href="index.php" class="btn btn-secondary">Cancelar</a>
    </div>
  </form>
</div>
</body>
</html>