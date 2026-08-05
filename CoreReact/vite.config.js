import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// En desarrollo, las llamadas a /servicios se reenvían al WCF local
// (http://localhost:63602). Esto evita los problemas de CORS del navegador,
// igual que hacía PHP con cURL en el servidor.
//
// Cuando se migre a APIs/microservicios, solo hay que cambiar el target
// (o crear un proxy por dominio/endpoint) sin tocar las vistas.
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/servicios': {
        target: 'http://localhost:63602',
        changeOrigin: true,
        secure: false,
        // Los .svc del WCF cuelgan de la raíz de la aplicación
        // (ej. /AutenticacionService.svc/autenticar), por lo que el prefijo
        // "/servicios" usado por la app React debe quitarse al reenviar.
        rewrite: (path) => path.replace(/^\/servicios/, '')
      }
    }
  }
});
