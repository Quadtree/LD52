using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class FluidNetwork
{
    public GameGrid Grid;
    public IntVec2[] Tiles;
    public IntVec2[] Pumps;
    public IntVec2[] Outlets;

    const int NUM_FLUID_TYPES = 3;

    const int PUMP_RATE = 500;

    public void Update()
    {
        var output = new int[NUM_FLUID_TYPES];

        foreach (var pump in Pumps)
        {
            var pumpedByThisPump = new int[NUM_FLUID_TYPES];
            var totalPumpedByThis = 0;

            for (int fluidType = 0; fluidType < NUM_FLUID_TYPES; ++fluidType)
            {
                var pumpedOfThisType = Math.Min(Grid.Fluid[pump.x, pump.y, fluidType], 500);
                pumpedByThisPump[fluidType] = pumpedOfThisType;
                totalPumpedByThis += pumpedOfThisType;
            }

            var ratioMilis = 1000;

            if (totalPumpedByThis > 500)
            {
                ratioMilis = (PUMP_RATE * 1000) / totalPumpedByThis;
            }

            for (int fluidType = 0; fluidType < NUM_FLUID_TYPES; ++fluidType)
            {
                var actualPumpedAmount = pumpedByThisPump[fluidType] * ratioMilis / 1000;
                Grid.Fluid[pump.x, pump.y, fluidType] -= actualPumpedAmount;
                output[fluidType] += actualPumpedAmount;
            }
        }

        GD.Print($"output={String.Join(",", output)}");
    }
}