using Domain.Workouts;

namespace Domain.Tests.Workouts;

public class SetRecordTests
{
    [Fact]
    public void CalculateVolume_ReturnsRepsTimesWeight()
    {
        var set = new SetRecord(0, 10, 100f, 120m);

        var volume = set.CalculateVolume();

        Assert.Equal(1000f, volume);
    }

    [Fact]
    public void IntensityPercent1Rm_CalculatesCorrectly()
    {
        var set = new SetRecord(0, 10, 80f, 100m);

        Assert.Equal(80.00m, set.IntensityPercent1Rm);
    }
}
