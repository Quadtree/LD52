using System;
using Godot;

public class TitleScreen : Control
{
    public override void _Ready()
    {
        this.FindChildByName<Button>("StartButton").Connect("pressed", this, nameof(StartButtonPressed));
        this.FindChildByName<Button>("LevelSelectButton").Connect("pressed", this, nameof(LevelSelectButtonPressed));
    }

    void StartButtonPressed()
    {
        Level.CurrentLevel = 0;
        GetTree().ChangeScene("res://maps/Default.tscn");
    }

    void LevelSelectButtonPressed()
    {
        this.FindChildByName<PopupDialog>("LevelSelectDialog").PopupCentered();
    }
}
