import { useState, useEffect, type JSX } from 'react'
import { Calendar, ChevronDown, ChevronUp, Dumbbell, Loader2, AlertCircle } from 'lucide-react'
import { api, WorkoutStatus } from '../lib/api'
import type { WorkoutSummaryDto, WorkoutDetailsDto } from '../lib/api'

type FilterType = 'all' | 'week' | 'month'

function formatDate(iso: string): string {
  return new Date(iso).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

function ExpandedDetails({
  isLoadingDetail,
  detail,
  exerciseNames,
}: Readonly<{
  isLoadingDetail: boolean
  detail: WorkoutDetailsDto | undefined
  exerciseNames: Record<string, string>
}>) {
  let content: JSX.Element

  if (isLoadingDetail) {
    content = (
      <div className="flex items-center gap-2 text-muted font-body text-sm py-4">
        <Loader2 className="w-4 h-4 animate-spin" /> Loading exercises…
      </div>
    )
  } else if (detail) {
    content = (
      <div className="space-y-4">
        {detail.exercises.map((ex, exIdx) => (
          <div key={ex.exerciseId} className="bg-gray-700/30 rounded-xl p-4">
            <div className="text-foreground font-heading font-bold uppercase text-sm mb-3">
              Exercise {exIdx + 1}: {exerciseNames[ex.exerciseId] ?? 'Unknown exercise'}
            </div>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="text-muted text-xs font-body font-semibold uppercase tracking-wide border-b border-gray-600">
                    <th className="text-left py-1.5">Set</th>
                    <th className="text-center py-1.5">Reps</th>
                    <th className="text-center py-1.5">Weight</th>
                    <th className="text-right py-1.5">Intensity</th>
                  </tr>
                </thead>
                <tbody>
                  {ex.sets.map((set, setIdx) => (
                    <tr key={`${ex.exerciseId}-${setIdx}`} className="border-b border-gray-600/50 last:border-0">
                      <td className="py-2 text-muted font-body text-sm">{setIdx + 1}</td>
                      <td className="py-2 text-center text-foreground font-body font-semibold">{set.reps}</td>
                      <td className="py-2 text-center text-primary font-body font-bold">{set.weight} kg</td>
                      <td className="py-2 text-right text-muted font-body text-sm">
                        {set.intensityPercent1Rm > 0 ? `${Number(set.intensityPercent1Rm).toFixed(1)}%` : '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        ))}
        {detail.exercises.length === 0 && (
          <p className="text-muted font-body text-sm">No exercises recorded.</p>
        )}
      </div>
    )
  } else {
    content = <p className="text-muted font-body text-sm">Could not load details.</p>
  }

  return (
    <div className="border-t border-gray-700 px-6 pb-6 pt-4">{content}</div>
  )
}

export default function WorkoutLog() {
  const [workouts, setWorkouts] = useState<WorkoutSummaryDto[]>([])
  const [exerciseNames, setExerciseNames] = useState<Record<string, string>>({})
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const [expandedId, setExpandedId] = useState<string | null>(null)
  const [details, setDetails] = useState<Record<string, WorkoutDetailsDto>>({})
  const [loadingDetails, setLoadingDetails] = useState<Record<string, boolean>>({})

  const [filter, setFilter] = useState<FilterType>('all')

  const getFilterLabel = (f: FilterType): string => {
    if (f === 'all') return 'All Time'
    if (f === 'week') return 'Last Week'
    return 'Last Month'
  }

  useEffect(() => {
    api.getWorkoutHistory()
      .then(setWorkouts)
      .catch(() => setError('Failed to load workouts.'))
      .finally(() => setLoading(false))
  }, [])

  useEffect(() => {
    api.getExercises()
      .then((exercises) => {
        setExerciseNames(Object.fromEntries(exercises.map((exercise) => [exercise.id, exercise.name])))
      })
      .catch(() => {
        setExerciseNames({})
      })
  }, [])

  async function toggleExpand(workoutId: string) {
    if (expandedId === workoutId) {
      setExpandedId(null)
      return
    }
    setExpandedId(workoutId)
    if (!details[workoutId]) {
      setLoadingDetails((prev) => ({ ...prev, [workoutId]: true }))
      try {
        const d = await api.getWorkoutById(workoutId)
        setDetails((prev) => ({ ...prev, [workoutId]: d }))
      } finally {
        setLoadingDetails((prev) => ({ ...prev, [workoutId]: false }))
      }
    }
  }

  const now = new Date()
  const filteredWorkouts = workouts.filter((w) => {
    const d = new Date(w.date)
    if (filter === 'week') {
      const weekAgo = new Date(now); weekAgo.setDate(now.getDate() - 7)
      return d >= weekAgo
    }
    if (filter === 'month') {
      const monthAgo = new Date(now); monthAgo.setMonth(now.getMonth() - 1)
      return d >= monthAgo
    }
    return true
  })

  return (
    <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto">
      <div className="mb-10">
        <h1 className="font-heading text-5xl md:text-6xl font-black text-foreground uppercase mb-2">
          Workout Log
        </h1>
        <p className="text-muted font-body text-lg">All your training sessions in one place.</p>
      </div>

      {/* Filter Buttons */}
      <div className="flex gap-3 mb-8">
        {(['all', 'week', 'month'] as FilterType[]).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`px-5 py-2.5 rounded-xl font-heading font-bold text-sm uppercase tracking-wide transition-colors duration-200 cursor-pointer ${
              filter === f
                ? 'bg-primary text-white'
                : 'bg-surface text-muted hover:text-foreground hover:bg-gray-600 border border-gray-700'
            }`}
          >
            {getFilterLabel(f)}
          </button>
        ))}
      </div>

      {/* States */}
      {error && (
        <div className="flex items-center gap-3 bg-red-500/10 border border-red-500/30 rounded-xl px-4 py-3 mb-6">
          <AlertCircle className="w-4 h-4 text-red-400 flex-shrink-0" />
          <p className="text-red-400 font-body text-sm">{error}</p>
        </div>
      )}

      {loading && (
        <div className="flex items-center gap-2 text-muted font-body text-sm py-12 justify-center">
          <Loader2 className="w-5 h-5 animate-spin" /> Loading workouts…
        </div>
      )}

      {/* Workouts List */}
      {!loading && (
        <div className="space-y-4">
          {filteredWorkouts.length === 0 ? (
            <div className="bg-surface rounded-2xl border border-gray-700 p-12 text-center">
              <Dumbbell className="w-12 h-12 text-muted mx-auto mb-4" />
              <p className="text-muted font-body text-lg">No workouts found for this period.</p>
            </div>
          ) : (
            filteredWorkouts.map((workout) => {
              const isExpanded = expandedId === workout.workoutId
              const detail = details[workout.workoutId]
              const isLoadingDetail = loadingDetails[workout.workoutId]

              const statusBadgeClass = workout.status === WorkoutStatus.Completed
                ? 'bg-cta/20 text-cta'
                : 'bg-orange-400/20 text-orange-400'

              const statusLabel = workout.status === WorkoutStatus.Completed ? 'Completed' : 'In Progress'

              const chevron = isExpanded ? (
                <ChevronUp className="w-5 h-5 text-muted" />
              ) : (
                <ChevronDown className="w-5 h-5 text-muted" />
              )

              const volumeLabel = workout.totalVolume > 0 ? `${workout.totalVolume} kg` : '—'

              return (
                <div
                  key={workout.workoutId}
                  className="bg-surface rounded-2xl border border-gray-700 hover:border-primary/50 transition-colors duration-200 overflow-hidden"
                >
                  {isExpanded && (
                    <ExpandedDetails
                      isLoadingDetail={isLoadingDetail}
                      detail={detail}
                      exerciseNames={exerciseNames}
                    />
                  )}

                  <button
                    className="w-full flex items-center justify-between p-6 cursor-pointer text-left"
                    onClick={() => toggleExpand(workout.workoutId)}
                  >
                    <div className="flex items-center gap-4">
                      <div className="w-12 h-12 bg-primary/20 rounded-xl flex items-center justify-center flex-shrink-0">
                        <Dumbbell className="w-6 h-6 text-primary" />
                      </div>
                      <div>
                        <div className="font-heading text-xl font-bold text-foreground uppercase">
                          Workout — {formatDate(workout.date)}
                        </div>
                        <div className="flex items-center gap-3 text-muted text-sm font-body">
                          <span className="flex items-center gap-1">
                            <Calendar className="w-3.5 h-3.5" />
                            {formatDate(workout.date)}
                          </span>
                          <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-body font-semibold ${statusBadgeClass}`}>
                            {statusLabel}
                          </span>
                        </div>
                      </div>
                    </div>

                    <div className="flex items-center gap-6">
                      <div className="hidden sm:flex gap-6 text-right">
                        <div>
                          <div className="font-heading text-lg font-bold text-primary">{workout.exerciseCount}</div>
                          <div className="text-muted text-xs font-body uppercase tracking-wide">Exercises</div>
                        </div>
                        <div>
                          <div className="font-heading text-lg font-bold text-foreground">{volumeLabel}</div>
                          <div className="text-muted text-xs font-body uppercase tracking-wide">Volume</div>
                        </div>
                      </div>
                      {chevron}
                    </div>
                  </button>
                </div>
              )
            })
          )}
        </div>
      )}
    </div>
  )
}
