import { useState, useEffect } from 'react'
import { api } from '../lib/api'
import { Search, Dumbbell } from 'lucide-react'

type MuscleGroup = 'All' | 'Chest' | 'Back' | 'Legs' | 'Shoulders' | 'Arms' | 'Core'

const muscleGroups: MuscleGroup[] = ['All', 'Chest', 'Back', 'Legs', 'Shoulders', 'Arms', 'Core']

type Difficulty = 'Beginner' | 'Intermediate' | 'Advanced'

interface Exercise {
  id: string
  name: string
  muscleGroup: Exclude<MuscleGroup, 'All'>
  difficulty?: Difficulty | null
  description?: string | null
}

const difficultyColors: Record<Difficulty, string> = {
  Beginner: 'text-cta bg-cta/20 border-cta/30',
  Intermediate: 'text-primary bg-primary/20 border-primary/30',
  Advanced: 'text-red-400 bg-red-400/20 border-red-400/30',
}

const muscleGroupColors: Record<Exclude<MuscleGroup, 'All'>, string> = {
  Chest: 'text-blue-400 bg-blue-400/20',
  Back: 'text-purple-400 bg-purple-400/20',
  Legs: 'text-yellow-400 bg-yellow-400/20',
  Shoulders: 'text-cyan-400 bg-cyan-400/20',
  Arms: 'text-pink-400 bg-pink-400/20',
  Core: 'text-orange-400 bg-orange-400/20',
}

export default function Exercises() {
  const [exercises, setExercises] = useState<Exercise[]>([])
  const [activeGroup, setActiveGroup] = useState<MuscleGroup>('All')
  const [search, setSearch] = useState('')

  useEffect(() => {
    api.getExercises()
      .then((data) => {
        setExercises(data as Exercise[])
      })
      .catch((err) => console.error('Failed to fetch exercises', err))
  }, [])

  const filtered = exercises.filter((e) => {
    const matchGroup = activeGroup === 'All' || e.muscleGroup === activeGroup
    const matchSearch = e.name.toLowerCase().includes(search.toLowerCase()) ||
      e.muscleGroup.toLowerCase().includes(search.toLowerCase())
    return matchGroup && matchSearch
  })

  return (
    <div className="pt-28 pb-16 px-6 max-w-6xl mx-auto">
      <div className="mb-10">
        <h1 className="font-heading text-5xl md:text-6xl font-black text-foreground uppercase mb-2">
          Exercise Library
        </h1>
        <p className="text-muted font-body text-lg">Browse exercises by muscle group.</p>
      </div>

      {/* Search */}
      <div className="relative mb-6">
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-muted" />
        <input
          type="text"
          placeholder="Search exercises..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full bg-surface border border-gray-700 rounded-xl pl-12 pr-4 py-3.5 text-foreground font-body placeholder-muted focus:outline-none focus:border-primary transition-colors duration-200"
        />
      </div>

      {/* Muscle Group Filter Pills */}
      <div className="flex flex-wrap gap-3 mb-8">
        {muscleGroups.map((group) => (
          <button
            key={group}
            onClick={() => setActiveGroup(group)}
            className={`px-5 py-2.5 rounded-full font-heading font-bold text-sm uppercase tracking-wide transition-colors duration-200 cursor-pointer ${
              activeGroup === group
                ? 'bg-primary text-white'
                : 'bg-surface text-muted hover:text-foreground hover:bg-gray-600 border border-gray-700'
            }`}
          >
            {group}
          </button>
        ))}
      </div>

      {/* Exercise Count */}
      <p className="text-muted font-body text-sm mb-6">
        Showing <span className="text-primary font-bold">{filtered.length}</span> exercises
      </p>

      {/* Exercises Grid */}
      {filtered.length === 0 ? (
        <div className="bg-surface rounded-2xl border border-gray-700 p-12 text-center">
          <Dumbbell className="w-12 h-12 text-muted mx-auto mb-4" />
          <p className="text-muted font-body text-lg">No exercises found.</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((exercise) => (
            <div
              key={exercise.id}
              className="bg-surface rounded-2xl border border-gray-700 hover:border-primary/50 transition-colors duration-200 cursor-pointer group p-6"
            >
              <div className="flex items-start justify-between mb-4">
                <div className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-body font-semibold ${muscleGroupColors[exercise.muscleGroup]}`}>
                  {exercise.muscleGroup}
                </div>
                <div className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-body font-semibold border ${exercise.difficulty ? difficultyColors[exercise.difficulty] : 'text-muted bg-surface/20 border-gray-700'}`}>
                  {exercise.difficulty ?? 'Unknown'}
                </div>
              </div>

              <h3 className="font-heading text-xl font-bold text-foreground uppercase mb-2 group-hover:text-primary transition-colors duration-200">
                {exercise.name}
              </h3>

              <p className="text-muted font-body text-sm leading-relaxed">
                {exercise.description}
              </p>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
