using Godot;


public class GameEndedMenu : MenuTemplates
{
    private string _outcomeText;
    private string _movesText;
    private string _bestText;

    private Label _outcomeLabel;
    private Label _bestLabel;
    private Label _movesLabel;
    private TextureRect _starsRect;

    private bool _owned = false;

    [Signal]
    delegate void PlayAnimation();



    public void Init(int level, bool owned, int moves, int best)
    {

        _outcomeText = $"LEVEL {level} COMPLETED!";


        if (owned)
        {
            _owned = true;
            _outcomeText = $"LEVEL {level} OWNED!";
        }

        _movesText = $"MOVES: {moves}";
        
        if (moves > 99)
        {
        _movesText = $"MOVES: DUMB";
        }

        _bestText = $"BEST: {best}";
        if (best == -1)
        {
            _bestText = "BEST: -";
        }

    }

    public override void _Ready()
    {
        base._Ready();

        Connect(nameof(PlayAnimation), _gameManager, "_on_GameEndedMenu_PlayAnimation");

        if (_owned)
        {
            EmitSignal(nameof(PlayAnimation));
        }



        _outcomeLabel = (Label)FindNode("Outcome");
        _bestLabel = (Label)FindNode("Best");
        _movesLabel = (Label)FindNode("Moves");
        _starsRect = (TextureRect)FindNode("Stars");

        _outcomeLabel.Text = _outcomeText;
        _bestLabel.Text = _bestText;
        _movesLabel.Text = _movesText;


    }

}
