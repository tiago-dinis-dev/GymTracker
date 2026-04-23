const BASE_URL = 'https://localhost:7017'

function getToken(): string | null {
  return localStorage.getItem('gymtracker_token')
}

function authHeaders(): Record<string, string> {
  const token = getToken()
  const headers: Record<string, string> = { 'Content-Type': 'application/json' }
  if (token) headers['Authorization'] = `Bearer ${token}`
  return headers
}

async function request<T>(path: string, options?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    ...options,
    headers: { ...authHeaders(), ...(options?.headers as Record<string, string>) },
  })
  if (!res.ok) {
    const body = await res.json().catch(() => null)
    throw new Error(body?.error ?? `${res.status} ${res.statusText}`)
  }
  return res.json() as Promise<T>
}

export interface SetInfo {
  index: number
  reps: number
  weight: number
  estimated1Rm: number
}

export interface CreateWorkoutResult {
  workoutId: string
  userId: string
  date: string
  status: string
}

export const WorkoutStatus = {
  InProgress: 0,
  Completed: 1,
} as const

export type WorkoutStatus = (typeof WorkoutStatus)[keyof typeof WorkoutStatus]

export interface WorkoutSummaryDto {
  workoutId: string
  userId: string
  date: string
  status: WorkoutStatus
  exerciseCount: number
  totalVolume: number
}

export interface SetRecordDto {
  reps: number
  weight: number
  intensityPercent1Rm: number
}

export interface ExercisePerformedDto {
  exerciseId: string
  sets: SetRecordDto[]
}

export interface WorkoutDetailsDto {
  id: string
  userId: string
  date: string
  status: string
  exercises: ExercisePerformedDto[]
}

export interface FitnessInsight {
  userId: string
  summary: string
  keyFindings: string[]
  personalizedAdvice: string[]
  generatedAt: string
}

export interface ExerciseDto {
  id: string
  name: string
  muscleGroup: string
  difficulty?: string | null
  description?: string | null
}

export const api = {
  login: (email: string, password: string) =>
    request<{ token: string }>('/api/user/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),

  register: (name: string, email: string, weight: number, height: number) =>
    request<{ token: string }>('/api/user/register', {
      method: 'POST',
      body: JSON.stringify({ name, email, weight, height }),
    }),

  getWorkoutHistory: () =>
    request<WorkoutSummaryDto[]>('/api/workout-history'),

  getWorkoutById: (workoutId: string) =>
    request<WorkoutDetailsDto>(`/api/workouts/${workoutId}`),

  getFitnessInsight: (userId: string) =>
    request<FitnessInsight>(`/api/fitness/insight/${userId}`),

  getExercises: () =>
    request<ExerciseDto[]>('/api/exercises'),

  getInProgressWorkout: () =>
    fetch(`${BASE_URL}/api/workouts/in-progress`, { headers: authHeaders() })
      .then((res) => {
        if (res.status === 204) return null
        if (!res.ok) throw new Error(`${res.status} ${res.statusText}`)
        return res.json() as Promise<WorkoutDetailsDto>
      }),

  createWorkout:(date: string) =>
    request<CreateWorkoutResult>('/api/workouts', {
      method: 'POST',
      body: JSON.stringify(date),
    }),

  addExerciseToWorkout: (workoutId: string, exerciseId: string, sets: SetInfo[]) =>
    request<{ workoutId: string; exerciseName: string }>(`/api/workout-exercises/${workoutId}`, {
      method: 'POST',
      body: JSON.stringify({ exerciseId, sets }),
    }),

  completeWorkout: (workoutId: string) =>
    request<void>(`/api/workouts/${workoutId}/complete`, {
      method: 'POST',
    }),
}
