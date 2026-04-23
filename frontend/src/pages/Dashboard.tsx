import { useState, useEffect } from 'react'
import { Dumbbell, Flame, TrendingUp, Weight, ChevronRight, Sparkles, Loader2, AlertCircle, CheckCircle, Lightbulb } from 'lucide-react'
import { Link } from 'react-router-dom'
import { api, WorkoutStatus } from '../lib/api'
import type { WorkoutSummaryDto, FitnessInsight } from '../lib/api'
import { useAuth } from '../context/AuthContext'

function formatVolume(kg: number): string {
  return kg >= 1000 ? `${(kg / 1000).toFixed(1)}t` : `${kg} kg`
}

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

function computeStats(history: WorkoutSummaryDto[]) {
  const now = new Date()
  const weekAgo = new Date(now)
  weekAgo.setDate(now.getDate() - 7)

  const thisWeek = history.filter((w) => new Date(w.date) >= weekAgo).length
  const totalVolume = history.reduce((sum, w) => sum + w.totalVolume, 0)

  // Simple streak: consecutive days with a workout going backwards from today
  const sortedDates = [...new Set(history.map((w) => w.date.slice(0, 10)))]
    .sort((a, b) => b.localeCompare(a))
  let streak = 0
  let cursor = new Date(now)
  cursor.setHours(0, 0, 0, 0)
  for (const d of sortedDates) {
    const wd = new Date(d)
    const diff = Math.round((cursor.getTime() - wd.getTime()) / 86400000)
    if (diff <= 1) {
      streak++
      cursor = wd
    } else {
      break
    }
  }

  return { thisWeek, totalVolume, streak }
}

export default function Dashboard() {
  const { user } = useAuth()
  const [history, setHistory] = useState<WorkoutSummaryDto[]>([])
  const [loadingHistory, setLoadingHistory] = useState(true)
  const [historyError, setHistoryError] = useState('')

  const [insight, setInsight] = useState<FitnessInsight | null>(null)
  const [loadingInsight, setLoadingInsight] = useState(false)
  const [insightError, setInsightError] = useState('')

  useEffect(() => {
    api.getWorkoutHistory()
      .then(setHistory)
      .catch(() => setHistoryError('Failed to load workout history.'))
      .finally(() => setLoadingHistory(false))
  }, [])

  async function handleGetInsight() {
    if (!user?.userId) return
    setInsight(null)
    setInsightError('')
    setLoadingInsight(true)
    try {
      const result = await api.getFitnessInsight(user.userId)
      setInsight(result)
    } catch {
      setInsightError('Could not generate insight. Please try again.')
    } finally {
      setLoadingInsight(false)
    }
  }

  const { thisWeek, totalVolume, streak } = computeStats(history)

  const statsCards = [
    { label: 'Total Workouts', value: loadingHistory ? '…' : String(history.length), icon: Dumbbell, color: 'text-primary', bg: 'bg-primary/20' },
    { label: 'This Week', value: loadingHistory ? '…' : String(thisWeek), icon: Flame, color: 'text-orange-400', bg: 'bg-orange-400/20' },
    { label: 'Current Streak', value: loadingHistory ? '…' : `${streak}d`, icon: TrendingUp, color: 'text-cta', bg: 'bg-cta/20' },
    { label: 'Total Volume', value: loadingHistory ? '…' : formatVolume(totalVolume), icon: Weight, color: 'text-purple-400', bg: 'bg-purple-400/20' },
  ]

  const recentWorkouts = history.slice(0, 5)

  // Mini bar chart: volume of last 10 workouts (keep ids for stable keys)
  const chartItems = history.slice(0, 10).map((w) => ({ id: w.workoutId, volume: w.totalVolume }))
  const chartMax = Math.max(...chartItems.map((c) => c.volume), 1)

  return (
    <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto">
      <div className="mb-12">
        <h1 className="font-heading text-5xl md:text-6xl font-black text-foreground uppercase mb-2">
          {user?.name ? `Hey, ${user.name.split(' ')[0]}` : 'Your Dashboard'}
        </h1>
        <p className="text-muted font-body text-lg">Here's your training overview.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-12">
        {statsCards.map((stat) => {
          const Icon = stat.icon
          return (
            <div
              key={stat.label}
              className="bg-surface rounded-2xl p-6 border border-gray-700 hover:border-primary transition-colors duration-200 cursor-pointer group"
            >
              <div className={`w-12 h-12 ${stat.bg} rounded-xl flex items-center justify-center mb-4`}>
                <Icon className={`w-6 h-6 ${stat.color}`} />
              </div>
              <div className={`font-heading text-3xl font-black ${stat.color} mb-1`}>
                {stat.value}
              </div>
              <div className="text-muted font-body text-sm font-semibold uppercase tracking-wide">
                {stat.label}
              </div>
            </div>
          )
        })}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-8">
        {/* Recent Workouts */}
        <div className="lg:col-span-2 bg-surface rounded-2xl border border-gray-700 overflow-hidden">
          <div className="flex items-center justify-between p-6 border-b border-gray-700">
            <h2 className="font-heading text-2xl font-bold text-foreground uppercase">Recent Workouts</h2>
            <Link
              to="/workouts"
              className="flex items-center gap-1 text-primary hover:text-secondary transition-colors duration-200 cursor-pointer text-sm font-body font-semibold"
            >
              View All <ChevronRight className="w-4 h-4" />
            </Link>
          </div>

          {historyError ? (
            <div className="p-6 text-red-400 font-body text-sm flex items-center gap-2">
              <AlertCircle className="w-4 h-4" /> {historyError}
            </div>
          ) : loadingHistory ? (
            <div className="p-6 flex items-center gap-2 text-muted font-body text-sm">
              <Loader2 className="w-4 h-4 animate-spin" /> Loading workouts…
            </div>
          ) : recentWorkouts.length === 0 ? (
            <div className="p-6 text-muted font-body text-sm">No workouts recorded yet.</div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="text-muted text-xs font-body font-semibold uppercase tracking-wide border-b border-gray-700">
                    <th className="text-left px-6 py-3">Date</th>
                    <th className="text-left px-6 py-3">Status</th>
                    <th className="text-right px-6 py-3">Exercises</th>
                    <th className="text-right px-6 py-3">Volume</th>
                  </tr>
                </thead>
                <tbody>
                  {recentWorkouts.map((workout, index) => (
                    <tr
                      key={workout.workoutId}
                      className={`hover:bg-gray-700/50 transition-colors duration-200 cursor-pointer ${
                        index < recentWorkouts.length - 1 ? 'border-b border-gray-700/50' : ''
                      }`}
                    >
                      <td className="px-6 py-4 text-muted font-body text-sm">{formatDate(workout.date)}</td>
                      <td className="px-6 py-4">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-body font-semibold ${
                          workout.status === WorkoutStatus.Completed
                            ? 'bg-cta/20 text-cta'
                            : 'bg-orange-400/20 text-orange-400'
                        }`}>
                          {workout.status === WorkoutStatus.Completed ? 'Completed' : 'In Progress'}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-right text-muted font-body text-sm">{workout.exerciseCount}</td>
                      <td className="px-6 py-4 text-right text-primary font-body font-bold text-sm">{formatVolume(workout.totalVolume)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        {/* Volume Chart */}
        <div className="bg-surface rounded-2xl border border-gray-700 p-6">
          <h2 className="font-heading text-2xl font-bold text-foreground uppercase mb-6">Volume Trend</h2>
          <div className="bg-gray-700/50 rounded-xl p-4 border border-gray-600">
            <div className="text-muted text-xs font-body font-semibold uppercase tracking-wide mb-3">Last 10 Workouts</div>
            {chartItems.length === 0 ? (
              <div className="h-24 flex items-center justify-center text-muted font-body text-xs">No data yet</div>
            ) : (
              <div className="h-24 flex items-end gap-1">
                {chartItems.map((item) => (
                  <div
                    key={item.id}
                    className="flex-1 bg-primary/40 hover:bg-primary rounded-t transition-colors duration-200 cursor-pointer"
                    style={{ height: `${Math.max(8, (item.volume / chartMax) * 100)}%` }}
                    title={formatVolume(item.volume)}
                  />
                ))}
              </div>
            )}
            {chartItems.length > 0 && (
              <div className="text-muted text-xs font-body mt-2 text-center">
                Peak: {formatVolume(chartMax)}
              </div>
            )}
          </div>

          <div className="mt-6 space-y-3">
            <div className="flex justify-between text-sm font-body">
              <span className="text-muted">Avg volume/session</span>
              <span className="text-foreground font-semibold">
                {history.length > 0 ? formatVolume(Math.round(totalVolume / history.length)) : '—'}
              </span>
            </div>
            <div className="flex justify-between text-sm font-body">
              <span className="text-muted">Completed</span>
              <span className="text-cta font-semibold">
                {history.filter((w) => w.status === WorkoutStatus.Completed).length} / {history.length}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* ── AI Suggestions ─────────────────────────────────────────── */}
      <div className="bg-surface rounded-2xl border border-gray-700 overflow-hidden">
        <div className="flex items-center justify-between p-6 border-b border-gray-700">
          <div className="flex items-center gap-3">
            <div className="w-10 h-10 bg-cta/20 rounded-xl flex items-center justify-center">
              <Sparkles className="w-5 h-5 text-cta" />
            </div>
            <div>
              <h2 className="font-heading text-2xl font-bold text-foreground uppercase">AI Coach</h2>
              <p className="text-muted font-body text-xs">Personalized insights based on your training data</p>
            </div>
          </div>
          <button
            onClick={handleGetInsight}
            disabled={loadingInsight || !user?.userId}
            className="flex items-center gap-2 bg-cta hover:bg-green-400 text-white font-heading font-bold text-sm uppercase tracking-wide px-5 py-2.5 rounded-xl transition-colors duration-200 cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
          >
            {loadingInsight ? (
              <><Loader2 className="w-4 h-4 animate-spin" /> Analysing…</>
            ) : (
              <><Sparkles className="w-4 h-4" /> {insight ? 'Refresh' : 'Get Suggestions'}</>
            )}
          </button>
        </div>

        <div className="p-6">
          {insightError && (
            <div className="flex items-center gap-3 bg-red-500/10 border border-red-500/30 rounded-xl px-4 py-3">
              <AlertCircle className="w-4 h-4 text-red-400 flex-shrink-0" />
              <p className="text-red-400 font-body text-sm">{insightError}</p>
            </div>
          )}

          {loadingInsight && !insight && (
            <div className="flex flex-col items-center justify-center py-12 text-muted gap-3">
              <Loader2 className="w-8 h-8 animate-spin text-cta" />
              <p className="font-body text-sm">FitnessCoach is analysing your training data…</p>
            </div>
          )}

          {!loadingInsight && !insight && !insightError && (
            <div className="flex flex-col items-center justify-center py-12 text-muted gap-3">
              <Sparkles className="w-10 h-10 text-gray-600" />
              <p className="font-body text-sm text-center max-w-sm">
                Click <strong className="text-cta">Get Suggestions</strong> to let your AI coach analyse your workout history and give you personalised advice.
              </p>
            </div>
          )}

          {insight && (
            <div className="space-y-6">
              {/* Summary */}
              <div className="bg-gray-700/40 rounded-xl p-5 border border-gray-600">
                <p className="text-foreground font-body text-sm leading-relaxed">{insight.summary}</p>
                <p className="text-muted font-body text-xs mt-3">
                  Generated {new Date(insight.generatedAt).toLocaleString()}
                </p>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Key Findings */}
                {insight.keyFindings.length > 0 && (
                  <div>
                    <h3 className="font-heading text-lg font-bold text-foreground uppercase mb-4 flex items-center gap-2">
                      <CheckCircle className="w-5 h-5 text-primary" /> Key Findings
                    </h3>
                    <ul className="space-y-3">
                      {insight.keyFindings.map((finding, i) => (
                        <li key={finding} className="flex items-start gap-3">
                          <span className="w-5 h-5 bg-primary/20 text-primary rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">
                            {i + 1}
                          </span>
                          <p className="text-muted font-body text-sm leading-relaxed">{finding}</p>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}

                {/* Personalised Advice */}
                {insight.personalizedAdvice.length > 0 && (
                  <div>
                    <h3 className="font-heading text-lg font-bold text-foreground uppercase mb-4 flex items-center gap-2">
                      <Lightbulb className="w-5 h-5 text-cta" /> Personalised Advice
                    </h3>
                    <ul className="space-y-3">
                      {insight.personalizedAdvice.map((advice, i) => (
                        <li key={advice} className="flex items-start gap-3">
                          <span className="w-5 h-5 bg-cta/20 text-cta rounded-full flex items-center justify-center text-xs font-bold flex-shrink-0 mt-0.5">
                            {i + 1}
                          </span>
                          <p className="text-muted font-body text-sm leading-relaxed">{advice}</p>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
