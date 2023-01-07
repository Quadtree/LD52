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

    public bool Picked;

    public GameGrid Grid;

    public IntVec2 Pos;

    public int Growth;

    public bool IsRipe => Growth >= MaxGrowth;

    public enum EYieldType
    {
        FoodLeaf,
        BitterLeaf,
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

            // water is always recycled into the air
            Grid.GasLevels[(int)FluidType.Blue] += BlueUsedPerTick;

            Scale = Vector3.One * ((Growth / (float)MaxGrowth) * .75f + .25f);
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
        if (Picked) return;

        Picked = true;
        QueueFree();

        if (!Grid.Score.ContainsKey(YieldType)) Grid.Score[YieldType] = 0;

        Grid.Score[YieldType]++;

        GD.Print($"score={string.Join(",", Grid.Score)}");
    }
}
