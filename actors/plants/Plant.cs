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

    [Export]
    public EYieldType YieldType;

    public GameGrid Grid;

    public IntVec2 Pos;

    public int Growth;

    public bool IsRipe => Growth >= MaxGrowth;

    public enum EYieldType
    {
        FoodLeaf
    }

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        if (Growth < MaxGrowth && Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] >= RedUsedPerTick && Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] >= GreenUsedPerTick && Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] >= BlueUsedPerTick)
        {
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] -= RedUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] -= GreenUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] -= BlueUsedPerTick;
            Growth++;

            Scale = Vector3.One * (Growth / (float)MaxGrowth);
        }
    }

    public void Reposition(GameGrid gg, IntVec2 pos)
    {
        Grid = gg;
        Pos = pos;
        Scale = Vector3.Zero;
        this.SetGlobalLocation(gg.TileToVector(pos));
    }

    public void Pick()
    {

    }
}
