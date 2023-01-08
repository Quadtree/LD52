using System;
using Godot;

public class ElapsedTime : Label
{
    public override void _Process(float delta)
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();

        if (grid != null)
        {
            Text = $"Time: {grid.TimeElapsed:n1}s";
        }
    }
}
