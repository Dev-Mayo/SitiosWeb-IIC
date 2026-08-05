import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// La app consume los microservicios a través del API Gateway (YARP) en
// http://localhost:5080 (ver src/config.js). El gateway ya expone CORS para
// el origen del dev server, por lo que no se necesita proxy de reenvío
// (antes se reenviaba /servicios al WCF en http://localhost:63602).
export default defineConfig({
  plugins: [react()]
});
