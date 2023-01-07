using System.Collections.Generic;

public class Level
{
    public virtual void CreateLevel(GameGrid grid)
    {

    }

    public void CreateFluidTub(GameGrid grid, IntVec2 bottomLeft, int width, FluidType type)
    {
        grid.AddTubWall(bottomLeft);
        grid.AddTubWall(bottomLeft + new IntVec2(0, 1));

        for (var i = 0; i < width; ++i)
        {
            grid.AddFluid(bottomLeft + new IntVec2(i + 1, 1), type, 1_000_000);
            grid.AddTubWall(bottomLeft + new IntVec2(i + 1, 0));
        }

        grid.AddTubWall(bottomLeft + new IntVec2(width + 1, 0));
        grid.AddTubWall(bottomLeft + new IntVec2(width + 1, 1));
    }

    public virtual Dictionary<Plant.EYieldType, int> Requirements { get; }
    public virtual string[] AvailablePlantTypes { get; }
    public virtual bool AllowFilter => false;
}

public class Level1 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(10, 1), 3, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(6, 1), 3, FluidType.Blue);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.FoodLeaf] = 15;
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/FoodLeaf.tscn" };
}

public class Level2 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(10, 1), 3, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(1, 1), 7, FluidType.Blue);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.BitterLeaf] = 5;
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/BitterLeaf.tscn" };
}