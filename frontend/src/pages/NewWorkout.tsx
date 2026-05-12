import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { Plus, Trash2, CheckCircle, Loader2, AlertCircle, Dumbbell, Search } from 'lucide-react'
import { api } from '../lib/api'
import type { ExerciseDto, SetInfo, CreateWorkoutResult } from '../lib/api'

interface SetDraft {
  reps: string
  weight: string
}

interface LoggedExercise {
  exerciseId: string
  exerciseName: string
  sets: SetInfo[]
}

const ALL_MUSCLE_GROUPS = ['All', 'Chest', 'Back', 'Legs', 'Shoulders', 'Arms', 'Abs']

function calc1RM(reps: number, weight: number): number {
  if (reps <= 0 || weight <= 0) return 0
  return Math.round(weight * (1 + reps / 30) * 100) / 100
}

export default function NewWorkout() {
  const navigate = useNavigate()

  const [workout, setWorkout] = useState<CreateWorkoutResult | null>(null)
  const [creating, setCreating] = useState(true)
  const [createError, setCreateError] = useState('')

  const [exercises, setExercises] = useState<ExerciseDto[]>([])
  const [search, setSearch] = useState('')
  const [muscleFilter, setMuscleFilter] = useState('All')

  const [selectedExercise, setSelectedExercise] = useState<ExerciseDto | null>(null)
  const [sets, setSets] = useState<SetDraft[]>([{ reps: '', weight: '' }])
  const [logging, setLogging] = useState(false)
  const [logError, setLogError] = useState('')

  const [logged, setLogged] = useState<LoggedExercise[]>([])
  const [completing, setCompleting] = useState(false)

  useEffect(() => {
    Promise.all([api.getInProgressWorkout(), api.getExercises()])
      .then(([inProgress, ex]) => {
        setExercises(ex)
        if (inProgress) {
          setWorkout({ workoutId: inProgress.id, userId: inProgress.userId.toString(), date: inProgress.date.toString(), status: inProgress.status })
          setCreating(false)
        } else {
          return api.createWorkout().then((w) => {
            setWorkout(w)
            setCreating(false)
          })
        }
      })
      .catch((err: Error) => {
        setCreateError(err.message || 'Failed to start workout.')
        setCreating(false)
      })
  }, [])

  const filteredExercises = exercises.filter((e) => {
    const matchesSearch = e.name.toLowerCase().includes(search.toLowerCase())
    const matchesMuscle = muscleFilter === 'All' || e.muscleGroup === muscleFilter
    return matchesSearch && matchesMuscle
  })

  function addSet() {
    setSets((prev) => [...prev, { reps: '', weight: '' }])
  }

  function removeSet(i: number) {
    setSets((prev) => prev.filter((_, idx) => idx !== i))
  }

  function updateSet(i: number, field: 'reps' | 'weight', value: string) {
    setSets((prev) => prev.map((s, idx) => idx === i ? { ...s, [field]: value } : s))
  }

  function selectExercise(ex: ExerciseDto) {
    setSelectedExercise(ex)
    setSets([{ reps: '', weight: '' }])
    setLogError('')
  }

  async function handleLogExercise() {
    if (!workout || !selectedExercise) return

    const parsedSets: SetInfo[] = sets.map((s, i) => {
      const reps = Number.parseInt(s.reps) || 0
      const weight = Number.parseFloat(s.weight) || 0
      return {
        index: i,
        reps,
        weight,
        estimated1Rm: calc1RM(reps, weight),
      }
    })

    if (parsedSets.some((s) => s.reps <= 0 || s.weight <= 0)) {
      setLogError('All sets must have valid reps and weight.')
      return
    }

    setLogging(true)
    setLogError('')
    try {
      await api.addExerciseToWorkout(workout.workoutId, selectedExercise.id, parsedSets)
      setLogged((prev) => [...prev, { exerciseId: selectedExercise.id, exerciseName: selectedExercise.name, sets: parsedSets }])
      setSelectedExercise(null)
      setSets([{ reps: '', weight: '' }])
    } catch (err: unknown) {
      setLogError(err instanceof Error ? err.message : 'Failed to log exercise.')
    } finally {
      setLogging(false)
    }
  }

  async function handleComplete() {
    if (!workout) return
    setCompleting(true)
    try {
      await api.completeWorkout(workout.workoutId)
      navigate('/workouts')
    } catch (err: unknown) {
      setCreateError(err instanceof Error ? err.message : 'Failed to complete workout.')
      setCompleting(false)
    }
  }

  if (creating) {
    return (
      <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto flex items-center justify-center">
        <div className="flex items-center gap-3 text-muted font-body">
          <Loader2 className="w-6 h-6 animate-spin" /> Starting workout…
        </div>
      </div>
    )
  }

  if (createError && !workout) {
    return (
      <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto">
        <div className="flex items-center gap-3 bg-red-500/10 border border-red-500/30 rounded-xl px-4 py-3">
          <AlertCircle className="w-4 h-4 text-red-400 flex-shrink-0" />
          <p className="text-red-400 font-body text-sm">{createError}</p>
        </div>
      </div>
    )
  }

  return (
    <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto">
      <div className="mb-10 flex items-start justify-between gap-4 flex-wrap">
        <div>
          <h1 className="font-heading text-5xl md:text-6xl font-black text-foreground uppercase mb-2">
            Log Workout
          </h1>
          <p className="text-muted font-body text-lg">Select exercises, add your sets, then complete.</p>
        </div>
        <button
          onClick={handleComplete}
          disabled={completing || logged.length === 0}
          className="flex items-center gap-2 bg-cta hover:bg-green-400 text-white font-heading font-bold text-sm uppercase tracking-wide px-6 py-3 rounded-xl transition-colors duration-200 cursor-pointer disabled:opacity-40 disabled:cursor-not-allowed"
        >
          {completing ? <Loader2 className="w-4 h-4 animate-spin" /> : <CheckCircle className="w-4 h-4" />}
          Complete Workout
        </button>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Left: Exercise Picker */}
        <div className="bg-surface rounded-2xl border border-gray-700 overflow-hidden">
          <div className="p-5 border-b border-gray-700">
            <h2 className="font-heading text-xl font-bold text-foreground uppercase mb-4">Pick an Exercise</h2>
            <div className="relative mb-3">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-muted" />
              <input
                type="text"
                placeholder="Search exercises…"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="w-full bg-gray-700 border border-gray-600 rounded-xl pl-9 pr-4 py-2.5 text-sm font-body text-foreground placeholder-muted focus:outline-none focus:border-primary transition-colors"
              />
            </div>
            <div className="flex gap-2 flex-wrap">
              {ALL_MUSCLE_GROUPS.map((mg) => (
                <button
                  key={mg}
                  onClick={() => setMuscleFilter(mg)}
                  className={`px-3 py-1 rounded-lg text-xs font-heading font-bold uppercase tracking-wide transition-colors cursor-pointer ${
                    muscleFilter === mg ? 'bg-primary text-white' : 'bg-gray-700 text-muted hover:text-foreground'
                  }`}
                >
                  {mg}
                </button>
              ))}
            </div>
          </div>

          <div className="overflow-y-auto max-h-96">
            {filteredExercises.length === 0 ? (
              <p className="p-5 text-muted font-body text-sm">No exercises found.</p>
            ) : (
              filteredExercises.map((ex) => {
                const isLogged = logged.some((l) => l.exerciseId === ex.id)
                const isSelected = selectedExercise?.id === ex.id
                return (
                  <button
                    key={ex.id}
                    onClick={() => selectExercise(ex)}
                    className={`w-full flex items-center justify-between px-5 py-3.5 text-left border-b border-gray-700/50 last:border-0 transition-colors cursor-pointer ${
                      isSelected ? 'bg-primary/20' : 'hover:bg-gray-700/40'
                    }`}
                  >
                    <div>
                      <div className="font-heading font-bold text-sm text-foreground uppercase">{ex.name}</div>
                      <div className="text-muted text-xs font-body">{ex.muscleGroup}</div>
                    </div>
                    {isLogged && <CheckCircle className="w-4 h-4 text-cta flex-shrink-0" />}
                  </button>
                )
              })
            )}
          </div>
        </div>

        {/* Right: Set Builder + Logged */}
        <div className="space-y-5">
          {/* Set Builder */}
          <div className="bg-surface rounded-2xl border border-gray-700 p-5">
            {selectedExercise ? (
              <>
                <h2 className="font-heading text-xl font-bold text-foreground uppercase mb-4">
                  {selectedExercise.name}
                </h2>

                <div className="space-y-2 mb-4">
                  {sets.map((set, i) => (
                    <div key={`set-${selectedExercise.id}-${i}`} className="flex items-center gap-2">
                      <span className="text-muted font-body text-sm w-8 text-center">{i + 1}</span>
                      <input
                        type="number"
                        placeholder="Reps"
                        min={1}
                        value={set.reps}
                        onChange={(e) => updateSet(i, 'reps', e.target.value)}
                        className="flex-1 bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-sm font-body text-foreground placeholder-muted focus:outline-none focus:border-primary transition-colors"
                      />
                      <input
                        type="number"
                        placeholder="kg"
                        min={0}
                        step={0.5}
                        value={set.weight}
                        onChange={(e) => updateSet(i, 'weight', e.target.value)}
                        className="flex-1 bg-gray-700 border border-gray-600 rounded-lg px-3 py-2 text-sm font-body text-foreground placeholder-muted focus:outline-none focus:border-primary transition-colors"
                      />
                      {sets.length > 1 && (
                        <button onClick={() => removeSet(i)} className="text-muted hover:text-red-400 transition-colors cursor-pointer">
                          <Trash2 className="w-4 h-4" />
                        </button>
                      )}
                    </div>
                  ))}
                </div>

                <button
                  onClick={addSet}
                  className="flex items-center gap-1.5 text-muted hover:text-primary font-body text-sm transition-colors cursor-pointer mb-4"
                >
                  <Plus className="w-4 h-4" /> Add Set
                </button>

                {logError && (
                  <div className="flex items-center gap-2 text-red-400 font-body text-sm mb-3">
                    <AlertCircle className="w-4 h-4 flex-shrink-0" /> {logError}
                  </div>
                )}

                <button
                  onClick={handleLogExercise}
                  disabled={logging}
                  className="w-full flex items-center justify-center gap-2 bg-primary hover:bg-secondary text-white font-heading font-bold text-sm uppercase tracking-wide py-3 rounded-xl transition-colors cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {logging ? <Loader2 className="w-4 h-4 animate-spin" /> : <Plus className="w-4 h-4" />}
                  Log Exercise
                </button>
              </>
            ) : (
              <div className="flex flex-col items-center justify-center py-10 text-muted gap-3">
                <Dumbbell className="w-10 h-10 text-gray-600" />
                <p className="font-body text-sm">Select an exercise to log sets.</p>
              </div>
            )}
          </div>

          {/* Logged Exercises */}
          {logged.length > 0 && (
            <div className="bg-surface rounded-2xl border border-gray-700 p-5">
              <h2 className="font-heading text-xl font-bold text-foreground uppercase mb-4">Logged</h2>
              <div className="space-y-3">
                {logged.map((ex) => (
                  <div key={ex.exerciseId} className="bg-gray-700/30 rounded-xl p-4">
                    <div className="font-heading font-bold text-sm text-foreground uppercase mb-2">{ex.exerciseName}</div>
                    <div className="flex flex-wrap gap-2">
                      {ex.sets.map((s, si) => (
                        <span key={`${ex.exerciseId}-${si}`} className="bg-primary/20 text-primary text-xs font-body font-semibold px-2.5 py-1 rounded-lg">
                          {s.reps} reps × {s.weight} kg
                        </span>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
