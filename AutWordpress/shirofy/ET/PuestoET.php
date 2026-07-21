<?php

class PuestoET
{
    public int $id = 0;
    public string $nombre = '';
    public string $salario = '';
    public string $jefe = 'Sin asignar';

    public function cargarDesdeFila(array $fila): void
    {
        $this->id = isset($fila['puesto_id']) ? (int) $fila['puesto_id'] : 0;
        $this->nombre = $fila['nombre'] ?? '';
        $this->salario = $fila['salario'] ?? '';
        $this->jefe = $fila['nombre_jefe'] ?? 'Sin asignar';
    }
}