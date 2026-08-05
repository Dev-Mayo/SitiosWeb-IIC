import { createContext, useContext, useState, useCallback } from 'react';

const STORAGE_KEY = 'corereact_usuario';

const AuthContext = createContext(null);

// Sustituye a la sesión PHP. En un SPA el estado se guarda en el navegador;
// cuando se migre a microservicios basta con intercambiar el origen de los
// datos (cookie, token JWT, etc.) en este mismo contexto.
export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    try {
      return JSON.parse(localStorage.getItem(STORAGE_KEY) ?? 'null');
    } catch {
      return null;
    }
  });

  const login = useCallback((datosUsuario) => {
    setUser(datosUsuario);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(datosUsuario));
  }, []);

  const logout = useCallback(() => {
    setUser(null);
    localStorage.removeItem(STORAGE_KEY);
  }, []);

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const contexto = useContext(AuthContext);
  if (!contexto) {
    throw new Error('useAuth debe usarse dentro de <AuthProvider>.');
  }
  return contexto;
}
