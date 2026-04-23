namespace Domain.Progress;

public class WeeklyProgress
{
    public Dictionary<string, float> VolumePerMuscle { get; } = new();
    public void AddVolume(string muscleGroup, float volume)
    {
        if (VolumePerMuscle.ContainsKey(muscleGroup))
        {
            VolumePerMuscle[muscleGroup] += volume;
        }
        else
        {
            VolumePerMuscle[muscleGroup] = volume;
        }
    }
}
