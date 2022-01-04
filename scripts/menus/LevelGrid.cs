using Godot;

public class LevelGrid : MenuTemplates
{
    [Export]
    private int _levelPerRow = 3;
    private string _levelType;
    private Texture _star_texture;
    private AnimationPlayer _animationPlayer;
    private bool _animationBackwards = false;

    public override void _Ready()
    {

        base._Ready();

        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animationPlayer.Play("glow");
        _star_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/star.png");

        int row = 0;
        int col = 0;
        int num = 0;
        Godot.Collections.Array containers = GetNode<VBoxContainer>("NinePatchRect/ScrollContainer/VBoxContainer").GetChildren();
        foreach (HBoxContainer container in containers)
        {
            Godot.Collections.Array icons = container.GetChildren();
            foreach (LevelNumberIcon icon in icons)
            {
                num = row * _levelPerRow + col;
                icon.Name = "Level" + num.ToString();
                TextureButton button = icon.Button;
                if (num == 0)
                {
                    button.GrabFocus();
                }
                TextureRect stars = icon.StarsRect;
                stars.Texture = _star_texture;
                button.Name = icon.Name;
                SetOwned(stars, num);
                SetCompleted(stars, num);
                col++;
            }
            row++;
            col = 0;
        }
    }

    public void SetOwned(TextureRect star, int levelNumber)
    {
        int owned = _gameManager.GetOwned("Easy", levelNumber);

        if (owned == 1)
        {
            star.SelfModulate = new Color(1.09f, 1.04f, 0.79f);
        }
    }
    public void SetCompleted(TextureRect star, int levelNumber)
    {
        int best = _gameManager.GetBest("Easy", levelNumber);
        int owned = _gameManager.GetOwned("Easy", levelNumber);
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
            
            _animationBackwards = !_animationBackwards;
        }
    }
}
