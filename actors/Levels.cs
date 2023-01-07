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
    public virtual string Description => "???";
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

// you get lots of water, but there's no way to stop it from getting contaminated
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

    public override string Description => "The captain wants some bitterleaf to spice the new pie his chef is working on. Unfortunately, bittlerleaf produces poisonous red, so if left unchecked it will stop growing due to the poison. We'll need to flush the trays regularly.";
}

public class Level3 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(10, 1), 3, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(6, 1), 2, FluidType.Blue);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.FoodLeaf] = 5;
            ret[Plant.EYieldType.BitterLeaf] = 10;
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/FoodLeaf.tscn", "res://actors/plants/BitterLeaf.tscn" };

    public override bool AllowFilter => true;

    public override string Description => "All this flushing is using too much water! Fortunately, we've discovered a new block that only admits red. It should allow us to clean the water.";
}

public class Level4 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(10, 1), 1, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(5, 1), 4, FluidType.Blue);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.FoodLeaf] = 20;
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/FoodLeaf.tscn", "res://actors/plants/MuckRoot.tscn" };

    public override bool AllowFilter => true;

    public override string Description => "Now we're running short on green. Luckily, we've discovered a new plant called Muckroot that consumes red and produces green.";
}