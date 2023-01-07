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
    public int RedLiquidProducedPerTick;

    [Export]
    public int GreenLiquidProducedPerTick;

    [Export]
    public int RedGasProducedPerTick;

    [Export]
    public int BlueGasProducedPerTick = -1;

    [Export]
    public int MaxRedTolerancePerMili;

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
        MuckRoot,
    }

    public override void _Ready()
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        var redPartsPerMili = 0;
        if (Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] > 0)
        {
            redPartsPerMili = Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] * 1000 / (Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] + Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] + Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue]);
        }

        if (redPartsPerMili <= MaxRedTolerancePerMili &&
            Growth < MaxGrowth &&
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] >= RedUsedPerTick &&
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] >= GreenUsedPerTick &&
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] >= BlueUsedPerTick &&
            (Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] + Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] + Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue]) < (1_000_000 - RedLiquidProducedPerTick - GreenLiquidProducedPerTick)
        )
        {
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] -= RedUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] -= GreenUsedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Blue] -= BlueUsedPerTick;
            Growth++;

            // water is usually recycled into the air
            if (BlueGasProducedPerTick == -1)
                Grid.GasLevels[(int)FluidType.Blue] += BlueUsedPerTick;
            else
                Grid.GasLevels[(int)FluidType.Blue] += BlueGasProducedPerTick;

            Grid.GasLevels[(int)FluidType.Red] += RedGasProducedPerTick;

            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Red] += RedLiquidProducedPerTick;
            Grid.Fluid[Pos.x, Pos.y, (int)FluidType.Green] += GreenLiquidProducedPerTick;

            Scale = Vector3.One * ((Growth / (float)MaxGrowth) * .75f + .25f);
        }
    }

    public void Reposition(GameGrid gg, IntVec2 pos)
    {
        Grid = gg;
        Pos = pos;
        Scale = Vector3.One * .25f;
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
