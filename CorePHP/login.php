<?php
session_start();

$wcf_url = "http://localhost:63602/AutenticacionService.svc/autenticar";

$error    = "";
$bloqueado = false;

if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $usuario  = trim($_POST['usuario'] ?? '');
    $password = trim($_POST['password'] ?? '');

    if (empty($usuario) || empty($password)) {
        $error = "Usuario y/o contraseña incorrectos.";
    } else {
        // Llamar al WCF Core4
        $payload = json_encode([
            "Usuario"  => $usuario,
            "Password" => $password
        ]);

        $ch = curl_init($wcf_url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_POST, true);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $payload);
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'Content-Type: application/json',
            'Content-Length: ' . strlen($payload)
        ]);
        curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, false);
        curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, false);

        $response = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        curl_close($ch);

        if ($response === false || $httpCode !== 200) {
            $error = "Error al conectar con el servicio de autenticación.";
        } else {
            $resultado = json_decode($response, true);

            if ($resultado['Success']) {
                $_SESSION['id_usuario']      = $resultado['IdUsuario'];
                $_SESSION['nombre_completo'] = $resultado['NombreCompleto'];
                $_SESSION['usuario']         = $usuario;
                header("Location: bienvenida.php");
                exit;
            } else {
                $error     = $resultado['Mensaje'];
                $bloqueado = strpos($resultado['Mensaje'], 'bloqueado') !== false;
            }
        }
    }
}
?>
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Login — Administración de Personal</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />
    <style>
        body { background: #eef2f7; }
        .card-login {
            max-width: 420px;
            margin: 80px auto;
            border-radius: 14px;
            overflow: hidden;
            box-shadow: 0 6px 28px rgba(0,0,0,.15);
        }
        .card-header-custom {
            background: #1aad94;
            padding: 36px 24px;
            text-align: center;
            color: white;
        }
        .logo-emoji { font-size: 64px; text-align: center; }
    </style>
</head>
<body>

<div class="card-login bg-white">

    <div class="card-header-custom">
        <div class="logo-emoji">🏢</div>
        <h5 class="mt-2 mb-0 fw-bold">Recursos Humano</h5>
        <small class="opacity-75">Portal Administrativo</small>
    </div>

    <div class="p-4">

        <?php if (!empty($error)): ?>
            <div class="alert alert-danger"><?php echo htmlspecialchars($error); ?></div>
        <?php endif; ?>

        <?php if ($bloqueado): ?>
            <div class="alert alert-danger">
                <strong>Usuario bloqueado.</strong>
                Se superaron 3 intentos fallidos.
            </div>
        <?php endif; ?>

        <?php if (isset($_GET['msg']) && $_GET['msg'] === 'login'): ?>
            <div class="alert alert-warning">
                Por favor inicie sesión para utilizar el sistema.
            </div>
        <?php endif; ?>

        <form method="POST" action="">

            <div class="mb-3">
                <label class="form-label fw-semibold">ID de usuario</label>
                <input type="text" name="usuario" class="form-control"
                       placeholder="Ingrese su ID de usuario"
                       value="<?php echo htmlspecialchars($_POST['usuario'] ?? ''); ?>"
                       required />
            </div>

            <div class="mb-4">
                <label class="form-label fw-semibold">Contraseña</label>
                <input type="password" name="password" class="form-control"
                       placeholder="Ingrese su contraseña" required />
            </div>

            <button type="submit" class="btn btn-primary w-100 py-2 fw-semibold"
                    style="background:#1aad94; border-color:#1aad94">
                Ingresar
            </button>

        </form>
    </div>
</div>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>
</body>
</html>