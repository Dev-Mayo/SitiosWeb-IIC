<?php

require_once get_template_directory() . '/ET/OferenteET.php';
require_once get_template_directory() . '/Repositories/OferenteRepository.php';
require_once get_template_directory() . '/Services/OferenteService.php';

$mensajeExito = '';
$mensajeError = '';
$oferente = new OferenteET();
$oferente->puestoId = isset($_GET['id']) ? absint($_GET['id']) : 0;

try {
    $repository = new OferenteRepository();
    $service = new OferenteService($repository);
} catch (Throwable $error) {
    wp_die($error->getMessage());
}

$mensajeError = $service->cargarPuesto($oferente);

if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_POST['guardar_oferente'])) {
    $nonceValido = isset($_POST['aut3_nonce'])
        && wp_verify_nonce(
            sanitize_text_field(wp_unslash($_POST['aut3_nonce'])),
            'guardar_oferente_aut3'
        );

    if (!$nonceValido) {
        $mensajeError = 'La solicitud no es válida. Actualice la página e inténtelo nuevamente.';
    } else {
        $oferente->cargarDesdePost($_POST);
        $resultado = $service->procesarRegistro($oferente, $_FILES);

        if ($resultado['exito']) {
            $mensajeExito = $resultado['mensaje'];
            $mensajeError = '';
        } else {
            $mensajeError = $resultado['mensaje'];
        }
    }
}

get_header();
?>

<!-- HERO -->
<section class="hero">
    <div class="container">
        <h1>Registro de Oferente</h1>

        <p>
            Complete sus datos para participar por el puesto seleccionado.
        </p>
    </div>
</section>

<!-- FORMULARIO AUT3 -->
<section class="section">
    <div class="container">

        <div class="row justify-content-center">
            <div class="col-lg-9">

                <div class="card-shirofy">
                    <div class="card-body p-4 p-md-5">

                        <div class="mb-4">

                            <p class="text-uppercase fw-semibold mb-1"
                               style="color:var(--primary);
                                      font-size:0.85rem;
                                      letter-spacing:1px">

                                Puesto seleccionado
                            </p>

                            <h2 class="section-title mb-2">
                                <?php
                                echo esc_html($oferente->puestoNombre);
                                ?>
                            </h2>

                            <div class="divider"></div>
                        </div>


                        <?php if (!empty($mensajeError)): ?>

                            <div class="alert alert-danger">
                                <?php
                                echo esc_html($mensajeError);
                                ?>
                            </div>

                        <?php endif; ?>


                        <?php if (!empty($mensajeExito)): ?>

                            <div class="alert alert-success">

                                <p class="mb-3">
                                    <?php
                                    echo esc_html($mensajeExito);
                                    ?>
                                </p>

                                <a href="<?php
                                    echo esc_url(
                                        home_url('/puestos/')
                                    );
                                ?>"
                                   class="btn btn-success">

                                    Aceptar
                                </a>

                            </div>

                        <?php else: ?>

                            <?php if ($oferente->puestoId <= 0): ?>

                                <div class="alert alert-warning">
                                    No se recibió un puesto válido.
                                    Regrese al listado y seleccione uno.
                                </div>

                            <?php endif; ?>


                            <form method="post"
                                  enctype="multipart/form-data"
                                  id="formRegistroOferente">

                                <?php
                                wp_nonce_field(
                                    'guardar_oferente_aut3',
                                    'aut3_nonce'
                                );
                                ?>

                                <input type="hidden"
                                       name="puesto_id"
                                       value="<?php
                                            echo esc_attr(
                                                $oferente->puestoId
                                            );
                                       ?>">


                                <div class="row g-3">

                                    <div class="col-md-6">

                                        <label for="identificacion"
                                               class="form-label">

                                            Identificación
                                        </label>

                                        <input type="text"
                                               id="identificacion"
                                               name="identificacion"
                                               class="form-control"
                                               maxlength="20"
                                               value="<?php
                                                    echo esc_attr(
                                                        $oferente->identificacion
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="tipo_identificacion"
                                               class="form-label">

                                            Tipo de identificación
                                        </label>

                                        <select id="tipo_identificacion"
                                                name="tipo_identificacion"
                                                class="form-select"
                                                required>

                                            <option value="">
                                                Seleccione
                                            </option>

                                            <option value="Cedula"
                                                <?php
                                                selected(
                                                    $oferente->tipoIdentificacion,
                                                    'Cedula'
                                                );
                                                ?>>

                                                Cédula de identidad
                                            </option>

                                            <option value="DIMEX"
                                                <?php
                                                selected(
                                                    $oferente->tipoIdentificacion,
                                                    'DIMEX'
                                                );
                                                ?>>

                                                DIMEX
                                            </option>

                                            <option value="Pasaporte"
                                                <?php
                                                selected(
                                                    $oferente->tipoIdentificacion,
                                                    'Pasaporte'
                                                );
                                                ?>>

                                                Pasaporte
                                            </option>
                                        </select>
                                    </div>


                                    <div class="col-12">

                                        <label for="nombre_completo"
                                               class="form-label">

                                            Nombre completo
                                        </label>

                                        <input type="text"
                                               id="nombre_completo"
                                               name="nombre_completo"
                                               class="form-control"
                                               maxlength="100"
                                               value="<?php
                                                    echo esc_attr(
                                                        $oferente->nombreCompleto
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="fecha_nacimiento"
                                               class="form-label">

                                            Fecha de nacimiento
                                        </label>

                                        <input type="date"
                                               id="fecha_nacimiento"
                                               name="fecha_nacimiento"
                                               class="form-control"
                                               value="<?php
                                                    echo esc_attr(
                                                        $oferente->fechaNacimiento
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="correo"
                                               class="form-label">

                                            Correo electrónico
                                        </label>

                                        <input type="email"
                                               id="correo"
                                               name="correo"
                                               class="form-control"
                                               maxlength="100"
                                               value="<?php
                                                    echo esc_attr(
                                                        $oferente->correo
                                                    );
                                               ?>"
                                               required>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="telefono"
                                               class="form-label">

                                            Teléfono
                                        </label>

                                        <input type="tel"
                                               id="telefono"
                                               name="telefono"
                                               class="form-control"
                                               maxlength="20"
                                               pattern="[0-9+\-\s]{8,20}"
                                               value="<?php
                                                    echo esc_attr(
                                                        $oferente->telefono
                                                    );
                                               ?>"
                                               required>

                                        <small class="text-muted">
                                            Debe indicar al menos un teléfono.
                                        </small>
                                    </div>


                                    <div class="col-md-6">

                                        <label for="curriculum"
                                               class="form-label">

                                            Currículum
                                        </label>

                                        <input type="file"
                                               id="curriculum"
                                               name="curriculum"
                                               class="form-control"
                                               accept=".pdf,.doc,.docx"
                                               required>

                                        <small class="text-muted">
                                            Formatos permitidos:
                                            PDF, DOC y DOCX.
                                            Máximo 5 MB.
                                        </small>
                                    </div>

                                </div>


                                <div class="d-flex flex-column
                                            flex-sm-row
                                            justify-content-end
                                            gap-2 mt-4">

                                    <a href="<?php
                                        echo esc_url(
                                            home_url('/puestos/')
                                        );
                                    ?>"
                                       class="btn btn-secondary">

                                        Cancelar
                                    </a>

                                    <button type="submit"
                                            name="guardar_oferente"
                                            class="btn text-white"
                                            style="background:var(--primary)"
                                            <?php
                                            echo $oferente->puestoId <= 0
                                                ? 'disabled'
                                                : '';
                                            ?>>

                                        Aceptar
                                    </button>

                                </div>

                            </form>

                        <?php endif; ?>

                    </div>
                </div>

            </div>
        </div>

    </div>
</section>

<?php
$repository->cerrar();
get_footer();
?>