using Godot;
using System.Collections.Generic;

public partial class Boid : CharacterBody2D
{
	// Adjustable in the Inspector
	[Export] public float MaxSpeed = 500.0f;
	[Export] public float SeparationWeight = 1.0f;
	[Export] public float AlignmentWeight = 1.0f;
	[Export] public float CohesionWeight = 1.0f;
	[Export] public float FollowWeight = 1.0f;
	[Export] public float FollowRadius = 200.0f;
	[Export] public float SeparationDistance = 100.0f;
	private List<Boid> _neighbors = new();
	private Area2D _detectionArea;
	private CharacterBody2D _target;
	public override void _Ready()
	{
		_detectionArea = GetNode<Area2D>("DetectionArea");
		_detectionArea.BodyEntered += OnBodyEntered;
		_detectionArea.BodyExited += OnBodyExited;
		var viewportRect = GetViewportRect();
		var randomX = GD.Randf() * viewportRect.Size.X;
		var randomY = GD.Randf() * viewportRect.Size.Y;
		Position = new Vector2(randomX, randomY);
		
		var randomAngle = GD.Randf() * Mathf.Pi * 2;
		var randomVelocity = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)) * MaxSpeed;
		Velocity = randomVelocity;
		MoveAndSlide();
		
		// Optional: Make boid face its direction of movement
		LookAt(Position + Velocity);
	}
	public void SetTarget(CharacterBody2D Target)
	{
		_target = Target;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 separationVector = Separation() * SeparationWeight;
		
		Vector2 alignmentVector = Alignment() * AlignmentWeight;
		Vector2 cohesionVector = Cohesion() * CohesionWeight;
		Vector2 followVector = Centralization() * FollowWeight;
		Vector2 direction = (separationVector + alignmentVector + cohesionVector + followVector).Normalized();
		//GD.Print($"The value of my dir is: {direction}");
		Velocity = Velocity.Lerp(direction*MaxSpeed, (float)delta);
		
		// Clamp velocity to prevent boids from moving too fast
		//Velocity = Velocity.LimitLength(MaxSpeed);
		//GD.Print($"The value of my variable is: {Velocity}");
		MoveAndSlide();
		
		// Optional: Make boid face its direction of movement
		LookAt(Position + Velocity);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Boid boid && body != this)
		{
			_neighbors.Add(boid);
		}
	}

	private void OnBodyExited(Node2D body)
	{
		if (body is Boid boid && body != this)
		{
			_neighbors.Remove(boid);
		}
	}

	// Rule 1: Separation—Avoid crowding neighbors
	private Vector2 Separation()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 steer = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			Vector2 diff = Position - neighbor.Position;
			if (diff.Length() < SeparationDistance)
				steer += diff / diff.Length();
		}
		//GD.Print($"The value of my variable is: {steer}");
		return steer.Normalized();
	}

	// Rule 2: Alignment—Steer towards the average heading of neighbors
	private Vector2 Alignment()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 averageVelocity = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			averageVelocity += neighbor.Velocity;
		}
		averageVelocity /= _neighbors.Count;
		return averageVelocity.Normalized();
	}

	// Rule 3: Cohesion—Move towards the average position of neighbors
	private Vector2 Cohesion()
	{
		if (_neighbors.Count == 0) return Vector2.Zero;

		Vector2 centerOfMass = Vector2.Zero;
		foreach (var neighbor in _neighbors)
		{
			centerOfMass += neighbor.Position;
		}
		centerOfMass /= _neighbors.Count;

		Vector2 directionToCenter = centerOfMass - Position;
		return directionToCenter.Normalized();
	}
	
	private Vector2 Centralization()
	{
		if(_target != null)
		{
			////GD.Print($"The value of my variable is: {_target.GetPosition()}");
			////GD.Print($"The value of my variable is: {Position}");
			if (Position.DistanceTo(_target.GetPosition()) < FollowRadius)
			{
				return Vector2.Zero;
			}
			else
			{
				return ((_target.GetPosition() - Position).Normalized());
			}
		}
		else
		{
			return Vector2.Zero;
		}
		
	}
	private void BoidExitedScreen()
	{
		//GD.Print("Got here");
		Velocity *= -1;
		MoveAndSlide();
		
		// Optional: Make boid face its direction of movement
		LookAt(Position + Velocity);
	}
}
