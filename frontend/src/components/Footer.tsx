import { Dumbbell, ExternalLink, X } from 'lucide-react'
import { Link } from 'react-router-dom'

export default function Footer() {
  return (
    <footer className="bg-gray-900 border-t border-gray-700 mt-24">
      <div className="max-w-6xl mx-auto px-6 py-12">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div>
            <Link to="/" className="flex items-center gap-2 mb-4 cursor-pointer">
              <Dumbbell className="w-6 h-6 text-primary" />
              <span className="font-heading text-xl font-bold">
                <span className="text-primary">Gym</span>
                <span className="text-foreground">Tracker</span>
              </span>
            </Link>
            <p className="text-muted text-sm font-body leading-relaxed">
              Track your gains, crush your goals. The ultimate workout companion for serious athletes.
            </p>
          </div>

          <div>
            <h4 className="font-heading text-lg font-bold text-foreground mb-4 uppercase tracking-wide">Navigation</h4>
            <ul className="space-y-2">
              {[
                { label: 'Dashboard', to: '/dashboard' },
                { label: 'Workouts', to: '/workouts' },
                { label: 'Exercises', to: '/exercises' },
              ].map((link) => (
                <li key={link.to}>
                  <Link
                    to={link.to}
                    className="text-muted hover:text-primary transition-colors duration-200 cursor-pointer text-sm font-body"
                  >
                    {link.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          <div>
            <h4 className="font-heading text-lg font-bold text-foreground mb-4 uppercase tracking-wide">Connect</h4>
            <div className="flex gap-4">
              <a href="#" className="text-muted hover:text-primary transition-colors duration-200 cursor-pointer">
                <ExternalLink className="w-5 h-5" />
              </a>
              <a href="#" className="text-muted hover:text-primary transition-colors duration-200 cursor-pointer">
                <X className="w-5 h-5" />
              </a>
            </div>
          </div>
        </div>

        <div className="border-t border-gray-700 mt-8 pt-8 text-center">
          <p className="text-muted text-sm font-body">
            © {new Date().getFullYear()} GymTracker. Built for athletes, by athletes.
          </p>
        </div>
      </div>
    </footer>
  )
}
