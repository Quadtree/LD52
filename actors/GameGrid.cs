using System;
using Godot;

public class GameGrid : Spatial
{
    Placables? PlaceableSelected;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {

    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);

        if (@event.IsActionPressed("select_item_0"))
        {
            PlaceableSelected = Placables.TubWall;
        }
    }
}
