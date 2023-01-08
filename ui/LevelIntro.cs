using System;
using Godot;

public class LevelIntro : PopupDialog
{
    bool InitDone;

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        if (!InitDone)
        {
            var grid = GetTree().CurrentScene.FindChildByType<GameGrid>();
            if (grid.Level != null)
            {
                this.FindChildByName<Label>("LevelDesc").Text = grid.Level.Description;
                this.FindChildByName<Button>("StartLevel").Connect("pressed", this, nameof(StartLevelPressed));
                PopupCentered();
                InitDone = true;
            }
        }
    }

    void StartLevelPressed()
    {
        QueueFree();
    }
}
