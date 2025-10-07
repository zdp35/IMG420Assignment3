extends CharacterBody2D

@export_category("Player Properties")
@export var move_speed: float = 400.0
@export var jump_force: float = 600.0
@export var gravity: float = 30.0
@export var max_jump_count: int = 2

@export_category("Toggle Functions")
@export var double_jump: bool = false

var jump_count: int = max_jump_count

@onready var player_sprite: AnimatedSprite2D = $AnimatedSprite2D
@onready var spawn_point: Node2D = %SpawnPoint


func _physics_process(_delta: float) -> void:
	apply_gravity()
	handle_jumping()
	handle_horizontal()
	move_and_slide()
	player_animations()
	flip_player()


func apply_gravity() -> void:
	if not is_on_floor():
		velocity.y += gravity
	else:
		jump_count = max_jump_count

func handle_horizontal() -> void:
	var axis := Input.get_axis("Left", "Right")
	velocity.x = axis * move_speed

func handle_jumping() -> void:
	if Input.is_action_just_pressed("Jump"):
		if is_on_floor() and not double_jump:
			jump()
		elif double_jump and jump_count > 0:
			jump()
			jump_count -= 1

func jump() -> void:
	if AudioManager and AudioManager.jump_sfx:
		AudioManager.jump_sfx.play()
	velocity.y = -jump_force

# --------- ANIMATION / FLIP ---------- #
func player_animations() -> void:
	if is_on_floor():
		if abs(velocity.x) > 0.0:
			player_sprite.play("Walk", 1.5)
		else:
			player_sprite.play("Idle")
	else:
		player_sprite.play("Jump")

func flip_player() -> void:
	if velocity.x < 0.0:
		player_sprite.flip_h = true
	elif velocity.x > 0.0:
		player_sprite.flip_h = false

# --------- SIGNALS ---------- #
func _on_collision_body_entered(body: Node) -> void:
	if body.is_in_group("Traps"):
		if AudioManager and AudioManager.death_sfx:
			AudioManager.death_sfx.play()
		global_position = spawn_point.global_position
		velocity = Vector2.ZERO
		jump_count = max_jump_count
