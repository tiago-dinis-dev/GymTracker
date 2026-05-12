import { useState, type FormEvent } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { Dumbbell, Mail, Lock, User, Weight, AlertCircle, Loader2 } from 'lucide-react'
import { api } from '../lib/api'
import { useAuth } from '../context/AuthContext'

export default function Register() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [weight, setWeight] = useState('')
  const [height, setHeight] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const { token } = await api.register(name, email, Number(weight), Number(height))
      login(token)
      navigate('/dashboard')
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : 'Registration failed. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen bg-background flex items-center justify-center px-6 py-12">
      <div className="w-full max-w-md">
        {/* Logo */}
        <Link to="/" className="flex items-center justify-center gap-2 mb-10 group cursor-pointer">
          <Dumbbell className="w-9 h-9 text-primary transition-colors duration-200 group-hover:text-secondary" />
          <span className="font-heading text-4xl font-bold tracking-wide">
            <span className="text-primary">Gym</span>
            <span className="text-foreground">Tracker</span>
          </span>
        </Link>

        <div className="bg-surface rounded-2xl border border-gray-700 p-8">
          <h1 className="font-heading text-3xl font-black text-foreground uppercase mb-2">
            Create Account
          </h1>
          <p className="text-muted font-body text-sm mb-8">Start tracking your gains today.</p>

          {error && (
            <div className="flex items-center gap-3 bg-red-500/10 border border-red-500/30 rounded-xl px-4 py-3 mb-6">
              <AlertCircle className="w-4 h-4 text-red-400 flex-shrink-0" />
              <p className="text-red-400 font-body text-sm">{error}</p>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-muted font-body text-xs font-semibold uppercase tracking-wide mb-2">Name</label>
              <div className="relative">
                <User className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
                <input
                  type="text"
                  required
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  placeholder="Your name"
                  className="w-full bg-gray-900/50 border border-gray-700 rounded-xl pl-11 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
                />
              </div>
            </div>

            <div>
              <label className="block text-muted font-body text-xs font-semibold uppercase tracking-wide mb-2">Email</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
                <input
                  type="email"
                  required
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="you@example.com"
                  className="w-full bg-gray-900/50 border border-gray-700 rounded-xl pl-11 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
                />
              </div>
            </div>

            <div>
              <label className="block text-muted font-body text-xs font-semibold uppercase tracking-wide mb-2">Password</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
                <input
                  type="password"
                  required
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder="••••••••"
                  className="w-full bg-gray-900/50 border border-gray-700 rounded-xl pl-11 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
                />
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-muted font-body text-xs font-semibold uppercase tracking-wide mb-2">Weight (kg)</label>
                <div className="relative">
                  <Weight className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
                  <input
                    type="number"
                    required
                    min={1}
                    step={0.1}
                    value={weight}
                    onChange={(e) => setWeight(e.target.value)}
                    placeholder="70"
                    className="w-full bg-gray-900/50 border border-gray-700 rounded-xl pl-11 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
                  />
                </div>
              </div>
              <div>
                <label className="block text-muted font-body text-xs font-semibold uppercase tracking-wide mb-2">Height (cm)</label>
                <div className="relative">
                  <Weight className="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
                  <input
                    type="number"
                    required
                    min={1}
                    value={height}
                    onChange={(e) => setHeight(e.target.value)}
                    placeholder="175"
                    className="w-full bg-gray-900/50 border border-gray-700 rounded-xl pl-11 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
                  />
                </div>
              </div>
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full bg-primary hover:bg-secondary text-white font-heading font-bold text-sm uppercase tracking-wide py-4 rounded-xl transition-colors duration-200 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2 mt-2"
            >
              {loading && <Loader2 className="w-4 h-4 animate-spin" />}
              {loading ? 'Creating Account…' : 'Create Account'}
            </button>
          </form>

          <p className="text-center text-muted font-body text-sm mt-6">
            Already have an account?{' '}
            <Link to="/login" className="text-primary hover:text-secondary transition-colors duration-200 font-semibold">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  )
}
