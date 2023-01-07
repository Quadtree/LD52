using System;
using Godot;

public class Plant : Spatial
{
    [Export]
    public int RedUsedPerTick;

    [Export]
    public int GreenUsedPerTick;

    [Export]
    public int BlueUsedPerTick;

    public GameGrid Grid;

    public IntVec2 Pos;

    public override void _Ready()
    {

    }

    public override void _Process(float delta)
    {

    }

    public void Reposition(GameGrid gg, IntVec2 pos)
    {
        Grid = gg;
        this.SetGlobalLocation(gg.TileToVector(pos));
    }
}
