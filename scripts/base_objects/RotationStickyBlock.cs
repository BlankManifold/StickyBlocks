using Godot;
using System.Linq;

public class RotationStickyBlock : StickyBlock
{

    private Sprite _sprite;
    private Tween _tween;
    public static bool CanRotate = true;
    private bool _isRotated = false;
    public bool IsRotated
    {
        get { return _isRotated; }
        set { _isRotated = value; }
    }
    private float _rotationAngle = 0f;
    public float RotationAngle
    {
        get { return _rotationAngle; }
        set { _rotationAngle = value; }
    }

    private Godot.Collections.Dictionary _lastState = new Godot.Collections.Dictionary() {{"Rotation",null},{"IsRotated",false}, {"CanRotate",true}, {"IsCurrent",false}};
    public  Godot.Collections.Dictionary LastState { get { return _lastState; } }


    [Export(PropertyHint.Enum, "Rotation0,Rotation30,Rotation45,Rotation60,Rotation90,Rotation180,IRotation30,IRotation45,IRotation60,IRotation90,IRotation180")]
    private string _type = "Rotation0";
    public string Type
    {
        get { return _type; }
        set { _type = value; }
    }

    [Signal]
    public delegate void ImRotating();

    Godot.Collections.Dictionary<int, Color> _colors = new Godot.Collections.Dictionary<int, Color>
    {
        {0, new  Color(0.89f, 0.96f, 1f)}, {30,new Color(0.62f, 0.84f, 1f)},
        {45,new Color(0.25f, 0.69f, 1f)},{60, new Color(0.08f, 0.40f, 0.62f)},
        {90,new Color(0.12f, 0.25f, 0.34f)},{180,new Color(0.008f, 0.04f, 0.07f)}//Color(0.05f, 0.08f, 0.11f)}
    };

    public override void _Ready()
    {
        _sprite = GetNode<Sprite>("Sprite");
        _tween = GetNode<Tween>("Tween");
        SetUp();
    }
    public override void _Input(InputEvent inputEvent)
    {
        if (CanRotate)
        {
            if (inputEvent.IsActionPressed("rotate") && _isMouseOver && _type != "Rotation0")
            {
                if (_isCurrentBlock)
                {
                
                    _isRotated = !_isRotated;
                    Myrotate();

                    EmitSignal(nameof(ImRotating));

                    inputEvent.Dispose();
                    return;
                }

                _isRotated = !_isRotated;
                Myrotate();

                inputEvent.Dispose();
                return;
            }
        }

        inputEvent.Dispose();


    }

    public void UpdateState()
    {
        _lastState["Rotation"] = GlobalRotation;
        _lastState["IsRotated"] = _isRotated;
        _lastState["CanRotate"] = CanRotate;
        // _lastState["IsCurrent"] = _isCurrentBlock;
    }
    public void BackOneMove()
    {
        GlobalRotation = (float)_lastState["Rotation"];
        _isRotated = (bool)_lastState["IsRotated"] ;
        CanRotate = (bool)_lastState["CanRotate"];
        _isCurrentBlock = true;
    }

    

    private void SetUp()
    {
        int angle = _type.Split('n').Last().ToInt();
        _sprite.SelfModulate = _colors[angle];

        if (_type[0] == 'I')
        {
            angle = -angle;
        }

        _rotationAngle = Mathf.Deg2Rad(angle);

    }
    private void Myrotate()
    {
        
        float angle = _rotationAngle;

        if (_isRotated)
        {
            angle = -angle;
        }

        _tween.InterpolateProperty(this, "rotation", Rotation, Rotation + angle, 0.3f);
        _tween.Start();
    }

    public void _on_Tween_tween_started(object _, NodePath node)
    {
        CanRotate = false;
    }
    public void _on_Tween_tween_completed(object _, NodePath node)
    {
        CanRotate = true;
    }
    public void _on_RotationStickyBlock_mouse_entered() => _isMouseOver = true;
    public void _on_RotationStickyBlock_mouse_exited() => _isMouseOver = false;

}


