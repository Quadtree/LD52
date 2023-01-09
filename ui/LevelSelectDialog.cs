using System;
using Godot;

public class LevelSelectDialog : PopupDialog
{
    public override void _Ready()
    {
        for (var i = 0; i < Level.Levels.Length; ++i)
        {
            var btn = new Button();
            btn.Text = $"Level {i + 1}";

            btn.Connect("pressed", this, nameof(StartLevelPressed), new Godot.Collections.Array(i));

            this.FindChildByType<VBoxContainer>().AddChild(btn);
        }
    }

    void StartLevelPressed(int lvl)
    {
        Level.CurrentLevel = lvl;
        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
