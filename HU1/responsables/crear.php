<?php
// responsables/crear.php
session_start();
require_once __DIR__ . '/../clases/Responsable.php';

$errores = [];
$datos   = ['nombre' => '', 'apellidos' => '', 'identificacion' => ''];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $datos['nombre']         = trim($_POST['nombre']         ?? '');
    $datos['apellidos']      = trim($_POST['apellidos']      ?? '');
    $datos['identificacion'] = trim($_POST['identificacion'] ?? '');

    if ($datos['nombre']         === '') $errores[] = 'El nombre es obligatorio.';
    if ($datos['apellidos']      === '') $errores[] = 'Los apellidos son obligatorios.';
    if ($datos['identificacion'] === '') $errores[] = 'La identificación es obligatoria.';

    if (empty($errores)) {
        try {
            Responsable::crear($datos['nombre'], $datos['apellidos'], $datos['identificacion']);
            $_SESSION['mensaje'] = 'Responsable creado correctamente.';
            header('Location: index.php');
            exit;
        } catch (PDOException $e) {
            if ($e->getCode() === '23000') {
                $errores[] = 'Ya existe un responsable con esa identificación.';
            } else {
                $errores[] = 'Error al guardar: ' . $e->getMessage();
            }
        }
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Crear Responsable</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:540px">
  <h2 class="mb-4">Nuevo Responsable</h2>

  <?php if ($errores): ?>
    <div class="alert alert-danger">
      <ul class="mb-0">
        <?php foreach ($errores as $e): ?>
          <li><?= htmlspecialchars($e) ?></li>
        <?php endforeach; ?>
      </ul>
    </div>
  <?php endif; ?>

  <form method="post" novalidate>
    <div class="mb-3">
      <label class="form-label">Nombre <span class="text-danger">*</span></label>
      <input type="text" name="nombre" class="form-control"
             value="<?= htmlspecialchars($datos['nombre']) ?>" required>
    </div>
    <div class="mb-3">
      <label class="form-label">Apellidos <span class="text-danger">*</span></label>
      <input type="text" name="apellidos" class="form-control"
             value="<?= htmlspecialchars($datos['apellidos']) ?>" required>
    </div>
    <div class="mb-3">
      <label class="form-label">Identificación <span class="text-danger">*</span></label>
      <input type="text" name="identificacion" class="form-control"
             value="<?= htmlspecialchars($datos['identificacion']) ?>" required>
    </div>
    <div class="d-flex gap-2">
      <button type="submit" class="btn btn-primary">Guardar</button>
      <a href="index.php" class="btn btn-secondary">Cancelar</a>
    </div>
  </form>
</div>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>
