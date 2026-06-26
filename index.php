<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Control de Tareas – CUC</title>
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css">
  <style>
    body { background-color: #f8f9fa; }
    .menu-card {
      transition: transform .15s, box-shadow .15s;
      border: none;
      border-radius: 12px;
    }
    .menu-card:hover {
      transform: translateY(-4px);
      box-shadow: 0 8px 24px rgba(0,0,0,.12);
    }
    .icon-circle {
      width: 64px;
      height: 64px;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 28px;
      margin: 0 auto 1rem;
    }
  </style>
</head>
<body>

<div class="container py-5">

  <!-- Encabezado -->
  <div class="text-center mb-5">
    <h1 class="fw-bold">Control de Tareas</h1>
    <p class="text-muted">Colegio Universitario de Cartago – Administración y programación de sitios Web</p>
    <hr class="w-25 mx-auto">
  </div>

  <!-- Tarjetas de menú -->
  <div class="row g-4 justify-content-center">

    <!-- HU1 Responsables -->
    <div class="col-sm-6 col-lg-3">
      <a href="HU1/responsables/index.php" class="text-decoration-none">
        <div class="card menu-card shadow-sm h-100 text-center p-4">
          <div class="icon-circle bg-primary bg-opacity-10 text-primary">👤</div>
          <h5 class="fw-semibold mb-1">Responsables</h5>
          <p class="text-muted small mb-0">Crear, editar y eliminar responsables para asignarles tareas</p>
        </div>
      </a>
    </div>

    <!-- HU2 Tareas -->
    <div class="col-sm-6 col-lg-3">
      <a href="HU2/tareas/index.php" class="text-decoration-none">
        <div class="card menu-card shadow-sm h-100 text-center p-4">
          <div class="icon-circle bg-success bg-opacity-10 text-success">✅</div>
          <h5 class="fw-semibold mb-1">Tareas</h5>
          <p class="text-muted small mb-0">Crear, editar, eliminar y gestionar el estado de las tareas</p>
        </div>
      </a>
    </div>

    <!-- HU3 Grupos -->
    <div class="col-sm-6 col-lg-3">
      <a href="HU3/grupos/index.php" class="text-decoration-none">
        <div class="card menu-card shadow-sm h-100 text-center p-4">
          <div class="icon-circle bg-warning bg-opacity-10 text-warning">📁</div>
          <h5 class="fw-semibold mb-1">Grupos</h5>
          <p class="text-muted small mb-0">Agrupar tareas y ver las que pertenecen a cada grupo</p>
        </div>
      </a>
    </div>

    <!-- HU4 Tablero -->
    <div class="col-sm-6 col-lg-3">
      <a href="HU4/index.php" class="text-decoration-none">
        <div class="card menu-card shadow-sm h-100 text-center p-4">
          <div class="icon-circle bg-danger bg-opacity-10 text-danger">📋</div>
          <h5 class="fw-semibold mb-1">Tablero</h5>
          <p class="text-muted small mb-0">Visualizar tareas en formato Kanban por estado</p>
        </div>
      </a>
    </div>

  </div>

</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>