using System;
using Godot;

public class RestartLevelButton : Button
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.Connect("pressed", this, nameof(OnPressed));
    }

    void OnPressed()
    {
        this.GetTree().ChangeScene("res://maps/Default.tscn");
    }
}
