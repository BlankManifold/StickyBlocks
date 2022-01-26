using Godot;
using System.Collections.Generic;
using PlayerDataType = Godot.Collections.Dictionary;
//System.Collections.Generic.Dictionary<string,
//                             System.Collections.Generic.Dictionary<string,
//                                 System.Collections.Generic.Dictionary<string, int>>>;

public class GameManager : Node2D
{
    [Export]
    private Dictionary<string, int> _maxLevel = new Dictionary<string, int>() { { "EASY", 0 }, { "MEDIUM", 0 }, { "HARD", 0 } };
    public Dictionary<string, int> MaxLevel { get { return _maxLevel; } }
    private Dictionary<string,   bool> _justUnLocked = new Dictionary<string, bool>() {{ "EASY", false },{ "MEDIUM", false }, { "HARD", false } };
    public Dictionary<string, bool> JustUnLocked { get { return _justUnLocked; } }

    private Level _currentLevel;
    private int _currentLevelNumber = 0;
    public int CurrentLevelNumber { get { return _currentLevelNumber; }}
    private string _currentLevelType = "EASY";
    public string CurrentLevelType { get { return _currentLevelType; } set { _currentLevelType = value;}}
    private string[] _levelTypes = { "EASY", "MEDIUM", "HARD" };
    private Dictionary<string, string> _levelChain = new Dictionary<string, string>()  { { "EASY", "MEDIUM"}, { "MEDIUM", "HARD" },{ "HARD", null }};
    private Godot.Collections.Dictionary _levelLockDictionary = new Godot.Collections.Dictionary() { };
    private string _levelLockPath = "user://levelLock.dat";
    private PlayerDataType _playerDataDictionary = new PlayerDataType() { };
    private string _playerDataPath = "user://playerData.dat";
    public PlayerDataType PlayerDataDictionary { get { return _playerDataDictionary; } }
    public Godot.Collections.Dictionary LevelLockDictionary { get { return _levelLockDictionary; } }
    public string LevelLockPath { get { return _levelLockPath; } }
    public string PlayerDataPath { get { return _playerDataPath; } }

    private AnimationPlayer _gameManagerAnimationPlayer;
    private bool _animationBackwards = false;


    [Signal]
    public delegate void LevelCompleted(int level, bool owned, int moves, int best);
    [Signal]
    public delegate void QuitPressed();
    

    public void LoadDefaultPlayerData()
    {
        Dictionary<string, int> dataDictionary = new Dictionary<string, int>() { { "Owned", 0 }, { "Best", -1 } };
        _levelLockDictionary = new Godot.Collections.Dictionary() { };
        _playerDataDictionary = new PlayerDataType() { };

        foreach (string type in _levelTypes)
        {
            Dictionary<string, Dictionary<string, int>> typeDictionary = new Dictionary<string, Dictionary<string, int>>();
            Godot.Collections.Dictionary levelDataDictionary = new Godot.Collections.Dictionary();

            if (type == "EASY")
            {
                levelDataDictionary.Add("Unlocked", true);
            }
            else
            {
                levelDataDictionary.Add("Unlocked", false);
            }

            levelDataDictionary.Add("Completed", 0);
            _levelLockDictionary.Add(type, levelDataDictionary);


            for (int i = 0; i <= _maxLevel[type]; i++)
            {
                typeDictionary.Add($"Level{i}", dataDictionary);
            }
            _playerDataDictionary.Add(type, typeDictionary);
        }


    }
    public void LoadPlayerData()
    {

        File file1 = new File();
        File file2 = new File();
        Error err1 = file1.Open(_levelLockPath, File.ModeFlags.Read);
        Error err2 = file2.Open(_playerDataPath, File.ModeFlags.Read);
        if (err1 != 0 || err2 != 0)
        {
            GD.Print("Error: loading player data");
            LoadDefaultPlayerData();
            return;
        }

        _levelLockDictionary = (Godot.Collections.Dictionary)file1.GetVar();
        _playerDataDictionary = (PlayerDataType)file2.GetVar();

        file1.Close();
        file2.Close();
    }
    public void SavePlayerData()
    {
        File file1 = new File();
        File file2 = new File();
        Error err1 = file1.Open(_levelLockPath, File.ModeFlags.Write);
        Error err2 = file2.Open(_playerDataPath, File.ModeFlags.Write);
        if (err1 != 0 || err2 != 0)
        {
            GD.Print("Error: saving player data");
            return;
        }
        file1.StoreVar(_levelLockDictionary);
        file2.StoreVar(_playerDataDictionary);
        file1.Close();
        file2.Close();
    }

    public int GetOwned(string type, int level)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        return (int)dict2["Owned"];
    }
    public void SetOwned(string type, int level, int owned)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        dict2["Owned"] = owned;
    }
    public int GetBest(string type, int level)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        return (int)dict2["Best"];
    }
    public void SetBest(string type, int level, int best)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        dict2["Best"] = best;
    }
    public int GetData(string type, int level, string data)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        return (int)dict2[data];
    }
    public void SetData(string type, int level, string dataType, int data)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_playerDataDictionary[type];
        Godot.Collections.Dictionary dict2 = (Godot.Collections.Dictionary)dict1[$"Level{level}"];
        dict2[dataType] = data;
    }


    public bool IsLevelUnlocked(string type)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_levelLockDictionary[type];
        return (bool)dict1["Unlocked"];
    }
    public void SetLevelUnlocked(string type)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_levelLockDictionary[type];
        dict1["Unlocked"] = true;
    }

    public int NumberOfLevel(string type)
    {
        return _maxLevel[type];
    }
    public int NumberOfCompleted(string type)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_levelLockDictionary[type];
        return (int)dict1["Completed"];
    }
    public void AddCompleted(string type)
    {
        Godot.Collections.Dictionary dict1 = (Godot.Collections.Dictionary)_levelLockDictionary[type];
        dict1["Completed"] = (int)dict1["Completed"] + 1;
    }


    public void LoadLevel(string type, int levelNumber)
    {

        string path = $"res://scene/levels/{type}/Level{levelNumber}.tscn";
        PackedScene scene = ResourceLoader.Load<PackedScene>(path);

        Level level = scene.Instance<Level>();
        level.Number = levelNumber;
        level.Name = "Level";
        AddChild(level);
        level.Connect(nameof(Level.GameCompleted), this, nameof(_on_Level_GameCompleted));

        _currentLevel = level;
        _currentLevelNumber = levelNumber;
        // _currentLevelType = level.Type;
    }
    public void NextLevel() => LoadLevel(_currentLevelType, _currentLevelNumber + 1);
    public void UnLockNextType(string type)
    {
        string nextType = _levelChain[type];
        if (nextType != null)
        {   
            _justUnLocked[nextType] = true;
            SetLevelUnlocked(nextType);
        }
    }
    public bool UnlockedCondtion(string type)
    {
        return (NumberOfCompleted(type) == _maxLevel[type]);
    }



    public override void _Ready()
    {
        LoadPlayerData();
        _gameManagerAnimationPlayer = GetNode<AnimationPlayer>("GameManagerAnimationPlayer");
        // LoadDefaultPlayerData(); SavePlayerData();
    }


    public void _on_Level_GameCompleted(bool owned, int movesCounter)
    {
        string type = _currentLevel.Type;

        int highscore = GetData(type, _currentLevelNumber, "Best");
        if (highscore == -1)
        {
            SetData(type, _currentLevelNumber, "Best", movesCounter);
            AddCompleted(type);
            if (UnlockedCondtion(type))
            {
                UnLockNextType(type);
            }
        }
        else if (highscore > movesCounter)
        {
            SetData(type, _currentLevelNumber, "Best", movesCounter);
        }

        if (owned && GetOwned(type, _currentLevelNumber) != 1)
        {
            SetOwned(type, _currentLevelNumber, 1);
        }
        
        SavePlayerData();
        _currentLevel.QueueFree();

        EmitSignal(nameof(LevelCompleted), _currentLevelNumber, owned, movesCounter, GetData(type, _currentLevelNumber, "Best"));
    }
    public void _on_PauseMenu_button_pressed(string name, PauseMenu pauseMenu)
    {
        pauseMenu.Hide();
        GetTree().Paused = false;
        
        switch (name)
        {
            case "Resume":
                _currentLevel.ScaleModulate(false);
                break;
            case "Quit":
                _currentLevel.QueueFree();
                EmitSignal(nameof(QuitPressed));
                break;
        }
    }

    public void _on_GameManagerAnimationPlayer_animation_finished(string name)
    {
        if (name == "glow")
        {
            if (_animationBackwards)
            {
                _gameManagerAnimationPlayer.Play("glow");
            }
            else
            {
                _gameManagerAnimationPlayer.PlayBackwards("glow");
            }

            _animationBackwards = !_animationBackwards;
            return;
        }

    }
     public void _on_LevelGrid_PlayAnimation()
    {
        _gameManagerAnimationPlayer.Play("RESET");
        _gameManagerAnimationPlayer.Play("glow");
    }
     public void _on_GameEndedMenu_PlayAnimation()
    {
        _gameManagerAnimationPlayer.Play("glow");
    }

    public void ResetAnimation()
    {
        _gameManagerAnimationPlayer.Play("RESET");
        _gameManagerAnimationPlayer.Advance(0);
    }


}



