using Godot;
using System.Linq;


public class Main : Control
{
    private GameManager _gameManager;
    private MenuTemplates _menu;
    private PackedScene _mainMenuScene;
    private PackedScene _optionsMenuScene;
    private PackedScene _levelTypeMenuScene;
    private PackedScene _levelGridScene;
    private PackedScene _gameEndedMenuscene;


    public override void _Ready()
    {
        _gameManager = GetNode<GameManager>("GameManager");
        _mainMenuScene = ResourceLoader.Load<PackedScene>("res://scene/menus/MainMenu.tscn");
        _optionsMenuScene = ResourceLoader.Load<PackedScene>("res://scene/menus/OptionsMenu.tscn");
        _levelTypeMenuScene = ResourceLoader.Load<PackedScene>("res://scene/menus/LevelTypeMenu.tscn");
        _levelGridScene = ResourceLoader.Load<PackedScene>("res://scene/menus/LevelGrid.tscn");
        _gameEndedMenuscene = ResourceLoader.Load<PackedScene>("res://scene/menus/GameEndedMenu.tscn");
        //_levelPauseMenu = ResourceLoader.Load<PackedScene>();

        AddMenu(_mainMenuScene, "MainMenu");
    }


    public void AddMenu(PackedScene scene, string name = null)
    {
        MenuTemplates menu = scene.Instance<MenuTemplates>();
        AddChild(menu);
        menu.Name = name;
        menu.ConnectButtons();
        _menu = menu;
    }
    public void GoToMainMenu()
    {
        _menu.QueueFree();
        AddMenu(_mainMenuScene, "MainMenu");
    }
    public void GoToLevel(int level)
    {
        _menu.QueueFree();
        _gameManager.LoadLevel(level);
    }
    public void GoToLevelMenu()
    {
        _menu.QueueFree();
        AddMenu(_levelTypeMenuScene, "LevelTypeMenu");
    }
    public void GoToLevelGrid()
    {
        _menu.QueueFree();
        AddMenu(_levelGridScene, "LevelGrid");
    }
    public void GoToOptionsMenu()
    {
        _menu.QueueFree();
        AddMenu(_optionsMenuScene, "OptionsMenu");
    }
    public void GoToGameEndedMenu(int level, bool owned, int moves, int best)
    {
        GameEndedMenu menu = _gameEndedMenuscene.Instance<GameEndedMenu>();
        menu.Init(level, owned, moves, best);
        AddChild(menu);
        menu.Name = "GameEndedMenu";
        menu.ConnectButtons();
        _menu = menu;
    }


    public async void _on_MainMenu_button_pressed(string name)
    {
        MainMenu menu = (MainMenu)_menu;
        menu.StartAnimation(name);
        await ToSignal(menu, "AnimationFinished");

        switch (name)
        {
            case "Play":
                GoToLevelMenu();
                break;
            case "Options":
                GoToOptionsMenu();
                break;
        }
    }
    public async void _on_OptionsMenu_button_pressed(string name)
    {
        switch (name)
        {
            case "Return":
                GoToMainMenu();
                break;
            case "Reset":
                _gameManager.LoadDefaultPlayerData();
                _gameManager.SavePlayerData();

                _menu.Hide();
                await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
                _menu.Show();
                break;
        }
    }
    public void _on_LevelTypeMenu_button_pressed(string name)
    {

        switch (name)
        {
            case "Easy":
                if (_gameManager.IsLevelUnlocked(name))
                {
                    GoToLevelGrid();
                }
                break;
            case "Medium":
                if (_gameManager.IsLevelUnlocked(name))
                {
                    GoToLevelGrid();
                }
                break;
            case "Hard":
                if (_gameManager.IsLevelUnlocked(name))
                {
                    GoToLevelGrid();
                }
                break;
            case "Return":
                GoToMainMenu();
                break;
        }

    }
    public void _on_LevelGrid_button_pressed(string name)
    {
        if (name[0] == 'L')
        {
            string number = name.Split('l').Last();
            GoToLevel(number.ToInt());
            return;
        }
        switch (name)
        {
            case "Return":
                GoToLevelMenu();
                break;
        }
    }
    public void _on_GameManager_LevelCompleted(int level, bool owned, int moves, int best)
    {
        GoToGameEndedMenu(level, owned, moves, best);
    }
    public void _on_GameEndedMenu_button_pressed(string name)
    {

        switch (name)
        {
            case "Continue":
                int maxLevel = _gameManager.MaxLevel[_gameManager.CurrentLevelType];
                if (_gameManager.CurrentLevelNumber < maxLevel - 1)
                {
                    _menu.QueueFree();
                    _gameManager.NextLevel();
                }
                else
                {
                    GoToLevelMenu();
                }


                break;
            case "Retry":
                GoToLevel(_gameManager.CurrentLevelNumber);
                break;
            case "Return":
                GoToMainMenu();
                break;
        }

    }
   
    public void _on_GameManager_QuitPressed()
    {
        AddMenu(_mainMenuScene, "MainMenu");
    }

}


