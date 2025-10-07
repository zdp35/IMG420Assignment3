extends Area2D

# Define the next scene to load in the inspector
@export var next_scene : PackedScene

# Load next level scene when player collide with level finish door.
func _on_body_entered(body):
	if body.is_in_group("Player"):
		AudioManager.level_complete_sfx.play()
		SceneTransition.load_scene(next_scene)
