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
    public string PlantName;

    [Export]
    public string PlantDesc;

    [Export]
    public EYieldType YieldType;

    public bool Picked;

    public GameGrid Grid;

    public IntVec2 Pos;

    public int Growth;

    public bool IsRipe => Growth >= MaxGrowth;

    public string SourceFile;

    public bool IsGhost;

    public bool HasRipened;

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
        if (IsGhost) return;

        if (Pos.y > 0 && Grid.IsTileOpenToFluid(Pos + new IntVec2(0, -1), FluidType.Blue))
        {
            QueueFree();
            return;
        }

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

        if (IsRipe && !HasRipened)
        {
            foreach (var it in this.FindChildrenByType<MeshInstance>())
            {
                var ns = it.GetSurfaceMaterialCount();

                for (var i = 0; i < ns; ++i)
                {
                    var mat = it.GetActiveMaterial(i);
                    if (mat is SpatialMaterial)
                    {
                        var tm = (SpatialMaterial)mat.Duplicate(false);

                        tm.AlbedoColor = new Color(
                            Util.Clamp(tm.AlbedoColor.r + 0.35f, 0f, 1f),
                            Util.Clamp(tm.AlbedoColor.g + 0.35f, 0f, 1f),
                            Util.Clamp(tm.AlbedoColor.b - 0.35f, 0f, 1f),
                            tm.AlbedoColor.a
                        );

                        it.SetSurfaceMaterial(i, tm);
                    }
                }
            }

            HasRipened = true;
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

        GD.Print($"score={string.Join(",", Grid.Score)} / {string.Join(",", Grid.Level.Requirements)}");

        Grid.CheckWinConditions();

        Util.SpawnOneShotSound("res://sounds/harvest1.wav", this, -5);
    }
}
