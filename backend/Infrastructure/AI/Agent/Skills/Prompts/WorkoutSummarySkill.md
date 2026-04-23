# Workout Summary Skill

## Purpose
Analyze a user's overall workout habits using aggregated stats such as total workouts completed,
total volume lifted, average volume per session, average workout duration, and the date of the
last workout. Use this skill to evaluate training consistency, volume progression and frequency trends.

## When to use
Use this skill first when generating a fitness insight. It provides the high-level picture of
how active and consistent the user is. Without this data the other skills lack context.

## Interpretation guidance
- TotalWorkouts < 5 → beginner or very inconsistent; encourage building a habit.
- AverageWorkoutDuration < 20 min → sessions are short; suggest extending if goals allow.
- LastWorkoutAt > 7 days ago → user may be losing momentum; motivational note recommended.
- High TotalVolume with low TotalWorkouts → intense but infrequent training.
