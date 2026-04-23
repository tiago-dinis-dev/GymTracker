import { createContext, useContext, useState, type ReactNode } from 'react'

export interface AuthUser {
  userId: string
  name: string
  email: string
}

interface AuthContextType {
  user: AuthUser | null
  token: string | null
  login: (token: string) => void
  logout: () => void
  isAuthenticated: boolean
}

const AuthContext = createContext<AuthContextType | null>(null)

// JWT claims use the long XML schema URIs
function decodeToken(token: string): AuthUser | null {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    return {
      userId:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] ?? '',
      name:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] ?? '',
      email:
        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? '',
    }
  } catch {
    return null
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem('gymtracker_token')
  )
  const [user, setUser] = useState<AuthUser | null>(() => {
    const t = localStorage.getItem('gymtracker_token')
    return t ? decodeToken(t) : null
  })

  function login(newToken: string) {
    localStorage.setItem('gymtracker_token', newToken)
    setToken(newToken)
    setUser(decodeToken(newToken))
  }

  function logout() {
    localStorage.removeItem('gymtracker_token')
    setToken(null)
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, token, login, logout, isAuthenticated: !!token }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used within AuthProvider')
  return ctx
}
