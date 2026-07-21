<?php

require_once __DIR__ . '/../ET/PuestoET.php';
require_once __DIR__ . '/../Repositories/PuestoRepository.php';

class PuestoService
{
    private PuestoRepository $repository;

    public function __construct(PuestoRepository $repository)
    {
        $this->repository = $repository;
    }

    /**
     * @return array{exito: bool, mensaje: string, puestos: PuestoET[]}
     */
    public function obtenerPuestosDisponibles(): array
    {
        try {
            $puestos = $this->repository->listarDisponibles();

            return array(
                'exito' => true,
                'mensaje' => '',
                'puestos' => $puestos
            );
        } catch (Throwable $error) {
            return array(
                'exito' => false,
                'mensaje' => 'No se pudieron cargar los puestos disponibles en este momento.',
                'puestos' => array()
            );
        }
    }
}