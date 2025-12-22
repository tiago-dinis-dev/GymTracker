using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Workouts;

public class SetRecord
{
    public int Reps { get; private set; }
    public float Weight { get; private set; }

    private SetRecord() { }
    public SetRecord(int reps, float weight)
    {
        Reps = reps;
        Weight = weight;
    }

    public float CalculateVolume()
    {
        return Reps * Weight;
    }
}
