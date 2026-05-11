# Muscle Group Balance Skill

## Purpose
Analyze training volume and intensity distribution across muscle groups: Chest, Back, Shoulders,
Arms, Legs and Abs. Use this skill to detect imbalances that could lead to injury, posture
problems, or aesthetic asymmetry.

## When to use
Always use **get_all_muscle_group_stats** to retrieve all muscle groups in a single call instead
of querying each group individually. This ensures no group is missed and reduces latency.
Use this skill after gathering workout and exercise data to evaluate whether the user trains
muscle groups in a balanced way. It is essential for generating structural advice about training
program design.

## Interpretation guidance
- Legs TotalVolume significantly lower than upper body → common imbalance; strongly recommend leg days.
- Back TotalVolume < Chest TotalVolume → anterior dominance risk (posture); recommend more pulling movements.
- Abs TotalSets very low → core neglect; suggest core work for injury prevention.
- One muscle group with > 40% of total volume → overdevelopment risk; recommend redistribution.
- AverageIntensity < 30% → exercises may be too light for adaptation.
