extends Node2D

var score : int = 0


func add_score():
	score += 1


func load_next_level(next_scene : PackedScene):
	get_tree().change_scene_to_packed(next_scene)
