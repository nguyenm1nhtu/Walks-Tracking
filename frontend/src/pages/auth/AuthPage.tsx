import { isAxiosError } from 'axios'
import { Eye, EyeOff } from 'lucide-react'
import { useState } from 'react'
import type { FormEvent } from 'react'
import MessageBanner from '../../components/common/MessageBanner'
import Modal from '../../components/common/Modal'
import { clearAccessToken, getAccessToken } from '../../lib/apiClient'
import { login, logout, register } from '../../services/authService'
import type { LoginRequestDto, RegisterRequestDto } from '../../types/auth'

interface AuthPageProps {
  onAuthChanged?: () => void
}

type AuthMode = 'login' | 'register'

const availableRoles = ['Reader', 'Writer'] as const

type AvailableRole = (typeof availableRoles)[number]

const emptyRegisterForm: RegisterRequestDto = {
  username: '',
  password: '',
  roles: [],
}

const emptyLoginForm: LoginRequestDto = {
  username: '',
  password: '',
}

function AuthPage({ onAuthChanged }: AuthPageProps) {
  const [registerForm, setRegisterForm] = useState<RegisterRequestDto>(emptyRegisterForm)
  const [loginForm, setLoginForm] = useState<LoginRequestDto>(emptyLoginForm)
  const [selectedRole, setSelectedRole] = useState<AvailableRole>('Reader')
  const [isRegistering, setIsRegistering] = useState(false)
  const [isLoggingIn, setIsLoggingIn] = useState(false)
  const [isLoggingOut, setIsLoggingOut] = useState(false)
  const [showRegisterPassword, setShowRegisterPassword] = useState(false)
  const [showLoginPassword, setShowLoginPassword] = useState(false)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [storedToken, setStoredToken] = useState(getAccessToken())
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [authMode, setAuthMode] = useState<AuthMode>('login')

  const isAuthenticated = Boolean(storedToken)

  function openAuthModal(mode: AuthMode) {
    setAuthMode(mode)
    setErrorMessage(null)
    setSuccessMessage(null)
    setIsModalOpen(true)
  }

  function closeAuthModal() {
    setIsModalOpen(false)
    setErrorMessage(null)
  }

  async function handleRegister(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsRegistering(true)
    setErrorMessage(null)
    setSuccessMessage(null)

    try {
      const payload: RegisterRequestDto = {
        username: registerForm.username.trim(),
        password: registerForm.password,
        roles: [selectedRole],
      }

      const responseMessage = await register(payload)
      setSuccessMessage(responseMessage || 'Sign-up successful. Please sign in.')
      setRegisterForm(emptyRegisterForm)
      setSelectedRole('Reader')
      setShowRegisterPassword(false)
      setAuthMode('login')
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    } finally {
      setIsRegistering(false)
    }
  }

  async function handleLogin(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setIsLoggingIn(true)
    setErrorMessage(null)
    setSuccessMessage(null)

    try {
      await login({
        username: loginForm.username.trim(),
        password: loginForm.password,
      })

      const token = getAccessToken()
      setStoredToken(token)
      setSuccessMessage('Signed in successfully. JWT token has been saved automatically.')
      setLoginForm(emptyLoginForm)
      setShowLoginPassword(false)
      setIsModalOpen(false)
      onAuthChanged?.()
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    } finally {
      setIsLoggingIn(false)
    }
  }

  async function handleLogout() {
    setIsLoggingOut(true)
    setErrorMessage(null)
    setSuccessMessage(null)

    try {
      const responseMessage = await logout()
      setSuccessMessage(responseMessage || 'Logged out successfully.')
    } catch (error) {
      setErrorMessage(toUserMessage(error))
    } finally {
      clearAccessToken()
      setStoredToken('')
      setIsModalOpen(false)
      setIsLoggingOut(false)
      onAuthChanged?.()
    }
  }

  return (
    <>
      <div className="auth-toolbar-wrap">
        <div className="auth-toolbar">
          <span className={`status-pill ${isAuthenticated ? 'status-ok' : 'status-idle'}`}>
            {isAuthenticated ? 'Signed in' : 'Guest'}
          </span>
          <div className="auth-actions header-auth-actions">
            <button type="button" onClick={() => openAuthModal('login')}>
              Sign in
            </button>
            <button type="button" className="secondary" onClick={() => openAuthModal('register')}>
              Sign up
            </button>
            <button
              type="button"
              className="danger"
              onClick={() => void handleLogout()}
              disabled={!isAuthenticated || isLoggingOut}
            >
              {isLoggingOut ? 'Logging out...' : 'Logout'}
            </button>
          </div>
        </div>
        <MessageBanner type="error" message={errorMessage} />
        <MessageBanner type="success" message={successMessage} />
      </div>

      <Modal open={isModalOpen} title="Authentication" onClose={closeAuthModal}>
        <div className="auth-modal-content">
          <MessageBanner type="error" message={errorMessage} />
          <MessageBanner type="success" message={successMessage} />

          {authMode === 'register' ? (
            <form onSubmit={handleRegister} className="column auth-form minimalist-form">
              <label className="field-label" htmlFor="register-email">
                Email
              </label>
              <input
                id="register-email"
                className="input"
                type="email"
                placeholder="Enter your email"
                value={registerForm.username}
                onChange={(event) =>
                  setRegisterForm((previous) => ({ ...previous, username: event.target.value }))
                }
                required
              />

              <label className="field-label" htmlFor="register-password">
                Password
              </label>
              <div className="password-field">
                <input
                  id="register-password"
                  className="input password-input"
                  type={showRegisterPassword ? 'text' : 'password'}
                  placeholder="Create a password"
                  value={registerForm.password}
                  onChange={(event) =>
                    setRegisterForm((previous) => ({ ...previous, password: event.target.value }))
                  }
                  minLength={6}
                  required
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowRegisterPassword((previous) => !previous)}
                  aria-label={showRegisterPassword ? 'Hide password' : 'Show password'}
                >
                  {showRegisterPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>

              <div className="column role-group">
                <span className="role-label">Role</span>
                <div className="role-options" role="group" aria-label="Roles">
                  {availableRoles.map((role) => (
                    <label key={role} className="role-option">
                      <input
                        type="radio"
                        name="register-role"
                        checked={selectedRole === role}
                        onChange={() => setSelectedRole(role)}
                      />
                      <span>{role}</span>
                    </label>
                  ))}
                </div>
              </div>

              <div className="auth-submit-row">
                <button type="submit" disabled={isRegistering}>
                  {isRegistering ? 'Signing up...' : 'Sign up'}
                </button>
              </div>

              <button type="button" className="link-button" onClick={() => setAuthMode('login')}>
                Already have an account? Sign in
              </button>
            </form>
          ) : (
            <form onSubmit={handleLogin} className="column auth-form minimalist-form">
              <label className="field-label" htmlFor="login-email">
                Email
              </label>
              <input
                id="login-email"
                className="input"
                type="email"
                placeholder="Enter your email"
                value={loginForm.username}
                onChange={(event) =>
                  setLoginForm((previous) => ({ ...previous, username: event.target.value }))
                }
                required
              />

              <label className="field-label" htmlFor="login-password">
                Password
              </label>
              <div className="password-field">
                <input
                  id="login-password"
                  className="input password-input"
                  type={showLoginPassword ? 'text' : 'password'}
                  placeholder="Enter your password"
                  value={loginForm.password}
                  onChange={(event) =>
                    setLoginForm((previous) => ({ ...previous, password: event.target.value }))
                  }
                  required
                />
                <button
                  type="button"
                  className="password-toggle"
                  onClick={() => setShowLoginPassword((previous) => !previous)}
                  aria-label={showLoginPassword ? 'Hide password' : 'Show password'}
                >
                  {showLoginPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </button>
              </div>

              <div className="auth-submit-row">
                <button type="submit" disabled={isLoggingIn}>
                  {isLoggingIn ? 'Signing in...' : 'Sign in'}
                </button>
              </div>

              <button type="button" className="link-button" onClick={() => setAuthMode('register')}>
                No account yet? Sign up
              </button>
            </form>
          )}
        </div>
      </Modal>
    </>
  )
}

function toUserMessage(error: unknown): string {
  if (isAxiosError(error)) {
    const serverData = error.response?.data

    if (typeof serverData === 'string' && serverData.trim()) {
      return serverData
    }

    if (serverData && typeof serverData === 'object') {
      const typedData = serverData as { detail?: unknown; title?: unknown; errors?: unknown }

      if (typeof typedData.detail === 'string' && typedData.detail.trim()) {
        return typedData.detail
      }

      if (typedData.errors && typeof typedData.errors === 'object') {
        const errorEntries = Object.values(typedData.errors as Record<string, unknown>)
        const firstGroup = errorEntries.find((entry) => Array.isArray(entry))

        if (firstGroup && Array.isArray(firstGroup) && typeof firstGroup[0] === 'string') {
          return firstGroup[0]
        }
      }

      if (typeof typedData.title === 'string' && typedData.title.trim()) {
        return typedData.title
      }
    }

    if (error.message) {
      return error.message
    }
  }

  if (error instanceof Error) {
    return error.message
  }

  return 'An error occurred while calling the API.'
}

export default AuthPage
