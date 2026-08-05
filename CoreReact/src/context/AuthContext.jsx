import { createContext, useContext, useState, useCallback } from 'react';
import { USER_KEY, TOKEN_KEY } from '../config';

const AuthContext = createContext(null);

// Sesión del SPA: perfil del usuario + token JWT, persistidos en localStorage.
// El token lo adjunta apiClient como "Authorization: Bearer <token>" en cada
// llamada protegida al gateway.
export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    try {
      return JSON.parse(localStorage.getItem(USER_KEY) ?? 'null');
    } catch {
      return null;
    }
  });

  const [token, setToken] = useState(() => {
    try {
      return localStorage.getItem(TOKEN_KEY) ?? '';
    } catch {
      return '';
    }
  });

  const login = useCallback((datosUsuario) => {
    const { token: nuevoToken, ...perfil } = datosUsuario;
    setUser(perfil);
    setToken(nuevoToken ?? '');
    try {
      localStorage.setItem(USER_KEY, JSON.stringify(perfil));
      if (nuevoToken) {
        localStorage.setItem(TOKEN_KEY, nuevoToken);
      } else {
        localStorage.removeItem(TOKEN_KEY);
      }
    } catch {
      // almacenamiento no disponible
    }
  }, []);

  const logout = useCallback(() => {
    setUser(null);
    setToken('');
    try {
      localStorage.removeItem(USER_KEY);
      localStorage.removeItem(TOKEN_KEY);
    } catch {
      // almacenamiento no disponible
    }
  }, []);

  return (
    <AuthContext.Provider value={{ user, token, login, logout }}>
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
