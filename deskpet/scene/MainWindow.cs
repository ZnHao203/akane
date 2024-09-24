using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;

public partial class MainWindow : Control
{
	// track the speed of the moving window, not in-use now
	private Vector2I speed;
	// track the facing direction of the character
	private bool directionIsLeft;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// NOTE: I don't think all these code works...
		// OS.WindowPerPixelTransparencyEnabled = true;
		// GetTree().Root.GetViewport().TransparentBg = true;
		// GetTree().Root.TransparentBg = true;
		// application/boot_splash/bg_color
		// DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Transparent, true, 0);
		// OS.WindowPosition = new Vector2(100, 100);
		// GetWindow().Position

		// initialize local variables
		speed = new Vector2I(0, 20);
		directionIsLeft = true;
		Debug.Print(GetWindow().Position.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void handleHorizontalMotion(bool newDirectionIsLeft)
	{
		// only turn if new direction is different from the last direction
		if (directionIsLeft != newDirectionIsLeft)
		{
			// flip horizontally
			var fishChar = GetNode<Character>("character");
			fishChar.ApplyScale(new Vector2(-1, 1));
			directionIsLeft = newDirectionIsLeft;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed) 
			{
				if (eventKey.Keycode == Key.Escape)
				{
					GetTree().Quit();
				} else if (eventKey.Keycode == Key.Left)
				{
					Vector2I deltaPosition = new Vector2I(-20, 0);
					GetWindow().Position = GetWindow().Position + deltaPosition;
					handleHorizontalMotion(true);
				} else if (eventKey.Keycode == Key.Right)
				{
					Vector2I deltaPosition = new Vector2I(20, 0);
					GetWindow().Position = GetWindow().Position + deltaPosition;
					handleHorizontalMotion(false);
				} else if (eventKey.Keycode == Key.Up)
				{
					Vector2I deltaPosition = new Vector2I(0, -20);
					GetWindow().Position = GetWindow().Position + deltaPosition;
				} else if (eventKey.Keycode == Key.Down)
				{
					Vector2I deltaPosition = new Vector2I(0, 20);
					GetWindow().Position = GetWindow().Position + deltaPosition;
				} 
			}
		}
			

	}
}
