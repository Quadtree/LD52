using System;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    bool Placing = false;

    const int WIDTH = 16;
    const int HEIGHT = 16;

    bool[,] TubWalls = new bool[WIDTH, HEIGHT];

    // a full square has a value of 1_000_000_000
    int[,,] Fluid = new int[WIDTH, HEIGHT, 3];

    public override void _Ready()
    {
        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        tw.Multimesh.InstanceCount = WIDTH * HEIGHT;
        tw.Multimesh.VisibleInstanceCount = 0;
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

        Fluid[8, 16, 0] += 100_000;
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

    private void AddTubWall(IntVec2 pos)
    {
        GD.Print($"Adding tub wall at {pos}");

        TubWalls[pos.x, pos.y] = true;

        var tw = this.FindChildByName<MultiMeshInstance>("TubWalls");
        var nextInstanceId = tw.Multimesh.VisibleInstanceCount;
        tw.Multimesh.VisibleInstanceCount++;

        tw.Multimesh.SetInstanceTransform(nextInstanceId, new Transform(Quat.Identity, TileToVector(pos)));
    }
}
