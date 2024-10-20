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
	// record the size of current game window;
	private Vector2I windowSize;
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
			// flip horizontally then turn
			// NOTE: need to adjust fish turn animation to adapt this
			
			directionIsLeft = newDirectionIsLeft;
			ApplyScale(new Vector2(-1, 1));
			_animationPlayer.Play("fishTurn");

			isBubbleEvent= true;
			_bubbleEventTimer.Start(0.6);
			Debug.Print("handleHorizontal Motion is reached");
		}
		
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
		
		// // Check if the input event is a mouse button event
		// if (inputEvent is InputEventMouseButton mouseEvent)
		// {
		// 	// Check if the left mouse button was pressed
		// 	if (mouseEvent.ButtonIndex == MouseButton.Left && mouseEvent.Pressed)
		// 	{
		// 		// increase speed 
		// 		moveCtrl.increaseSpeed();
		// 		// stop bubbling
		// 		_bubbleEventTimer.Stop();
		// 		isBubbleEvent = false;
		// 	}
		// }
			
	}

	// bubble event handling
	private bool isBubbleEvent;
	//private Godot.Timer bubbleEventTimer = new Godot.Timer();
	private void _on_timer_timeout()
	{
		isBubbleEvent = false;
		_animationPlayer.Stop();
		_animationPlayer.Play("RESET");
		Debug.Print("TIMER TIMEOUT INTERRUPT is reached");
	}
	
	// start get bubble animation and timer
	private void startBubbleEvent()
	{
		isBubbleEvent = true;
		_animationPlayer.Stop();

		// want a mod for glass!
		if (!glass_mod) {
			GetNode<Sprite2D>("glass").Visible = false;
			GD.Print("_on_fishturn_animation_player_animation_finished glass.Visible = false reached");
		}

		_animationPlayer.Play("getBubbles");
		_bubbleEventTimer.Start(2);
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
	// when the fish is idle, occasionally get some bubbles
	private void handleIdleMotion()
	{
		Random random = new Random();
		int randomNumber = random.Next(0, 100);
		// random movement 1 - stop and get bubbles
		if (randomNumber < 1) 
		{
			Debug.Print("Minimum speed branch BUBBLE is reached");
			startBubbleEvent();
		}
	}

	private void handleRotation()
	{
		GetNode<Node2D>(".").RotationDegrees = moveCtrl.getRotationDegrees(GetNode<Node2D>(".").RotationDegrees);
	}
	private void move()
	{
		// handleDisplacement();
		
		// check if we need to wait until the bubble event is finished
		if (isBubbleEvent) return;

		Vector2I displacement = moveCtrl.getDisplacement();
		//handleRotation();

		// out of screen handling
		// reverse speed direction if at bound of screen 
		if (GetWindow().Position.X < 0) 
		{
			if (displacement.X < 0) displacement.X = 0;
			Debug.Print("SCREEN EDGE HANDLING CASE 1 is reached");
		} else if (GetWindow().Position.X > (screenSize.X - windowSize.X)) // consider the buffer of the game window itself
		{
			if (displacement.X > 0) displacement.X = 0;
			Debug.Print("SCREEN EDGE HANDLING CASE 2 is reached");
		}
		
		if (GetWindow().Position.Y > (screenSize.Y - windowSize.Y))  // consider the buffer of the game window itself
		{
			if (displacement.Y > 0) displacement.Y = 0;
			Debug.Print("SCREEN EDGE HANDLING CASE 3 is reached");
		} else if (GetWindow().Position.Y < 0)
		{
			if (displacement.Y < 0) displacement.Y = 0;
			Debug.Print("SCREEN EDGE HANDLING CASE 4 is reached");
		}

		// turning animation
		if (lastSpeed.X > 0 && displacement.X < 0) // turn left animation
		{
			handleHorizontalMotion(true);
		} else if (lastSpeed.X < 0 && displacement.X > 0) // turn right animation
		{
			handleHorizontalMotion(false);
		} else if (displacement.X == 0 && displacement.Y == 0) // idle bubble animation
		{
			handleIdleMotion();
		}

		// Debug.Print("Move is reached, last speed is "+lastSpeed.ToString() + ", current is " + displacement.ToString());
		// move!
		GetWindow().Position = GetWindow().Position + displacement;
		// update variables - but not include 0, so we have previous direction
		if (displacement.X != 0) lastSpeed = displacement;
	}

	
	private MovementControl moveCtrl;

	// set to true if you want some glasses
	private bool glass_mod = false;
	private void _on_fishturn_animation_player_animation_finished(StringName anim_name)
	{
		// want a mod for glass!
		if (!glass_mod) {
			GetNode<Sprite2D>("glass").Visible = false;
			GD.Print("_on_fishturn_animation_player_animation_finished glass.Visible = false reached");
		}
	}
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Hello from C# to Godot :)");
		
		// initialize local variables
		speed = new Vector2I(2, 2);
		lastSpeed = new Vector2I(2, 2);
		// ApplyScale(new Vector2(-1, 1));
		directionIsLeft = false;
		isBubbleEvent = false;

		// when inited, we don't want fishTurn texture to be visible
		var fishTurn = GetNode<Sprite2D>("turn");
		fishTurn.Visible = false;

		// set initial position
		GetWindow().Position = new Vector2I(0,0);

		// get screen size
		var screenID = DisplayServer.WindowGetCurrentScreen();
		screenSize = DisplayServer.ScreenGetSize(screenID);

		// stretch window size to fit 
		// tested 250 window width in 1920x1280 resolution screen, which look good
		double scaleFactor = Math.Ceiling((double) screenSize.X / (double)1920);
		GD.Print("the screen size is " + screenSize.X.ToString());
		GD.Print("the window scale factor is set to " + ((int)scaleFactor).ToString());

		// get window size for border checks
		windowSize = GetWindow().Size; // initially should be (250,200)
		Vector2I newSize = new Vector2I(windowSize.X*(int)scaleFactor, windowSize.Y*(int)scaleFactor);
		GetWindow().Size = newSize;  // scale viewport size
		windowSize = GetWindow().Size; // then save a again


		// Get the Area2D node
		var area2D = GetNode<Area2D>("body/Area2D");

		// Connect the input_event signal to the handler method
		// area2D.Connect("input_event", new Callable(this, nameof(_on_area_2d_input_event)));

		// Get the AnimationPlayer node (adjust the path to your AnimationPlayer)
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_bubbleEventTimer = GetNode<Godot.Timer>("Timer");

		// animate the fins
		GetNode<AnimatedSprite2D>("pectoral/pectoralAnimatedSprite2D").Play();
		GetNode<AnimatedSprite2D>("dorsal/dorsalAnimatedSprite2D").Play();
		GetNode<AnimatedSprite2D>("anal/analAnimatedSprite2D").Play();
		GetNode<AnimatedSprite2D>("tail/tailAnimatedSprite2D").Play();

		// want a mod for glass!
		if (glass_mod)
		{
			GetNode<AnimatedSprite2D>("glass/glassAnimatedSprite2D").Play();
			GetNode<Sprite2D>("glass").Visible = true;
		} else {
			GetNode<Sprite2D>("glass").Visible = false;
		}
		

		moveCtrl = new MovementControl();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		move();
	}
}
