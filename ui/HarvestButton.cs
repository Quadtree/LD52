using System;
using Godot;

public class HarvestButton : Button
{
    public override void _Ready()
    {
        Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Harvest;
    }
}
