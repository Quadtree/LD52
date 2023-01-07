using System;
using Godot;

public class Plant : Spatial
{
    [Export]
    public int RedUsedPerTick;

    [Export]
    public int GreenUsedPerTick;

    [Export]
    public int BlueUsedPerTick;

    [Export]
    public int MaxGrowth;

    public GameGrid Grid;

    public IntVec2 Pos;

    public int Growth;

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        if (Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] >= RedUsedPerTick && Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] >= GreenUsedPerTick && Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] >= BlueUsedPerTick)
        {
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] -= RedUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] -= GreenUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] -= BlueUsedPerTick;
        }
    }

    public void Reposition(GameGrid gg, IntVec2 pos)
    {
        Grid = gg;
        this.SetGlobalLocation(gg.TileToVector(pos));
    }
}
