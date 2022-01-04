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

    private AnimationPlayer _animationPlayer;
    private bool _animationBackwards = false;


    public void Init(int level, bool owned, int moves, int best)
    {
            
        _outcomeText = $"Level {level} Completed!";
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if (owned)
        {
            _animationPlayer.Play("glow");
            _outcomeText = $"Level {level} owned!";
        }
     
        _movesText = $"Moves: {moves}";
        _bestText = $"Best: {best}";
        if (best == -1)
        {
            _bestText = "Best: -";
        }

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




 
