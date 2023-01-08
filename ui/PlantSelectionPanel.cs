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
                }
            }
        }
    }
}
