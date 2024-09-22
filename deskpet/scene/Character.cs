using Godot;
using System;

public partial class Character : Node2D
{
	private AnimationPlayer _animationPlayer;
	
	// This is the signal handler method that will be connected to the Area2D signal
	private void _on_area_2d_input_event(Node viewport, InputEvent inputEvent, int shapeIdx) 
	{
		GD.Print("Input Event detected: ", inputEvent);
		
		// Check if the input event is a mouse button event
		if (inputEvent is InputEventMouseButton mouseEvent)
		{
			// Check if the left mouse button was pressed
			if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
			{
				// Trigger the animation (replace "your_animation_name" with the actual animation name)
				if (_animationPlayer != null)
				{
					_animationPlayer.Play("getBubbles");
				}
			}
		}
			
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello from C# to Godot :)");
		
		// Get the Area2D node
		var area2D = GetNode<Area2D>("fishLeft/Area2D");

		// Connect the input_event signal to the handler method
		// area2D.Connect("input_event", new Callable(this, nameof(_on_area_2d_input_event)));

		// Get the AnimationPlayer node (adjust the path to your AnimationPlayer)
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
