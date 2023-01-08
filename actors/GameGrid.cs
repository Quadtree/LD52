using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    bool Placing = false;
    bool Destroying = false;

    const int WIDTH = 16;
    const int HEIGHT = 16;

    bool[,] TubWalls = new bool[WIDTH, HEIGHT];
    bool[,] FilterWalls = new bool[WIDTH, HEIGHT];

    bool[,] LiquidFell = new bool[WIDTH, HEIGHT];

    // a full square has a value of 1_000_000
    public int[,,] Fluid = new int[WIDTH, HEIGHT, 3];

    bool[,] Pipe = new bool[WIDTH, HEIGHT];
    bool[,] Pump = new bool[WIDTH, HEIGHT];
    bool[,] Outlet = new bool[WIDTH, HEIGHT];

    int[,] PumpRotorToInstanceIdMapping = new int[WIDTH, HEIGHT];
    float[,] PumpRotorRotation = new float[WIDTH, HEIGHT];

    public int[] GasLevels = new int[FluidNetwork.NUM_FLUID_TYPES];

    public Level Level;

    public List<FluidNetwork> FluidNetworks = new List<FluidNetwork>();

    public Dictionary<Plant.EYieldType, int> Score = new Dictionary<Plant.EYieldType, int>();

    bool WinDialogDisplayed = false;

    public float TimeElapsed = 0;

    public bool Running = true;

    public Spatial PlacementGhost;

    public override void _Ready()
    {
        foreach (var it in this.FindChildrenByType<MultiMeshInstance>())
        {
            var meshInstanceChildren = it.FindChildrenByType<MeshInstance>().ToArray();
            if (meshInstanceChildren.Length == 1)
            {
                it.Multimesh.Mesh = meshInstanceChildren[0].Mesh;
                GD.Print($"Setting {it}'s mesh to {meshInstanceChildren[0].Mesh}");
            }
            else
            {
                GD.Print($"{it} has no valid mesh source (len={meshInstanceChildren.Length})");
            }
        }

        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        tw.Multimesh.InstanceCount = WIDTH * HEIGHT;
        tw.Multimesh.VisibleInstanceCount = 0;


        var lq = this.FindChildByName<MultiMeshInstance>("Liquid");
        lq.Multimesh.InstanceCount = WIDTH * HEIGHT;
        lq.Multimesh.VisibleInstanceCount = -1;

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("PipeCenters");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("Pumps");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("PumpStators");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("Outlets");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("FilterWalls");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        {
            var mmi = this.FindChildByName<MultiMeshInstance>("Pipes");
            mmi.Multimesh.InstanceCount = WIDTH * HEIGHT;
            mmi.Multimesh.VisibleInstanceCount = 0;
        }

        var backWall = this.FindChildByName<MultiMeshInstance>("BackWall");
        backWall.Multimesh.InstanceCount = (WIDTH + 2) * (HEIGHT + 2);

        for (var y = 0; y < HEIGHT + 2; ++y)
        {
            for (var x = 0; x < WIDTH + 2; ++x)
            {
                if ((x == 0 && y == 0) || (x == 0 && y == HEIGHT + 1) || (x == WIDTH + 1 && y == HEIGHT + 1) || (x == WIDTH + 1 && y == 0))
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(Quat.Identity, new Vector3(-1000, -1000, 0)));
                }
                else if (x == 0)
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(new Quat(new Vector3(0, Mathf.Pi / 2, 0)), TileToVector(new IntVec2(x - 1, y - 1)) + new Vector3(.5f, 0.0f, 0)));
                }
                else if (x == WIDTH + 1)
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(new Quat(new Vector3(0, Mathf.Pi / 2, 0)), TileToVector(new IntVec2(x - 1, y - 1)) + new Vector3(-.5f, 0.0f, 0)));
                }
                else if (y == 0)
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(new Quat(new Vector3(Mathf.Pi / 2, Mathf.Pi / 2, 0)), TileToVector(new IntVec2(x - 1, y - 1)) + new Vector3(0.0f, 0.5f, 0)));
                }
                else if (y == HEIGHT + 1)
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(new Quat(new Vector3(Mathf.Pi / 2, Mathf.Pi / 2, 0)), TileToVector(new IntVec2(x - 1, y - 1)) + new Vector3(0.0f, -0.5f, 0)));
                }
                else
                {
                    backWall.Multimesh.SetInstanceTransform(x + y * (WIDTH + 2), new Transform(Quat.Identity, TileToVector(new IntVec2(x - 1, y - 1)) + new Vector3(0.0f, 0.0f, -.5f)));
                }
            }
        }

        Level = Level.Levels[Level.CurrentLevel];

        Level.CreateLevel(this);
    }

    public override void _Process(float delta)
    {
        if (PlacementGhost != null)
        {
            var picked = VectorToTile(Picking.PickPointAtCursor(this));

            if (picked != null)
            {
                PlacementGhost.SetGlobalLocation(TileToVector(picked.Value));
                PlacementGhost.Visible = true;
            }
            else
            {
                PlacementGhost.Visible = false;
            }
        }

        if (Placing && PlaceableSelected == Placables.Harvest)
        {
            var picked = Picking.PickObjectAtCursor(this);
            if (picked != null && picked.GetParent() is Plant && picked.GetParent<Plant>().IsRipe)
            {
                GD.Print($"Picking plant {picked}");
                picked.GetParent<Plant>().Pick();
            }
        }
        else
        {
            if (Placing && PlaceableSelected != null)
            {
                var picked = VectorToTile(Picking.PickPointAtCursor(this));
                //GD.Print(picked);

                if (picked != null && IsInBounds(picked.Value))
                {
                    if (PlaceableSelected == Placables.TubWall && !TubWalls[picked.Value.x, picked.Value.y]) AddTubWall(picked.Value);
                    if (PlaceableSelected == Placables.Pipe && !Pipe[picked.Value.x, picked.Value.y]) AddPipe(picked.Value);
                    if (PlaceableSelected == Placables.Pump && !Pump[picked.Value.x, picked.Value.y]) AddPump(picked.Value);
                    if (PlaceableSelected == Placables.Outlet && !Outlet[picked.Value.x, picked.Value.y]) AddOutlet(picked.Value);
                    if (PlaceableSelected == Placables.Filter && !FilterWalls[picked.Value.x, picked.Value.y]) AddFilterWall(picked.Value);
                    if (PlaceableSelected == Placables.Plant0) PlacePlantAt(picked.Value, Level.AvailablePlantTypes[0]);
                    if (PlaceableSelected == Placables.Plant1) PlacePlantAt(picked.Value, Level.AvailablePlantTypes[1]);
                    if (PlaceableSelected == Placables.Plant2) PlacePlantAt(picked.Value, Level.AvailablePlantTypes[2]);
                }
            }

            if (Destroying)
            {
                var picked = VectorToTile(Picking.PickPointAtCursor(this));

                if (picked != null)
                {
                    DeleteAll(picked.Value);
                }
            }
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        base._PhysicsProcess(delta);

        if (!Running) return;

        TimeElapsed += delta;

        foreach (var fn in FluidNetworks) fn.Update();

        for (var i = 0; i < FluidNetwork.NUM_FLUID_TYPES; ++i)
        {
            var condensed = GasLevels[i] / 1000;

            for (var x = 0; x < WIDTH; ++x)
            {
                Fluid[x, 0, i] += condensed;
                GasLevels[i] -= condensed;
            }

            AT.True(GasLevels[i] >= 0);
        }

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

                    lq.Multimesh.SetInstanceColor(GetLiquidInstanceId(new IntVec2(x, y)), new Color(
                        (Fluid[x, y, (int)FluidType.Red] * 10) / (float)totalFluid,
                        Fluid[x, y, (int)FluidType.Green] / (float)totalFluid,
                        Fluid[x, y, (int)FluidType.Blue] / (float)totalFluid,
                    0.5f));
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
            SetPlacementGhost("res://models/block.glb");
        }
        if (@event.IsActionPressed("select_item_1")) PlaceableSelected = Placables.Pipe;
        if (@event.IsActionPressed("select_item_2")) PlaceableSelected = Placables.Pump;
        if (@event.IsActionPressed("select_item_3")) PlaceableSelected = Placables.Outlet;
        if (@event.IsActionPressed("select_item_4")) PlaceableSelected = Placables.Plant0;
        if (@event.IsActionPressed("select_item_5")) PlaceableSelected = Placables.Plant1;
        if (@event.IsActionPressed("select_item_6")) PlaceableSelected = Placables.Plant2;
        if (@event.IsActionPressed("select_filter") && Level.AllowFilter) PlaceableSelected = Placables.Filter;
        if (@event.IsActionPressed("select_harvest")) PlaceableSelected = Placables.Harvest;

        if (PlaceableSelected == Placables.TubWall)
        {

        }

        if (@event.IsActionPressed("cheat_instantly_meet_requirements") && OS.IsDebugBuild())
        {
            Score = Level.Requirements.ToArray().ToDictionary(it => it.Key, it => it.Value);
            CheckWinConditions();
        }

        if (@event.IsActionPressed("deselect_or_destroy"))
        {
            if (PlaceableSelected != null)
            {
                PlaceableSelected = null;
                PlacementGhost.QueueFree();
                PlacementGhost = null;
            }
            else
            {
                Destroying = true;
            }
        }

        if (@event.IsActionReleased("deselect_or_destroy")) Destroying = false;
    }

    private void SetPlacementGhost(string srcName)
    {
        if (PlacementGhost != null) PlacementGhost.QueueFree();

        PlacementGhost = GD.Load<PackedScene>(srcName).Instance<Spatial>();
        GetTree().CurrentScene.AddChild(PlacementGhost);


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
            0.5f
        );
    }

    public void AddTubWall(IntVec2 pos)
    {
        GD.Print($"Adding tub wall at {pos}");

        DeleteAll(pos);

        TubWalls[pos.x, pos.y] = true;

        AddToMultimesh("TubWalls", TileToVector(pos));
    }

    public void AddFilterWall(IntVec2 pos)
    {
        GD.Print($"Adding filter wall at {pos}");

        DeleteAll(pos);

        FilterWalls[pos.x, pos.y] = true;

        AddToMultimesh("FilterWalls", TileToVector(pos));
    }

    public void AddPipe(IntVec2 pos)
    {
        GD.Print($"Adding pipe at {pos}");

        AT.True(!Pipe[pos.x, pos.y]);

        DeleteAll(pos);

        Pipe[pos.x, pos.y] = true;

        AddToMultimesh("PipeCenters", TileToVector(pos));

        RecomputeFluidNetworks();
    }

    public void AddPump(IntVec2 pos)
    {
        GD.Print($"Adding pump at {pos}");

        DeleteAll(pos);

        AT.True(!Pump[pos.x, pos.y]);

        Pump[pos.x, pos.y] = true;

        PumpRotorToInstanceIdMapping[pos.x, pos.y] = AddToMultimesh("Pumps", TileToVector(pos));
        AddToMultimesh("PumpStators", TileToVector(pos));

        RecomputeFluidNetworks();
    }

    public void AddOutlet(IntVec2 pos)
    {
        GD.Print($"Adding outlet at {pos}");

        DeleteAll(pos);

        AT.True(!Outlet[pos.x, pos.y]);

        Outlet[pos.x, pos.y] = true;

        AddToMultimesh("Outlets", TileToVector(pos));

        RecomputeFluidNetworks();
    }

    public void DeletePipe(IntVec2 pos)
    {
        if (Pipe[pos.x, pos.y])
        {
            Pipe[pos.x, pos.y] = false;
            RemoveFromMultimesh("PipeCenters", TileToVector(pos));
            RecomputeFluidNetworks();
        }
    }

    public void DeletePump(IntVec2 pos)
    {
        if (Pump[pos.x, pos.y])
        {
            Pump[pos.x, pos.y] = false;
            RemoveFromMultimesh("Pumps", TileToVector(pos));
            RemoveFromMultimesh("PumpStators", TileToVector(pos));
            RecomputeFluidNetworks();
        }
    }

    public void DeleteOutlet(IntVec2 pos)
    {
        if (Outlet[pos.x, pos.y])
        {
            Outlet[pos.x, pos.y] = false;
            RemoveFromMultimesh("Outlets", TileToVector(pos));
            RecomputeFluidNetworks();
        }
    }

    public void DeleteWall(IntVec2 pos)
    {
        if (TubWalls[pos.x, pos.y])
        {
            TubWalls[pos.x, pos.y] = false;
            RemoveFromMultimesh("TubWalls", TileToVector(pos));
        }
    }

    public void DeleteFilter(IntVec2 pos)
    {
        if (FilterWalls[pos.x, pos.y])
        {
            FilterWalls[pos.x, pos.y] = false;
            RemoveFromMultimesh("FilterWalls", TileToVector(pos));
        }
    }

    public void DeleteAll(IntVec2 pos)
    {
        GetTree().CurrentScene.FindChildByPredicate<Plant>(it => it.Pos == pos)?.QueueFree();

        DeletePipe(pos);
        DeletePump(pos);
        DeleteOutlet(pos);
        DeleteWall(pos);
        DeleteFilter(pos);
    }

    private int AddToMultimesh(string subName, Vector3 pos)
    {
        var tw = this.FindChildByName<MultiMeshInstance>(subName);
        var nextInstanceId = tw.Multimesh.VisibleInstanceCount;
        tw.Multimesh.VisibleInstanceCount++;

        tw.Multimesh.SetInstanceTransform(nextInstanceId, new Transform(Quat.Identity, pos));

        return nextInstanceId;
    }

    private void RemoveFromMultimesh(string subName, Vector3 pos)
    {
        var mm = this.FindChildByName<MultiMeshInstance>(subName).Multimesh;

        var foundIt = false;

        for (var i = 0; i < mm.VisibleInstanceCount; ++i)
        {
            if (mm.GetInstanceTransform(i).origin.DistanceSquaredTo(pos) < 0.1f)
            {
                mm.VisibleInstanceCount--;
                foundIt = true;
            }

            if (foundIt)
            {
                mm.SetInstanceTransform(i, mm.GetInstanceTransform(i + 1));
            }
        }

        if (!foundIt) GD.PushWarning($"Not able to find sub mesh from {subName} to delete at {pos}");
    }

    public void AddFluid(IntVec2 pos, FluidType type, int amt)
    {
        Fluid[pos.x, pos.y, (int)type] += amt;

        AT.LessThan(Fluid[pos.x, pos.y, (int)type], 1_000_001);
    }

    private bool IsTileOpenToFluid(IntVec2 tile, FluidType fluid)
    {
        if (TubWalls[tile.x, tile.y]) return false;

        if (FilterWalls[tile.x, tile.y] && fluid != FluidType.Red) return false;

        return true;
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

    private bool IsPartOfFluidNetwork(IntVec2 pos)
    {
        return Pipe[pos.x, pos.y] || Pump[pos.x, pos.y] || Outlet[pos.x, pos.y];
    }

    private void RecomputeFluidNetworks()
    {
        var pipeMMI = this.FindChildByName<MultiMeshInstance>("Pipes").Multimesh;
        pipeMMI.VisibleInstanceCount = 0;

        FluidNetworks = new List<FluidNetwork>();

        var claimed = new bool[WIDTH, HEIGHT];

        var deltas = new IntVec2[]{
            new IntVec2(1, 0),
            new IntVec2(-1, 0),
            new IntVec2(0, 1),
            new IntVec2(0, -1),
        };

        for (var x = 0; x < WIDTH; ++x)
        {
            for (var y = 0; y < WIDTH; ++y)
            {
                if (IsPartOfFluidNetwork(new IntVec2(x, y)) && !claimed[x, y])
                {
                    var open = new HashSet<IntVec2>();
                    var closed = new HashSet<IntVec2>();

                    open.Add(new IntVec2(x, y));

                    while (open.Count > 0)
                    {
                        var next = open.First();
                        open.Remove(next);
                        closed.Add(next);
                        claimed[next.x, next.y] = true;

                        foreach (var delta in deltas)
                        {
                            var np = next + delta;

                            if (IsInBounds(np) && IsPartOfFluidNetwork(np) && !open.Contains(np) && !closed.Contains(np))
                            {
                                if (next.y != np.y)
                                {
                                    pipeMMI.SetInstanceTransform(pipeMMI.VisibleInstanceCount, new Transform(Quat.Identity, (TileToVector(next) + TileToVector(np)) / 2));
                                }
                                else
                                {
                                    pipeMMI.SetInstanceTransform(pipeMMI.VisibleInstanceCount, new Transform(new Quat(new Vector3(0, 0, Mathf.Pi / 2)), (TileToVector(next) + TileToVector(np)) / 2));
                                }

                                pipeMMI.VisibleInstanceCount++;
                                open.Add(np);
                            }
                        }
                    }

                    FluidNetworks.Add(new FluidNetwork
                    {
                        Grid = this,
                        Outlets = closed.Where(it => Outlet[it.x, it.y]).ToArray(),
                        Pumps = closed.Where(it => Pump[it.x, it.y]).ToArray(),
                        Tiles = closed.ToArray(),
                    });
                }
            }
        }

        GD.Print($"Discovered {FluidNetworks.Count} fluid networks");

        foreach (var fn in FluidNetworks)
        {
            GD.Print($"FN outlets={String.Join(",", fn.Outlets)}");
        }
    }

    public bool IsInBounds(IntVec2 pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < WIDTH && pos.y < HEIGHT;
    }

    public bool PlacePlantAt(IntVec2 pos, string plant)
    {
        var vecPos = TileToVector(pos);

        if (GetTree().CurrentScene.FindChildByPredicate<Plant>(it => it.Pos == pos && it.SourceFile == plant) != null) return false;

        DeleteAll(pos);

        GD.Print($"Placing plant {plant} at {pos}");

        var plantInstance = GD.Load<PackedScene>(plant).Instance<Plant>();
        GetTree().CurrentScene.AddChild(plantInstance);
        plantInstance.Reposition(this, pos);
        plantInstance.SourceFile = plant;

        return true;
    }

    public void CheckWinConditions()
    {
        if (WinDialogDisplayed) return;

        foreach (var cond in Level.Requirements)
        {
            if (!Score.ContainsKey(cond.Key) || Score[cond.Key] < cond.Value)
            {
                return;
            }
        }

        // if we get this far we've won
        WinDialogDisplayed = true;

        var dialog = GetTree().CurrentScene.FindChildByName<WinLevelDialog>("WinLevelDialog");

        dialog.PopupCentered();
        dialog.AddScoreAndUpdate(Level.GetType().ToString(), TimeElapsed);
    }

    public void RotatePump(IntVec2 pos, float rads)
    {
        var instId = PumpRotorToInstanceIdMapping[pos.x, pos.y];
        PumpRotorRotation[pos.x, pos.y] += rads;

        var mmi = this.FindChildByName<MultiMeshInstance>("Pumps");

        mmi.Multimesh.SetInstanceTransform(instId, new Transform(new Quat(new Vector3(0, 0, PumpRotorRotation[pos.x, pos.y])), TileToVector(pos)));
    }
}
