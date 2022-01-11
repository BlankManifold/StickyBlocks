using Godot;
using System;

public class LevelGrid : MenuTemplates
{
    [Export]
    private int _levelPerRow = 3;
    private string _levelType;

    private int _rowCount;
    private int _remainder;
    private Texture _star_texture;
    private AnimationPlayer _animationPlayer;

    private VBoxContainer _container;
    private bool _animationBackwards = false;

    private PackedScene _levelNumberIcon;

    public void Init(string type)
    {
        _levelType = type;
    }
    public override void _Ready()
    {
        base._Ready();
        // _animationPlayer.Play("RESET");

        _container = GetNode<VBoxContainer>("NinePatchRect/ScrollContainer/VBoxContainer");
        _star_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/star.png");

        _levelNumberIcon = ResourceLoader.Load<PackedScene>("res://scene/base_objects/LevelNumberIcon.tscn");
        _gameManager.CurrentLevelType = _levelType;

        int totalLevelCount = _gameManager.MaxLevel[_levelType];
        _rowCount = Math.DivRem(totalLevelCount, _levelPerRow, out _remainder);
        for (int row = 0; row < _rowCount; row++)
        {
            AddRow(row);
        }
        if (_remainder != 0)
        {
            AddRow(_rowCount, _remainder);
        }
        _buttons = new Godot.Collections.Array<TextureButton>(GetTree().GetNodesInGroup("buttons"));


        LevelNumberIcon firstIcon = _container.GetNode<LevelNumberIcon>("HBoxContainer0/Level0");
        TextureButton firstButton = firstIcon.Button;
        firstButton.GrabFocus();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("RESET");
        _animationPlayer.Play("glow");
    }

    public void AddRow(int currentRow, int remainder = 0)
    {
        HBoxContainer iconContainer = new HBoxContainer();
        iconContainer.Set("custom_constants/separation", 100);
        iconContainer.Alignment = BoxContainer.AlignMode.Center;
        iconContainer.Name = "HBoxContainer" + currentRow.ToString();
        _container.AddChild(iconContainer);

        int iconCount = _levelPerRow;

        if (remainder != 0)
        {
            iconCount = remainder;
            iconContainer.Alignment = BoxContainer.AlignMode.Begin;
        }

        for (int i = 0; i < iconCount; i++)
        {
            LevelNumberIcon icon = _levelNumberIcon.Instance<LevelNumberIcon>();

            int num = currentRow * _levelPerRow + i;
            icon.LevelNumber = num.ToString();
            icon.Name = "Level" + icon.LevelNumber;

            iconContainer.AddChild(icon);

            TextureButton button = icon.Button;
            button.Name = icon.Name;

            TextureRect stars = icon.StarsRect;
            stars.Texture = _star_texture;
            SetOwned(stars, num);
            SetCompleted(stars, num);

        }
    }

    public void SetOwned(TextureRect star, int levelNumber)
    {
        int owned = _gameManager.GetOwned(_levelType, levelNumber);

        if (owned == 1)
        {
            star.SelfModulate = new Color(1.09f, 1.04f, 0.79f);
        }
    }
    public void SetCompleted(TextureRect star, int levelNumber)
    {
        int best = _gameManager.GetBest(_levelType, levelNumber);
        int owned = _gameManager.GetOwned(_levelType, levelNumber);
        if (best != -1 && owned != 1)
        {
            star.SelfModulate = new Color(0.79f, 0.74f, 0.49f);
        }
    }

    public void _on_AnimationPlayer_animation_finished(string name)
    {
        if (name == "glow")
        {
            if (_animationBackwards)
            {
                _animationPlayer.Play("glow");
            }
            else
            {
                _animationPlayer.PlayBackwards("glow");
            }

        }
    }

}
