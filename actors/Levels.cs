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
}

public class Level1 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        CreateFluidTub(grid, new IntVec2(10, 1), 3, FluidType.Green);

        CreateFluidTub(grid, new IntVec2(6, 1), 3, FluidType.Blue);
    }
}