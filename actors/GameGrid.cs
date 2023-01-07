using System;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    bool Placing = false;

    const int WIDTH = 16;
    const int HEIGHT = 16;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        if (Placing && PlaceableSelected != null)
        {
            var picked = VectorToTile(Picking.PickPointAtCursor(this));
            GD.Print(picked);

            if (PlaceableSelected == Placables.TubWall)
            {

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
            Mathf.RoundToInt(v3.Value.x + (WIDTH / 2)),
            Mathf.RoundToInt(v3.Value.y + (HEIGHT / 2))
        );
    }
}
