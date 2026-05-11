# Exercise Progress Skill

## Purpose
Analyze per-exercise performance stats including total sets, total reps, total volume, and
average weight for a given exercise. Use this skill to detect strength progression, stagnation,
or overtraining on specific movements.

## When to use
Always start with **get_all_exercise_stats** to discover which exercises the user has data for.
This avoids guessing exercise names and missing data. Only call **get_exercise_stats** afterwards
if you need to drill into a specific exercise not already covered by the bulk result.
Pair with the Muscle Group Balance skill to verify exercise selection supports balanced development.

## Interpretation guidance
- High TotalVolume + low AverageWeight → high rep / endurance focus.
- Low TotalSets + high AverageWeight → strength/power focus.
- Stagnant AverageWeight across multiple observations → plateau; suggest progressive overload.
- Only one or two exercises per muscle group → limited variety; suggest diversification.
