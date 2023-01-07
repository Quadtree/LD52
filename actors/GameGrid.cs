using System;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    bool Placing = false;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        if (Placing && PlaceableSelected != null)
        {
            if (PlaceableSelected == Placables.TubWall)
            {
                Picking.PickPointAtCursor()
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
}
