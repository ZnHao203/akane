using Godot;
using System;
using System.Data.Common;

public partial class MainWindow : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// OS.WindowPerPixelTransparencyEnabled = true;
		GetTree().Root.GetViewport().TransparentBg = true;
		GetTree().Root.TransparentBg = true;
		DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
				GetTree().Quit();
	}
}
