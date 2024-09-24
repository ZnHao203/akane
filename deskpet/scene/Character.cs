using Godot;
using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;

public partial class Character : Node2D
{
	// track the speed of the last moving - used for turning checks
	private Vector2I lastSpeed;
	// track the speed of the moving window
	private Vector2I speed;
	// track the facing direction of the character
	private bool directionIsLeft;
	// record the size of current screen;
	private Vector2I screenSize;
	
	// animation fnction pointer: 
	// 		current supported functions: getBubbles
	private AnimationPlayer _animationPlayer;
	
	// handles horizontal turnings
	public void handleHorizontalMotion(bool newDirectionIsLeft)
	{
		// only turn if new direction is different from the last direction
		if (directionIsLeft != newDirectionIsLeft)
		{
			// flip horizontally
			ApplyScale(new Vector2(-1, 1));
			directionIsLeft = newDirectionIsLeft;
		}
		// TODO: add turning animation here
		Debug.Print("handleHorizontal Motion is reached");
	}

	// return the current speed
	public Vector2I getSpeed()
	{
		return speed;
	}
	// set current speed
	public void addSpeed(Vector2I speedUp)
	{
		speed = speed + speedUp;
	}
	
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

	private void reverseHorizontal()
	{
		// handle speed
		speed.X = -speed.X;
		Debug.Print("reverseHorizontal");
	}

	private void reverseVertical()
	{
		// handle speed
		speed.Y = -speed.Y;
		Debug.Print("reverseVertical");

	}

	private void move()
	{
		// out of screen handling
		if (GetWindow().Position.X > screenSize.X || GetWindow().Position.X < 0) 
		if (GetWindow().Position.X < 0) 
		{
			reverseHorizontal();
			Debug.Print("GetWindow().Position.ToString" + GetWindow().Position.ToString());
			// Debug.Print("GetViewport().Size.ToString()" + GetViewport().Size.ToString());
			Debug.Print("screenSize.X" + screenSize.ToString());

		}
		if (GetWindow().Position.Y > screenSize.Y || GetWindow().Position.Y < 0) 
		{
			reverseVertical();
		}
		// check if we want to turn
		if (lastSpeed.X > 0 && speed.X < 0) // turn left
		{
			handleHorizontalMotion(true);
		} else if (lastSpeed.X < 0 && speed.X > 0) // turn right
		{
			handleHorizontalMotion(false);
		}
		// move!
		GetWindow().Position = GetWindow().Position + speed;
		// update last speed
		lastSpeed = speed;
		// update speed - slightly reduce
		var minSpeed = 2;
		if (speed.X > minSpeed) {
			speed.X -= 1;
		} else if (speed.X < 0-minSpeed) {
			speed.X += 1;
		}
		if (speed.Y > minSpeed) {
			speed.Y -= 1;
		} else if (speed.Y < 0-minSpeed) {
			speed.Y += 1;
		}
		Debug.Print("Move is reached");
	}

	

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello from C# to Godot :)");
		
		// initialize local variables
		speed = new Vector2I(0, 10);
		directionIsLeft = true;

		// get screen size
		var screenID = DisplayServer.WindowGetCurrentScreen();
		screenSize = DisplayServer.ScreenGetSize(screenID);

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
		move();
	}
}
