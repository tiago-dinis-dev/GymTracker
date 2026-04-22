using Domain.Progress;

namespace Domain.Tests.Progress;

public class WeeklyProgressTests
{
    [Fact]
    public void AddVolume_NewMuscleGroup_AddsEntry()
    {
        var progress = new WeeklyProgress();

        progress.AddVolume("Chest", 1000f);

        Assert.Single(progress.VolumePerMuscle);
        Assert.Equal(1000f, progress.VolumePerMuscle["Chest"]);
    }

    [Fact]
    public void AddVolume_ExistingMuscleGroup_AccumulatesVolume()
    {
        var progress = new WeeklyProgress();

        progress.AddVolume("Chest", 1000f);
        progress.AddVolume("Chest", 500f);

        Assert.Equal(1500f, progress.VolumePerMuscle["Chest"]);
    }

    [Fact]
    public void AddVolume_MultipleMuscleGroups_TrackedSeparately()
    {
        var progress = new WeeklyProgress();

        progress.AddVolume("Chest", 1000f);
        progress.AddVolume("Back", 800f);

        Assert.Equal(2, progress.VolumePerMuscle.Count);
        Assert.Equal(1000f, progress.VolumePerMuscle["Chest"]);
        Assert.Equal(800f, progress.VolumePerMuscle["Back"]);
    }
}
