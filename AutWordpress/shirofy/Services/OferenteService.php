<?php

require_once __DIR__ . '/../ET/OferenteET.php';
require_once __DIR__ . '/../Repositories/OferenteRepository.php';

class OferenteService
{
    private OferenteRepository $repository;

    public function __construct(OferenteRepository $repository)
    {
        $this->repository = $repository;
    }

    public function cargarPuesto(OferenteET $oferente): string
    {
        if ($oferente->puestoId <= 0) {
            return '';
        }

        $fila = $this->repository->obtenerPuestoConcursoVigente($oferente->puestoId);

        if (!$fila) {
            $oferente->puestoId = 0;
            $oferente->puestoNombre = 'Puesto no disponible';
            return 'El puesto no existe, no está disponible o no tiene un concurso vigente.';
        }

        $oferente->puestoNombre = $fila['puesto_nombre'];
        $oferente->codigoConcurso = (int) $fila['codigo_concurso'];

        return '';
    }

    public function procesarRegistro(OferenteET $oferente, array $files): array
    {
        $errorPuesto = $this->cargarPuesto($oferente);

        if ($errorPuesto !== '') {
            return array(
                'exito' => false,
                'mensaje' => 'El puesto seleccionado no tiene un concurso vigente.'
            );
        }

        $mensajeValidacion = $this->validar($oferente, $files);

        if ($mensajeValidacion !== '') {
            return array('exito' => false, 'mensaje' => $mensajeValidacion);
        }

        if ($this->repository->existePostulacion($oferente->identificacion, $oferente->codigoConcurso)) {
            return array(
                'exito' => false,
                'mensaje' => 'El oferente ya está registrado en este concurso.'
            );
        }

        $resultadoArchivo = $this->subirCurriculum($files['curriculum']);

        if (!$resultadoArchivo['exito']) {
            return $resultadoArchivo;
        }

        try {
            $this->repository->guardarPostulacion($oferente, $resultadoArchivo['url']);
            $oferente->limpiarFormulario();

            return array(
                'exito' => true,
                'mensaje' => 'Datos guardados de manera satisfactoria.'
            );
        } catch (Throwable $error) {
            if (!empty($resultadoArchivo['archivo']) && file_exists($resultadoArchivo['archivo'])) {
                unlink($resultadoArchivo['archivo']);
            }

            return array(
                'exito' => false,
                'mensaje' => 'No fue posible guardar los datos: ' . $error->getMessage()
            );
        }
    }

    private function validar(OferenteET $oferente, array $files): string
    {
        $tiposPermitidos = array('Cedula', 'DIMEX', 'Pasaporte');

        if (
            empty($oferente->identificacion)
            || empty($oferente->tipoIdentificacion)
            || empty($oferente->nombreCompleto)
            || empty($oferente->fechaNacimiento)
            || empty($oferente->correo)
            || empty($oferente->telefono)
        ) {
            return 'Debe completar todos los campos requeridos.';
        }

        if ($oferente->puestoId <= 0 || $oferente->codigoConcurso <= 0) {
            return 'El puesto seleccionado no tiene un concurso vigente.';
        }

        if (!in_array($oferente->tipoIdentificacion, $tiposPermitidos, true)) {
            return 'El tipo de identificación seleccionado no es válido.';
        }

        if (!is_email($oferente->correo)) {
            return 'El correo electrónico no tiene un formato válido.';
        }

        if (!preg_match('/^[0-9+\-\s]{8,20}$/', $oferente->telefono)) {
            return 'El teléfono debe contener entre 8 y 20 caracteres válidos.';
        }

        if (!DateTime::createFromFormat('Y-m-d', $oferente->fechaNacimiento)) {
            return 'La fecha de nacimiento no es válida.';
        }

        if (!isset($files['curriculum']) || $files['curriculum']['error'] !== UPLOAD_ERR_OK) {
            return 'Debe seleccionar un currículum válido.';
        }

        return '';
    }

    private function subirCurriculum(array $archivo): array
    {
        $nombreArchivo = sanitize_file_name($archivo['name']);
        $extension = strtolower(pathinfo($nombreArchivo, PATHINFO_EXTENSION));
        $extensionesPermitidas = array('pdf', 'doc', 'docx');
        $tamanoMaximo = 5 * 1024 * 1024;

        if (!in_array($extension, $extensionesPermitidas, true)) {
            return array(
                'exito' => false,
                'mensaje' => 'El currículum debe ser PDF, DOC o DOCX.'
            );
        }

        if ((int) $archivo['size'] > $tamanoMaximo) {
            return array(
                'exito' => false,
                'mensaje' => 'El currículum no puede superar los 5 MB.'
            );
        }

        require_once ABSPATH . 'wp-admin/includes/file.php';

        $subida = wp_handle_upload(
            $archivo,
            array(
                'test_form' => false,
                'mimes' => array(
                    'pdf' => 'application/pdf',
                    'doc' => 'application/msword',
                    'docx' => 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                )
            )
        );

        if (isset($subida['error']) || empty($subida['url'])) {
            return array(
                'exito' => false,
                'mensaje' => isset($subida['error'])
                    ? $subida['error']
                    : 'No fue posible subir el currículum.'
            );
        }

        return array(
            'exito' => true,
            'mensaje' => '',
            'url' => esc_url_raw($subida['url']),
            'archivo' => $subida['file'] ?? ''
        );
    }
}
