using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Exercises;

public class Exercise : AggregateRoot
{
    public string Name { get; private set; }
    public MuscleGroup MuscleGroup { get; private set; }

    private Exercise() { }

    public Exercise(string name, MuscleGroup muscleGroup)
    {
        Name = name;
        MuscleGroup = muscleGroup;
    }
}
