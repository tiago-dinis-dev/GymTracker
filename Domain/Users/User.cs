using Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Users;

public class User : AggregateRoot
{
    public string Name { get; private set; }
    public float HeightCm { get; private set; }
    public float WeightKg { get; private set; }

    private User() { }

    public User(string name, float heightCm, float weightKg)
    {
        Name = name;
        HeightCm = heightCm;
        WeightKg = weightKg;
    }

    public void UpdateWeight(float newWeightKg)
    {
        if (newWeightKg <= 0)
        {
            throw new ArgumentException("Weight must be greater than zero.", nameof(newWeightKg));
        }

        WeightKg = newWeightKg;
    }

    public float CalculateBMI()
    {
        float heightM = HeightCm / 100;
        return WeightKg / (heightM * heightM);
    }
}
