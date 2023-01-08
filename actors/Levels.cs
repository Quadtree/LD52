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
            ret[Plant.EYieldType.FoodLeaf] = 8;
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

        CreateFluidTub(grid, new IntVec2(5, 1), 4, FluidType.Blue);

        CreateFluidTub(grid, new IntVec2(9, 7), 5, FluidType.Red);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.FoodLeaf] = 10;
            // 15 is possible...
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/FoodLeaf.tscn", "res://actors/plants/MuckRoot.tscn" };

    public override bool AllowFilter => true;

    public override string Description => "We're out of green, but luckily, we've discovered a new plant called Muckroot that consumes red and produces green.";
}

public class Level5 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(5, 1), 4, FluidType.Blue);

        CreateFluidTub(grid, new IntVec2(10, 1), 1, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(9, 7), 5, FluidType.Red);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.BitterLeaf] = 20;
            // 15 is possible...
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/BitterLeaf.tscn", "res://actors/plants/MuckRoot.tscn" };

    public override bool AllowFilter => true;

    public override string Description => "We've managed to obtain a bit more green. Let's make as much Bitterleaf for the new spiced lattes as we can.";
}

public class Level6 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(5, 1), 9, FluidType.Blue);
    }

    public override Dictionary<Plant.EYieldType, int> Requirements
    {
        get
        {
            var ret = new Dictionary<Plant.EYieldType, int>();
            ret[Plant.EYieldType.BitterLeaf] = 20;
            return ret;
        }
    }

    public override string[] AvailablePlantTypes => new string[] { "res://actors/plants/BitterLeaf.tscn", "res://actors/plants/MuckRoot.tscn", "res://actors/plants/GasLeaf.tscn" };

    public override bool AllowFilter => true;

    public override string Description => "We're out of everything! Luckily, we just developed a new plant called Gasleaf that can somehow convert water into red gas. We'll have to use that to create more bitterleaf for the captain's new spicy stew.";
}