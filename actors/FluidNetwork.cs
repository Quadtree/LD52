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

    public const int NUM_FLUID_TYPES = 3;

    const int PUMP_RATE = 500;

    public void Update()
    {
        foreach (var pump in Pumps)
        {
            var outlet = Util.Choice(Outlets.Select(it => (IntVec2?)it).Where(it => Grid.Fluid[it.Value.x, it.Value.y, (int)FluidType.Red] + Grid.Fluid[it.Value.x, it.Value.y, (int)FluidType.Green] + Grid.Fluid[it.Value.x, it.Value.y, (int)FluidType.Blue] < 500_000));
            if (outlet == null)
            {
                //GD.Print("No outlet");
                return;
            }

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

            float rot = 0;

            for (int fluidType = 0; fluidType < NUM_FLUID_TYPES; ++fluidType)
            {
                var actualPumpedAmount = pumpedByThisPump[fluidType] * ratioMilis / 1000;
                Grid.Fluid[pump.x, pump.y, fluidType] -= actualPumpedAmount;

                Grid.Fluid[outlet.Value.x, outlet.Value.y, fluidType] += actualPumpedAmount;

                rot += actualPumpedAmount / 4000;

                //output[fluidType] += actualPumpedAmount;
            }

            Grid.RotatePump(pump, rot);
        }

        //GD.Print($"output={String.Join(",", output)}");
    }
}