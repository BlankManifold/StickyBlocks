using Godot;

public class LevelGrid : LevelTypeMenu
{
    [Export]
    private int _levelPerRow = 3;
    private string _levelType;
    private Texture _star1_texture;
    public Texture Star1 { get { return _star1_texture; } }
    private Texture _star2_texture;
    public Texture Star2 { get { return _star2_texture; } }
    private Texture _star3_texture;
    public Texture Star3 { get { return _star3_texture; } }

    
    public override void _Ready()
    {

        base._Ready();

        _star1_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/stars1.png");
        _star2_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/stars2.png");
        _star3_texture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/stars3.png");

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
                TextureRect stars = icon.StarsRect;
                button.Name = icon.Name;
                SetStars(stars,num);
                col++;
            }
            row++;
            col = 0;
        }
    }

    public void SetStars(TextureRect stars, int levelNumber)
    {
        int starsNumber = _gameManager.GetStars("Easy",levelNumber);
        switch (starsNumber)
        {
            case 0:
                break;
            case 1:
                stars.Texture = _star1_texture;
                break;
            case 2:
                stars.Texture = _star2_texture;
                break;
            case 3:
                stars.Texture = _star3_texture;
                break;
        }
    }

}
