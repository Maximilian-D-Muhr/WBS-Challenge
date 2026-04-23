import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";

import { api, setAuthToken } from "../utils/api";

const TOKEN_KEY = "ct_token";

type Me = {
  id: string;
  email: string;
  displayName: string;
};

type AuthState = {
  user: Me | null;
  token: string | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, displayName: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() => localStorage.getItem(TOKEN_KEY));
  const [user, setUser] = useState<Me | null>(null);
  const [loading, setLoading] = useState<boolean>(true);

  // Keep the api wrapper's in-memory token in sync with React state.
  useEffect(() => {
    setAuthToken(token);
  }, [token]);

  // Fetch the current user whenever we have a token.
  useEffect(() => {
    let cancelled = false;

    async function fetchMe() {
      if (!token) {
        setUser(null);
        setLoading(false);
        return;
      }
      try {
        const me = await api<Me>("/auth/me");
        if (!cancelled) setUser(me);
      } catch {
        if (!cancelled) {
          localStorage.removeItem(TOKEN_KEY);
          setToken(null);
          setUser(null);
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    setLoading(true);
    fetchMe();
    return () => {
      cancelled = true;
    };
  }, [token]);

  const login = useCallback(async (email: string, password: string) => {
    const result = await api<{ token: string; expiresAtUtc: string }>("/auth/login", {
      method: "POST",
      body: { email, password },
    });
    localStorage.setItem(TOKEN_KEY, result.token);
    setToken(result.token);
  }, []);

  const register = useCallback(async (email: string, password: string, displayName: string) => {
    await api<void>("/auth/register", {
      method: "POST",
      body: { email, password, displayName },
    });
    await login(email, password);
  }, [login]);

  const logout = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    setToken(null);
    setUser(null);
  }, []);

  const value = useMemo(
    () => ({ user, token, loading, login, register, logout }),
    [user, token, loading, login, register, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used inside <AuthProvider>");
  return ctx;
}
