public class Level
{
    public virtual void CreateLevel(GameGrid grid)
    {

    }
}

public class Level1 : Level
{
    public override void CreateLevel(GameGrid grid)
    {
        base.CreateLevel(grid);

        grid.AddTubWall(new IntVec2())

        for (var i = 0; i < 4; ++i)
        {
            grid.AddFluid(new IntVec2(10 + i, 14), FluidType.Green, 1_000_000);
        }

    }
}