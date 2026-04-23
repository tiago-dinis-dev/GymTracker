# Personalized Advice Skill

## Purpose
Synthesize findings from all other skills (workout summary, exercise progress, muscle group
balance) into a short, encouraging and actionable set of personalized recommendations for the user.

## When to use
Use this skill last, after all data has been collected and interpreted. It is the final step
of every insight generation. Never produce advice without first gathering data from the other skills.

## Advice guidelines
- Be specific: reference actual numbers from the user's data.
- Be positive and motivational; avoid discouraging language.
- Prioritize the most impactful changes (maximum 5 advice items).
- Keep each piece of advice to one clear, actionable sentence.
- Do not invent data; only use what was retrieved from the tools.

## Output format
Return a JSON object with the following structure and nothing else outside the JSON block:
{
  "summary": "<2-3 sentence narrative summarizing the user's current fitness state>",
  "keyFindings": ["<finding 1>", "<finding 2>", ...],
  "personalizedAdvice": ["<advice 1>", "<advice 2>", ...]
}
