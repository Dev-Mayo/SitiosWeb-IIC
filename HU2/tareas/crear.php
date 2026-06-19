<?php
session_start();
require_once __DIR__ . '/../clases/Tarea.php';

$errores = [];
$datos = [
    'detalle' => '',
    'prioridad' => 'media',
    'fecha_limite' => '',
    'responsable_id' => ''
];

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $datos['detalle'] = trim($_POST['detalle'] ?? '');
    $datos['prioridad'] = $_POST['prioridad'] ?? 'media';
    $datos['fecha_limite'] = trim($_POST['fecha_limite'] ?? '');
    $datos['responsable_id'] = $_POST['responsable_id'] !== '' ? (int)$_POST['responsable_id'] : null;

    if ($datos['detalle'] === '') {
        $errores[] = 'El detalle de la tarea es obligatorio.';
    }

    if (!in_array($datos['prioridad'], ['baja', 'media', 'alta'], true)) {
        $datos['prioridad'] = 'media';
    }

    if (empty($errores)) {
        try {
            Tarea::crear(
                $datos['detalle'],
                $datos['prioridad'],
                $datos['fecha_limite'] !== '' ? $datos['fecha_limite'] : null,
                $datos['responsable_id']
            );
            $_SESSION['mensaje'] = 'Tarea creada correctamente.';
            header('Location: index.php');
            exit;
        } catch (PDOException $e) {
            $errores[] = 'Error al guardar: ' . $e->getMessage();
        }
    }
}

$responsables = Tarea::listarResponsables();
?>
<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Crear Tarea</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
</head>
<body class="bg-light">
<div class="container py-4" style="max-width:640px">
  <h2 class="mb-4">Nueva Tarea</h2>

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
      <label class="form-label">Detalle <span class="text-danger">*</span></label>
      <textarea name="detalle" class="form-control" rows="4" required><?= htmlspecialchars($datos['detalle']) ?></textarea>
    </div>
    <div class="mb-3">
      <label class="form-label">Prioridad</label>
      <select name="prioridad" class="form-select">
        <option value="baja" <?= $datos['prioridad'] === 'baja' ? 'selected' : '' ?>>Baja</option>
        <option value="media" <?= $datos['prioridad'] === 'media' ? 'selected' : '' ?>>Media</option>
        <option value="alta" <?= $datos['prioridad'] === 'alta' ? 'selected' : '' ?>>Alta</option>
      </select>
    </div>
    <div class="mb-3">
      <label class="form-label">Fecha límite</label>
      <input type="date" name="fecha_limite" class="form-control" value="<?= htmlspecialchars($datos['fecha_limite']) ?>">
    </div>
    <div class="mb-3">
      <label class="form-label">Responsable</label>
      <select name="responsable_id" class="form-select">
        <option value="">Sin responsable asignado</option>
        <?php foreach ($responsables as $r): ?>
          <option value="<?= $r['id'] ?>" <?= (string)$datos['responsable_id'] === (string)$r['id'] ? 'selected' : '' ?>>
            <?= htmlspecialchars($r['nombre'] . ' ' . $r['apellidos']) ?>
          </option>
        <?php endforeach; ?>
      </select>
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
