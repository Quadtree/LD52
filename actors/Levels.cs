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

        grid.AddFluid(new IntVec2(14, 14), FluidType.Green, 1_000_000);
    }
}