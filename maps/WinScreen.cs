using System;
using Godot;

public class WinScreen : Control
{
    public override void _Ready()
    {
        this.FindChildByName<Button>("RestartButton").Connect("pressed", this, nameof(RestartButtonPressed));
    }

    void RestartButtonPressed()
    {
        Level.CurrentLevel = 0;
        GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
