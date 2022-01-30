using Godot;
using System;

public class LevelGrid : MenuTemplates
{
    [Export]
    private int _levelPerRow = 3;
    private string _levelType;

    private int _rowCount;
    private int _remainder;

    private VBoxContainer _container;

    private PackedScene _levelNumberIcon;

    private Color _OnGlowColor = new Color(1.09f, 1.04f, 0.79f);
    private Color _OffGlowColor = new Color(0.88f, 0.82f, 0.54f);

    [Signal]
    delegate void PlayAnimation();

    public void Init(string type)
    {
        _levelType = type;
    }
    public override void _Ready()
    {
         base._Ready();
        // _animationPlayer.Play("RESET");

        _container = GetNode<VBoxContainer>("NinePatchRect/ScrollContainer/VBoxContainer");
        // _star_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/star.png");

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

        Connect(nameof(PlayAnimation),_gameManager, "_on_LevelGrid_PlayAnimation");
        EmitSignal(nameof(PlayAnimation));
        
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
            //stars.Texture = _star_texture;
            SetOwned(stars, num);
            SetCompleted(stars, num);

        }
    }

    public void SetOwned(TextureRect star, int levelNumber)
    {
        int owned = _gameManager.GetOwned(_levelType, levelNumber);

        if (owned == 1)
        {
            star.SelfModulate = _OnGlowColor;
        }
    }
    public void SetCompleted(TextureRect star, int levelNumber)
    {
        int best = _gameManager.GetBest(_levelType, levelNumber);
        int owned = _gameManager.GetOwned(_levelType, levelNumber);
        if (best != -1 && owned != 1)
        {
            star.SelfModulate = _OffGlowColor;
        }
    }

    public override void ConnectButtons()
    {
        string nodeName = this.Name;
        string targetMethod = $"_on_{nodeName}_button_pressed";
        foreach (TextureButton button in _buttons)
        {
            button.Connect("pressed", GetTree().Root.GetChild(0), targetMethod, new Godot.Collections.Array { button.Name });
            button.Connect("mouse_exited", this, "_on_mouse_exited", new Godot.Collections.Array { button });
            button.Connect("mouse_entered", this, "_on_mouse_entered", new Godot.Collections.Array { button });
        }
    }

    public override void _on_mouse_entered(TextureButton button)
    {
        if (button.IsInGroup("hoverableButton"))
        {
            _modulate = button.SelfModulate;
            button.SelfModulate *= 1.1f;
            return;
        }
        button.GrabFocus();
    }
    public override void _on_mouse_exited(TextureButton button)
    {
        if (button.IsInGroup("hoverableButton"))
        {
            button.SelfModulate = _modulate;
            return;
        }


    }
}
