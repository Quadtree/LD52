using System;
using Godot;

public class ItemSelectPanel : VBoxContainer
{
    bool InitDone = false;


    public override void _Process(float delta)
    {
        if (!InitDone)
        {
            var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
            if (grid.Level != null)
            {
                this.FindChildByName<Button>("WallButton").Connect("pressed", this, nameof(WallButtonPressed));
                this.FindChildByName<Button>("PipeButton").Connect("pressed", this, nameof(PipeButtonPressed));
                this.FindChildByName<Button>("PumpButton").Connect("pressed", this, nameof(PumpButtonPressed));
                this.FindChildByName<Button>("OutletButton").Connect("pressed", this, nameof(OutletButtonPressed));
                this.FindChildByName<Button>("FilterButton").Connect("pressed", this, nameof(FilterButtonPressed));

                if (!grid.Level.AllowFilter) this.FindChildByName<Button>("FilterButton").Visible = false;

                InitDone = true;
            }
        }
    }

    /*
    if (@event.IsActionPressed("select_item_0")) { PlaceableSelected = Placables.TubWall; SetPlacementGhost("res://models/block.glb"); }
        if (@event.IsActionPressed("select_item_1")) { PlaceableSelected = Placables.Pipe; SetPlacementGhost("res://models/junction.glb"); }
        if (@event.IsActionPressed("select_item_2")) { PlaceableSelected = Placables.Pump; SetPlacementGhost("res://models/pump_rotor.glb"); }
        if (@event.IsActionPressed("select_item_3")) { PlaceableSelected = Placables.Outlet; SetPlacementGhost("res://models/outlet.glb"); }
    */

    void WallButtonPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.TubWall;
        grid.SetPlacementGhost("res://models/block.glb");
    }

    void PipeButtonPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Pipe;
        grid.SetPlacementGhost("res://models/junction.glb");
    }

    void PumpButtonPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Pump;
        grid.SetPlacementGhost("res://models/pump_rotor.glb");
    }

    void OutletButtonPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Outlet;
        grid.SetPlacementGhost("res://models/outlet.glb");
    }

    void FilterButtonPressed()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Filter;
        grid.SetPlacementGhost("res://models/filter.glb");
    }
}
