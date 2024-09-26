using Godot;
using System;
using System.Data.Common;
using System.Diagnostics;

public partial class MainWindow : Control
{
	private Character fish;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fish = GetNode<Character>("character");
		Debug.Print(GetWindow().Position.ToString());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Debug.Print(fish.getSpeed().ToString());
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
				} 
				// else if (eventKey.Keycode == Key.Left)
				// {
				// 	// Vector2I deltaPosition = new Vector2I(-20, 0);
				// 	// GetWindow().Position = GetWindow().Position + deltaPosition;
				// 	// fish.handleHorizontalMotion(true);
				// 	fish.addSpeed(new Vector2I(-30, 0));
				// } else if (eventKey.Keycode == Key.Right)
				// {
				// 	// Vector2I deltaPosition = new Vector2I(20, 0);
				// 	// GetWindow().Position = GetWindow().Position + deltaPosition;
				// 	// fish.handleHorizontalMotion(false);
				// 	fish.addSpeed(new Vector2I(30, 0));
				// } else if (eventKey.Keycode == Key.Up)
				// {
				// 	// Vector2I deltaPosition = new Vector2I(0, -20);
				// 	// GetWindow().Position = GetWindow().Position + deltaPosition;
				// 	fish.addSpeed(new Vector2I(0, -20));
				// } else if (eventKey.Keycode == Key.Down)
				// {
				// 	// Vector2I deltaPosition = new Vector2I(0, 20);
				// 	// GetWindow().Position = GetWindow().Position + deltaPosition;
				// 	fish.addSpeed(new Vector2I(0, 20));
				// } 
			}
		}
			

	}
}
