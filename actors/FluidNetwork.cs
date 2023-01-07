using System.Collections.Generic;

public class FluidNetwork
{
    public GameGrid Grid;
    public IntVec2[] Tiles;
    public IntVec2[] Pumps;
    public IntVec2[] Outlets;

    const int NUM_FLUID_TYPES = 3;

    public void Update()
    {
        for (var pump in Pumps)
        {
            for (int fluidType = 0; fluidType < NUM_FLUID_TYPES; ++fluidType)
            {

            }
        }

    }
}