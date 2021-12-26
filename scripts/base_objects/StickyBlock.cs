using Godot;

public abstract class StickyBlock : StaticBody2D
{

    protected bool _isCurrentBlock = false;
    protected bool _isMouseOver = false;
    
    public bool IsCurrentBlock
    {
        get { return _isCurrentBlock; }
        set { _isCurrentBlock = value; }
    }

    [Export]
    protected bool _isFirst;
    [Export]
    protected bool _isLast;
    public bool IsLast { get { return _isLast; }}

}
