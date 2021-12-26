using Godot;


public class GameEndedMenu : MenuTemplates
{
    private string _outcomeText;
    private string _movesText;
    private string _bestText;
    private Texture _starsTexture;

    private Label _outcomeLabel;
    private Label _bestLabel;
    private Label _movesLabel;
    private TextureRect _starsRect;

    public void Init(int level, int stars, int moves, int best)
    {
        if (stars != 0)
        {
            _outcomeText = $"Level {level} completed!";
            _movesText = $"Moves: {moves}";
            _bestText = $"Best: {best}";
            _starsTexture = ResourceLoader.Load<Texture>($"res://assets/graphic/mainMenu/stars{stars}.png");

            return;
        }
        _outcomeText = "Level not completed!";
        _movesText = "Moves: -";
        _starsTexture = ResourceLoader.Load<Texture>("res://assets/graphic/mainMenu/stars0.png");


        if (best == -1)
        {
            _bestText = "Best: -";
        }
        _bestText = $"Best: {best}";

    }

    public override void _Ready()
    {
        base._Ready();
        
        _outcomeLabel = (Label)FindNode("Outcome");
        _bestLabel = (Label)FindNode("Best");
        _movesLabel = (Label)FindNode("Moves");
        _starsRect = (TextureRect)FindNode("Stars");

        _outcomeLabel.Text = _outcomeText;
        _bestLabel.Text = _bestText;
        _movesLabel.Text = _movesText;
        _starsRect.Texture = _starsTexture;

    }


}
