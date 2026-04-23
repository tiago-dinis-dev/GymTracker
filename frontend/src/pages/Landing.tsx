import { Link } from 'react-router-dom'
import { Dumbbell, BarChart2, Calendar, ArrowRight, Zap, Trophy, Users } from 'lucide-react'

const features = [
  {
    icon: Dumbbell,
    title: 'Log Every Rep',
    description: 'Track every set, rep, and weight with precision. Never lose progress again.',
  },
  {
    icon: BarChart2,
    title: 'Visualize Progress',
    description: 'Beautiful charts show your strength gains over time. Stay motivated.',
  },
  {
    icon: Calendar,
    title: 'Smart Scheduling',
    description: 'Plan workouts in advance and follow structured training programs.',
  },
]

const stats = [
  { value: '1,000+', label: 'Workouts Tracked' },
  { value: '50+', label: 'Exercises' },
  { value: '24/7', label: 'Access' },
]

export default function Landing() {
  return (
    <div className="overflow-x-hidden">
      {/* Hero */}
      <section className="relative min-h-screen flex items-center justify-center bg-background pt-24">
        <div className="absolute inset-0 overflow-hidden pointer-events-none">
          <div className="absolute top-1/4 left-1/4 w-96 h-96 bg-primary/10 rounded-full blur-3xl animate-pulse" />
          <div className="absolute bottom-1/4 right-1/4 w-64 h-64 bg-cta/10 rounded-full blur-3xl animate-pulse delay-1000" />
        </div>

        <div className="relative max-w-6xl mx-auto px-6 text-center">
          <div className="inline-flex items-center gap-2 bg-primary/20 border border-primary/30 rounded-full px-4 py-2 mb-8">
            <Zap className="w-4 h-4 text-primary" />
            <span className="text-primary text-sm font-body font-semibold">Your Ultimate Fitness Companion</span>
          </div>

          <h1 className="font-heading text-6xl md:text-8xl lg:text-9xl font-black text-foreground leading-none mb-6 uppercase tracking-tight">
            TRACK YOUR
            <span className="block text-primary">GAINS</span>
          </h1>

          <p className="text-muted text-lg md:text-xl font-body max-w-2xl mx-auto mb-10 leading-relaxed">
            Stop guessing. Start progressing. GymTracker gives you the tools to log workouts,
            analyze your performance, and crush every personal record.
          </p>

          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              to="/dashboard"
              className="inline-flex items-center justify-center gap-2 bg-cta hover:bg-green-400 text-white font-heading font-bold text-lg px-8 py-4 rounded-xl transition-colors duration-200 cursor-pointer uppercase tracking-wide"
            >
              Start Tracking Today
              <ArrowRight className="w-5 h-5" />
            </Link>
            <Link
              to="/exercises"
              className="inline-flex items-center justify-center gap-2 bg-surface hover:bg-gray-600 text-foreground font-heading font-bold text-lg px-8 py-4 rounded-xl transition-colors duration-200 cursor-pointer uppercase tracking-wide border border-gray-600"
            >
              Browse Exercises
            </Link>
          </div>
        </div>
      </section>

      {/* Stats */}
      <section className="bg-primary py-16">
        <div className="max-w-6xl mx-auto px-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8 text-center">
            {stats.map((stat) => (
              <div key={stat.label} className="group">
                <div className="font-heading text-5xl md:text-6xl font-black text-white mb-2 group-hover:scale-110 transition-transform duration-200">
                  {stat.value}
                </div>
                <div className="text-orange-100 font-body font-semibold text-lg uppercase tracking-wide">
                  {stat.label}
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Features */}
      <section className="py-24 bg-background">
        <div className="max-w-6xl mx-auto px-6">
          <div className="text-center mb-16">
            <h2 className="font-heading text-4xl md:text-6xl font-black text-foreground uppercase mb-4">
              Everything You Need
            </h2>
            <p className="text-muted font-body text-lg max-w-xl mx-auto">
              Purpose-built tools to take your training to the next level.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {features.map((feature) => {
              const Icon = feature.icon
              return (
                <div
                  key={feature.title}
                  className="bg-surface rounded-2xl p-8 border border-gray-700 hover:border-primary transition-colors duration-200 group cursor-pointer"
                >
                  <div className="w-14 h-14 bg-primary/20 rounded-xl flex items-center justify-center mb-6 group-hover:bg-primary/30 transition-colors duration-200">
                    <Icon className="w-7 h-7 text-primary" />
                  </div>
                  <h3 className="font-heading text-2xl font-bold text-foreground mb-3 uppercase">
                    {feature.title}
                  </h3>
                  <p className="text-muted font-body leading-relaxed">
                    {feature.description}
                  </p>
                </div>
              )
            })}
          </div>
        </div>
      </section>

      {/* Social proof */}
      <section className="py-24 bg-gray-900">
        <div className="max-w-6xl mx-auto px-6">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            {[
              { icon: Trophy, title: 'PRs Broken', value: '234', color: 'text-yellow-400' },
              { icon: Users, title: 'Active Users', value: '512', color: 'text-cta' },
              { icon: Zap, title: 'Workouts This Week', value: '89', color: 'text-primary' },
            ].map((item) => {
              const Icon = item.icon
              return (
                <div key={item.title} className="flex items-center gap-4 bg-surface rounded-2xl p-6 border border-gray-700">
                  <Icon className={`w-10 h-10 ${item.color} flex-shrink-0`} />
                  <div>
                    <div className={`font-heading text-4xl font-black ${item.color}`}>{item.value}</div>
                    <div className="text-muted font-body text-sm font-semibold uppercase tracking-wide">{item.title}</div>
                  </div>
                </div>
              )
            })}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="py-24 bg-primary">
        <div className="max-w-6xl mx-auto px-6 text-center">
          <h2 className="font-heading text-4xl md:text-7xl font-black text-white uppercase mb-6">
            Ready to Level Up?
          </h2>
          <p className="text-orange-100 font-body text-xl mb-10 max-w-xl mx-auto">
            Join hundreds of athletes already tracking their progress with GymTracker.
          </p>
          <Link
            to="/dashboard"
            className="inline-flex items-center justify-center gap-2 bg-white hover:bg-gray-100 text-primary font-heading font-black text-xl px-10 py-5 rounded-xl transition-colors duration-200 cursor-pointer uppercase tracking-wide"
          >
            Start Tracking Today
            <ArrowRight className="w-6 h-6" />
          </Link>
        </div>
      </section>
    </div>
  )
}
