using System;
using Godot;

public class PlantSelectionPanel : VBoxContainer
{
    bool InitDone = false;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!InitDone)
        {
            var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
            if (grid.Level != null)
            {
                for (var i = 0; i < 3; ++i)
                {
                    if (grid.Level.AvailablePlantTypes.Length <= i)
                    {
                        this.FindChildByName<Button>($"Plant{i}").Visible = false;
                    }
                    else
                    {
                        this.FindChildByName<Button>($"Plant{i}").Text = GD.Load<PackedScene>(grid.Level.AvailablePlantTypes[i]).Instance<Plant>().PlantName;

                        this.FindChildByName<Button>($"Plant{i}").Connect("pressed", this, $"Pressed{i}");


                    }
                }
            }
        }
    }

    void Pressed0()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Plant0;
        grid.SetPlacementGhostForPlant(0);
    }

    void Pressed1()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Plant1;
        grid.SetPlacementGhostForPlant(1);
    }

    void Pressed2()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        grid.PlaceableSelected = Placables.Plant2;
        grid.SetPlacementGhostForPlant(2);
    }
}
