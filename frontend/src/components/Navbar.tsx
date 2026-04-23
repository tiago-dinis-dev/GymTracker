import { Link, useLocation, useNavigate } from 'react-router-dom'
import { Dumbbell, LogOut, LogIn } from 'lucide-react'
import { useAuth } from '../context/AuthContext'

const navLinks = [
  { label: 'Dashboard', to: '/dashboard' },
  { label: 'Workouts', to: '/workouts' },
  { label: 'Log Workout', to: '/workout/new' },
  { label: 'Exercises', to: '/exercises' },
]

export default function Navbar() {
  const location = useLocation()
  const navigate = useNavigate()
  const { isAuthenticated, logout, user } = useAuth()

  function handleLogout() {
    logout()
    navigate('/')
  }

  return (
    <nav className="fixed top-4 left-4 right-4 z-50 mx-auto max-w-6xl">
      <div className="bg-gray-800/90 backdrop-blur-md rounded-2xl px-6 py-4 flex items-center justify-between shadow-xl border border-gray-700/50">
        <Link to="/" className="flex items-center gap-2 cursor-pointer group">
          <Dumbbell className="w-7 h-7 text-primary transition-colors duration-200 group-hover:text-secondary" />
          <span className="font-heading text-2xl font-bold tracking-wide">
            <span className="text-primary">Gym</span>
            <span className="text-foreground">Tracker</span>
          </span>
        </Link>

        <div className="hidden md:flex items-center gap-1">
          {isAuthenticated && navLinks.map((link) => {
            const isActive = location.pathname === link.to
            return (
              <Link
                key={link.to}
                to={link.to}
                className={`px-4 py-2 rounded-xl font-body font-semibold text-sm transition-colors duration-200 cursor-pointer ${
                  isActive
                    ? 'bg-primary text-white'
                    : 'text-muted hover:text-foreground hover:bg-gray-700'
                }`}
              >
                {link.label}
              </Link>
            )
          })}
        </div>

        <div className="flex items-center gap-2">
          {isAuthenticated ? (
            <>
              {user?.name && (
                <span className="hidden md:block text-muted font-body text-sm mr-2">
                  {user.name.split(' ')[0]}
                </span>
              )}
              <button
                onClick={handleLogout}
                className="flex items-center gap-1.5 px-4 py-2 rounded-xl font-body font-semibold text-sm text-muted hover:text-foreground hover:bg-gray-700 transition-colors duration-200 cursor-pointer"
              >
                <LogOut className="w-4 h-4" /> Logout
              </button>
            </>
          ) : (
            <Link
              to="/login"
              className="flex items-center gap-1.5 px-4 py-2 rounded-xl font-body font-semibold text-sm bg-primary text-white hover:bg-secondary transition-colors duration-200 cursor-pointer"
            >
              <LogIn className="w-4 h-4" /> Login
            </Link>
          )}
        </div>

        {/* Mobile nav links */}
        {isAuthenticated && (
          <div className="md:hidden flex items-center gap-2">
            {navLinks.map((link) => {
              const isActive = location.pathname === link.to
              return (
                <Link
                  key={link.to}
                  to={link.to}
                  className={`px-3 py-1.5 rounded-lg font-body font-semibold text-xs transition-colors duration-200 cursor-pointer ${
                    isActive
                      ? 'bg-primary text-white'
                      : 'text-muted hover:text-foreground hover:bg-gray-700'
                  }`}
                >
                  {link.label}
                </Link>
              )
            })}
          </div>
        )}
      </div>
    </nav>
  )
}
