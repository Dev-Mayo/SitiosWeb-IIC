<?php

class OferenteET
{
    public int $puestoId = 0;
    public int $codigoConcurso = 0;
    public string $puestoNombre = 'Puesto no seleccionado';
    public string $identificacion = '';
    public string $tipoIdentificacion = '';
    public string $nombreCompleto = '';
    public string $fechaNacimiento = '';
    public string $correo = '';
    public string $telefono = '';

    public function cargarDesdePost(array $post): void
    {
        $this->puestoId = isset($post['puesto_id']) ? absint($post['puesto_id']) : 0;
        $this->identificacion = isset($post['identificacion']) ? sanitize_text_field(wp_unslash($post['identificacion'])) : '';
        $this->tipoIdentificacion = isset($post['tipo_identificacion']) ? sanitize_text_field(wp_unslash($post['tipo_identificacion'])) : '';
        $this->nombreCompleto = isset($post['nombre_completo']) ? sanitize_text_field(wp_unslash($post['nombre_completo'])) : '';
        $this->fechaNacimiento = isset($post['fecha_nacimiento']) ? sanitize_text_field(wp_unslash($post['fecha_nacimiento'])) : '';
        $this->correo = isset($post['correo']) ? sanitize_email(wp_unslash($post['correo'])) : '';
        $this->telefono = isset($post['telefono']) ? sanitize_text_field(wp_unslash($post['telefono'])) : '';
    }

    public function limpiarFormulario(): void
    {
        $this->identificacion = '';
        $this->tipoIdentificacion = '';
        $this->nombreCompleto = '';
        $this->fechaNacimiento = '';
        $this->correo = '';
        $this->telefono = '';
    }
}
