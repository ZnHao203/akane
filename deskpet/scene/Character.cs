using Godot;
using System;
using System.Security.Cryptography;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

public partial class Character : Node2D
{
	// track the minimum speed for the fish
	private int minSpeed = 1;
	// track the speed of the last moving - used for turning checks
	private Vector2I lastSpeed;
	// track the speed of the moving window
	private Vector2I speed;
	// track the facing direction of the character
	private bool directionIsLeft;
	// record the size of current screen;
	private Vector2I screenSize;
	private Godot.Timer _bubbleEventTimer;
	
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
				// // Trigger the animation (replace "your_animation_name" with the actual animation name)
				// if (_animationPlayer != null)
				// {
				// 	_animationPlayer.Play("getBubbles");
				// }
				startSpeedUp(60, 30);
			}
		}
			
	}

	private void reverseHorizontal()
	{
		// handle speed
		// speed.X = -speed.X;
		// speed up a bit to avoid continuous flipping
		if (speed.X > 0) {
			speed.X = 0 - speed.X - 5;
		} else {
			speed.X = 0 - speed.X + 5;
		}
		Debug.Print("reverseHorizontal");
	}

	private void reverseVertical()
	{
		// handle speed
		// speed.Y = -speed.Y;
		if (speed.Y > 0) {
			speed.Y = 0 - speed.Y - 2;
		} else {
			speed.Y = 0 - speed.Y + 2;
		}
		Debug.Print("reverseVertical");

	}

	// reduce speed linearly until reaches minimum
	private void reduceSpeedRegular()
	{
		// update last speed
		lastSpeed = speed;
		// update speed - slightly reduce
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
	}
	// bubble event handling
	private bool isBubbleEvent = false;
	//private Godot.Timer bubbleEventTimer = new Godot.Timer();
	private void _on_timer_timeout()
	{
		isBubbleEvent = false;
		Debug.Print("TIMER TIMEOUT INTERRUPT is reached");
	}
	
	// start get bubble animation and timer
	private void startBubbleEvent()
	{
		isBubbleEvent = true;
		_animationPlayer.Play("getBubbles");
		_bubbleEventTimer.Start();
	}
	// increse the speed for random direction and random value
	private void startSpeedUp(int maxSpeedUpX, int maxSpeedUpY)
	{
		Random random = new Random();
		int randomNumber = random.Next(0, 100);
		// randomly determine speedup direction and value
		if (randomNumber < 30)
		{
			addSpeed(new Vector2I(random.Next(0-maxSpeedUpX, 0), 0));
		} else if (randomNumber < 60)
		{
			addSpeed(new Vector2I(random.Next(0, maxSpeedUpX), 0));
		} else if (randomNumber < 80)
		{
			addSpeed(new Vector2I(0, random.Next(0-maxSpeedUpY, 0)));
		} else
		{
			addSpeed(new Vector2I(0, random.Next(0, maxSpeedUpY)));
		} 
	}
	// if at minimum speed, to make the fish more realistic, add some random movement
	// 1 - add bubbles
	// 2 - accelerate on random direction
	private void checkSpecialEvent()
	{
		// check if at minimum speed!
		if ((speed.X <= minSpeed) && (speed.X >= 0-minSpeed) &&
			(speed.Y <= minSpeed) && (speed.Y >= 0-minSpeed))
		{
			// Debug.Print("Minimum speed branch is reached");
			// initialize random number generator
			Random random = new Random();
			int randomNumber = random.Next(0, 100);
			// random movement 1 - stop and get bubbles
			if (randomNumber < 1) 
			{
				Debug.Print("Minimum speed branch BUBBLE is reached");
				startBubbleEvent();
			}
			// random movement 2 - speed up!
			randomNumber = random.Next(0, 100);

			// there should be a number FPS vs the time I expect it to accelerate
			if (randomNumber <= 1) 
			{
				Debug.Print("Minimum speed branch SPEEDUP is reached");
				startSpeedUp(30, 10);
			}
		}
	}
	// get next speed we should use
	private Vector2I getDisplacement() 
	{
		// out of screen handling
		// reverse speed direction if at bound of screen 
		if (GetWindow().Position.X < 0 || GetWindow().Position.X > screenSize.X) 
		{
			reverseHorizontal();
		}
		if (GetWindow().Position.Y > screenSize.Y || GetWindow().Position.Y < 0) 
		{
			reverseVertical();
		}
		return speed;
	}
	private void handleDisplacement()
	{
		// check if we need to wait until the bubble event is finished
		if (isBubbleEvent) return;

		// flip and turn if changed horizontal direction
		Vector2I displacement = getDisplacement();
		if (lastSpeed.X > 0 && displacement.X < 0) // turn left
		{
			handleHorizontalMotion(true);
		} else if (lastSpeed.X < 0 && displacement.X > 0) // turn right
		{
			handleHorizontalMotion(false);
		}
		// reduce speed slightly, update speed/lastSpeed variables
		reduceSpeedRegular();
		// add randomized event: (1) bubbles or (2) speeding up
		checkSpecialEvent();
		Debug.Print(displacement.ToString());

		// move!
		GetWindow().Position = GetWindow().Position + displacement;
	}

	private void move()
	{
		handleDisplacement();
		// Debug.Print("Move is reached");
	}

	

	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello from C# to Godot :)");
		
		// initialize local variables
		speed = new Vector2I(0, 2);
		lastSpeed = new Vector2I(-1, 2);
		directionIsLeft = true;

		// set initial position
		GetWindow().Position = new Vector2I(0,0);

		// get screen size
		var screenID = DisplayServer.WindowGetCurrentScreen();
		screenSize = DisplayServer.ScreenGetSize(screenID);

		// Get the Area2D node
		var area2D = GetNode<Area2D>("fishLeft/Area2D");

		// Connect the input_event signal to the handler method
		// area2D.Connect("input_event", new Callable(this, nameof(_on_area_2d_input_event)));

		// Get the AnimationPlayer node (adjust the path to your AnimationPlayer)
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_bubbleEventTimer = GetNode<Godot.Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		move();
	}
}
