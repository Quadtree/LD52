using System;
using System.Text;
using Godot;
using Godot.Collections;

public class WinLevelDialog : PopupPanel
{
    HTTPRequest req;

    float MyTime;

    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {

    }

    public void AddScoreAndUpdate(string levelName, float seconds)
    {
        if (OS.IsDebugBuild()) levelName += "_dev";

        MyTime = seconds;

        this.FindChildByName<Label>("YourTime").Text = $"Your time was {seconds:n2}";

        req = new HTTPRequest();
        AddChild(req);

        req.Connect("request_completed", this, nameof(RequestCompleted));

        var outgoingData = new Dictionary<string, object>();
        outgoingData["level"] = levelName;
        outgoingData["timeSeconds"] = seconds;

        req.Request($"https://k1seztx1s2.execute-api.us-west-2.amazonaws.com/default/ld52-scores?data={Godot.StringExtensions.PercentEncode(JSON.Print(outgoingData))}");
    }

    void RequestCompleted(int result, int responseCode, string[] headers, byte[] body)
    {
        GD.Print($"Got response code {responseCode}");

        var data = JSON.Parse(Encoding.UTF8.GetString(body));

        GD.Print(data.Result?.GetType());

        var resData = (Dictionary)data.Result;

        var scoreArray = (Godot.Collections.Array)resData["scoresForThisLevel"];

        var hst = this.FindChildByName<GridContainer>("HighScoreTable");
        hst.ClearChildren();

        foreach (var rowUntyped in scoreArray)
        {
            var row = (Dictionary)rowUntyped;
            GD.Print($"{row["timeSeconds"]}");

            var lbl = UIUtil.Label($"{row["timeSeconds"]:n2}");
            lbl.Align = Label.AlignEnum.Center;
            lbl.SizeFlagsHorizontal = (int)SizeFlags.Expand | (int)SizeFlags.Fill;

            if ((float)row["timeSeconds"] == MyTime)
            {
                lbl.AddColorOverride("font_color", Colors.Yellow);
            }

            hst.AddChild(lbl);
        }

        req.QueueFree();
    }
}
