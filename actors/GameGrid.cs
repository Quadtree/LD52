using System;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    bool Placing = false;

    const int WIDTH = 16;
    const int HEIGHT = 16;

    bool[,] TubWalls = new bool[WIDTH, HEIGHT];

    bool[,] LiquidFell = new bool[WIDTH, HEIGHT];

    // a full square has a value of 1_000_000
    int[,,] Fluid = new int[WIDTH, HEIGHT, 3];

    bool[,] Pipe = new bool[WIDTH, HEIGHT];
    bool[,] Pump = new bool[WIDTH, HEIGHT];
    bool[,] Outlet = new bool[WIDTH, HEIGHT];

    Level Level;

    public override void _Ready()
    {
        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        tw.Multimesh.InstanceCount = WIDTH * HEIGHT;
        tw.Multimesh.VisibleInstanceCount = 0;


        var lq = this.FindChildByName<MultiMeshInstance>("Liquid");
        lq.Multimesh.InstanceCount = WIDTH * HEIGHT;
        lq.Multimesh.VisibleInstanceCount = -1;

        Level = new Level1();

        Level.CreateLevel(this);
    }

    public override void _Process(float delta)
    {
        if (Placing && PlaceableSelected != null)
        {
            var picked = VectorToTile(Picking.PickPointAtCursor(this));
            //GD.Print(picked);

            if (picked != null)
            {
                if (PlaceableSelected == Placables.TubWall)
                {
                    if (!TubWalls[picked.Value.x, picked.Value.y])
                    {
                        AddTubWall(picked.Value);
                    }
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        //Fluid[8, 15, (int)FluidType.Blue] += 10_000;

        for (var y = 0; y < HEIGHT; ++y)
        {
            for (var x = 0; x < WIDTH; ++x)
            {
                LiquidFell[x, y] = false;

                for (var f = 0; f < 3; ++f)
                {
                    if (Fluid[x, y, f] > 0 && y > 0 && IsTileOpenToFluid(new IntVec2(x, y - 1), (FluidType)f))
                    {
                        if (MoveFluidBetween(new IntVec2(x, y), new IntVec2(x, y - 1), (FluidType)f, Math.Min(Fluid[x, y, f] / 10 + 500, Fluid[x, y, f])) > 0)
                        {
                            LiquidFell[x, y] = true;
                        }
                    }
                }
            }

            for (var x = 0; x < WIDTH; ++x)
            {
                for (var f = 0; f < 3; ++f)
                {
                    if (!LiquidFell[x, y] && Fluid[x, y, f] > 0 && x > 0 && IsTileOpenToFluid(new IntVec2(x, y), (FluidType)f) && IsTileOpenToFluid(new IntVec2(x - 1, y), (FluidType)f))
                    {
                        var toFlow = (Fluid[x, y, f] - Fluid[x - 1, y, f]) / 10;

                        MoveFluidBetween(new IntVec2(x, y), new IntVec2(x - 1, y), (FluidType)f, toFlow);
                    }
                }
            }

            for (var x = WIDTH - 1; x >= 0; --x)
            {
                for (var f = 0; f < 3; ++f)
                {
                    if (!LiquidFell[x, y] && Fluid[x, y, f] > 0 && x < 15 && IsTileOpenToFluid(new IntVec2(x, y), (FluidType)f) && IsTileOpenToFluid(new IntVec2(x + 1, y), (FluidType)f))
                    {
                        var toFlow = (Fluid[x, y, f] - Fluid[x + 1, y, f]) / 10;

                        MoveFluidBetween(new IntVec2(x, y), new IntVec2(x + 1, y), (FluidType)f, toFlow);
                    }
                }
            }
        }

        var lq = this.FindChildByName<MultiMeshInstance>("Liquid");

        for (var x = 0; x < WIDTH; ++x)
        {
            for (var y = 0; y < HEIGHT; ++y)
            {
                var totalFluid = Fluid[x, y, 0] + Fluid[x, y, 1] + Fluid[x, y, 2];

                if (totalFluid > 0)
                {
                    Transform transform;

                    if (!LiquidFell[x, y])
                    {
                        transform = new Transform(new Basis(
                            new Vector3(1, 0, 0),
                            new Vector3(0, totalFluid / 1_000_000f, 0),
                            new Vector3(0, 0, 1)
                        ), TileToVector(new IntVec2(x, y)) + new Vector3(0, -0.5f + (totalFluid / 1_000_000f) / 2, 0));
                    }
                    else
                    {
                        transform = new Transform(new Basis(
                            new Vector3(totalFluid / 1_000_000f, 0, 0),
                            new Vector3(0, 1, 0),
                            new Vector3(0, 0, 1)
                        ), TileToVector(new IntVec2(x, y)) + new Vector3(0, -1, 0));
                    }

                    lq.Multimesh.SetInstanceTransform(GetLiquidInstanceId(new IntVec2(x, y)), transform);

                    lq.Multimesh.SetInstanceColor(GetLiquidInstanceId(new IntVec2(x, y)), new Color(Fluid[x, y, (int)FluidType.Red] / (float)totalFluid, Fluid[x, y, (int)FluidType.Green] / (float)totalFluid, Fluid[x, y, (int)FluidType.Blue] / (float)totalFluid, 0.5f));
                }
                else
                {
                    lq.Multimesh.SetInstanceColor(GetLiquidInstanceId(new IntVec2(x, y)), new Color(1, 1, 1, 0));
                }
            }
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("place_item"))
        {
            Placing = true;
        }

        if (@event.IsActionReleased("place_item"))
        {
            Placing = false;
        }

        if (@event.IsActionPressed("select_item_0"))
        {
            PlaceableSelected = Placables.TubWall;
        }
    }

    public IntVec2? VectorToTile(Vector3? v3)
    {
        if (v3 == null) return null;

        return new IntVec2(
            Mathf.RoundToInt(v3.Value.x + (WIDTH / 2) - 0.5f),
            Mathf.RoundToInt(v3.Value.y + (HEIGHT / 2) - 0.5f)
        );
    }

    public Vector3 TileToVector(IntVec2 tile)
    {
        return new Vector3(
            tile.x - (WIDTH / 2) + 0.5f,
            tile.y - (HEIGHT / 2) + 0.5f,
            0
        );
    }

    public void AddTubWall(IntVec2 pos)
    {
        GD.Print($"Adding tub wall at {pos}");

        TubWalls[pos.x, pos.y] = true;

        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        var nextInstanceId = tw.Multimesh.VisibleInstanceCount;
        tw.Multimesh.VisibleInstanceCount++;

        tw.Multimesh.SetInstanceTransform(nextInstanceId, new Transform(Quat.Identity, TileToVector(pos)));
    }

    public void AddPipe(IntVec2 pos)
    {
        GD.Print($"Adding pipe wall at {pos}");

        TubWalls[pos.x, pos.y] = true;

        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        var nextInstanceId = tw.Multimesh.VisibleInstanceCount;
        tw.Multimesh.VisibleInstanceCount++;

        tw.Multimesh.SetInstanceTransform(nextInstanceId, new Transform(Quat.Identity, TileToVector(pos)));
    }

    public void AddFluid(IntVec2 pos, FluidType type, int amt)
    {
        Fluid[pos.x, pos.y, (int)type] += amt;

        AT.LessThan(Fluid[pos.x, pos.y, (int)type], 1_000_001);
    }

    private bool IsTileOpenToFluid(IntVec2 tile, FluidType fluid)
    {
        if (!TubWalls[tile.x, tile.y]) return true;

        return false;
    }

    private int GetLiquidInstanceId(IntVec2 tile)
    {
        return tile.x + tile.y * WIDTH;
    }

    private int MoveFluidBetween(IntVec2 from, IntVec2 to, FluidType type, int amt)
    {
        Fluid[to.x, to.y, (int)type] += amt;
        Fluid[from.x, from.y, (int)type] -= amt;
        var overflow = 0;

        if (Fluid[to.x, to.y, (int)type] > 1_000_000)
        {
            overflow = Fluid[to.x, to.y, (int)type] - 1_000_000;
            Fluid[from.x, from.y, (int)type] += overflow;
            Fluid[to.x, to.y, (int)type] -= overflow;
        }

        AT.True(Fluid[to.x, to.y, (int)type] >= 0);
        AT.True(Fluid[to.x, to.y, (int)type] <= 1_000_000);
        AT.True(Fluid[from.x, from.y, (int)type] >= 0);
        AT.True(Fluid[from.x, from.y, (int)type] <= 1_000_000);

        return amt - overflow;
    }
}
