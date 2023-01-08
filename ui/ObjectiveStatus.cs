using System;
using System.Linq;
using Godot;

public class ObjectiveStatus : GridContainer
{
    bool Initialized = false;

    Plant.EYieldType[] Keys;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();

        if (!Initialized && grid?.Level != null)
        {
            Initialized = true;
            Keys = grid.Level.Requirements.Keys.ToArray();

            foreach (var it in Keys)
            {
                AddChild(UIUtil.Label(it.ToString()));
                AddChild(UIUtil.Label("?/?"));
            }
        }

        if (Initialized) UpdateList();
    }

    void UpdateList()
    {
        var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
        var i = 1;
        foreach (var it in Keys)
        {
            this.GetChild<Label>(i).Text = $"{(grid.Score.ContainsKey(it) ? grid.Score[it] : 0)}/{grid.Level.Requirements[it]}";
            i += 2;
        }
    }
}
