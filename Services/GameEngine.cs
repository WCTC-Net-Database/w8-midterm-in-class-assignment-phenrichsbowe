using W8_assignment_template.Data;
using W8_assignment_template.Helpers;
using W8_assignment_template.Interfaces;
using W8_assignment_template.Models.Characters;

namespace W8_assignment_template.Services;

public class GameEngine
{
    private readonly IContext _context;
    private readonly MapManager _mapManager;
    private readonly MenuManager _menuManager;
    private readonly OutputManager _outputManager;
    private readonly IRoomFactory _roomFactory;
    private ICharacter _player;
    private ICharacter _goblin;
    private ICharacter _wraith;
    private ICharacter _troll;

    private List<IRoom> _rooms;

    public GameEngine(IContext context, IRoomFactory roomFactory, MenuManager menuManager, MapManager mapManager, OutputManager outputManager)
    {
        _roomFactory = roomFactory;
        _menuManager = menuManager;
        _mapManager = mapManager;
        _outputManager = outputManager;
        _context = context;
    }

    public void Run()
    {
        if (_menuManager.ShowMainMenu())
        {
            SetupGame();
        }
    }

    private void AttackCharacter()
    {   
        var currentRoom = _player.CurrentRoom;
        var selectedCharacter = _player.CurrentRoom.Characters.FirstOrDefault(c => c != _player);

        if (currentRoom.Characters.Count >= 2)
        {
            _outputManager.WriteLine("Select a character from the list to attack:");

            for (int characterIndex = 0; characterIndex < currentRoom.Characters.Count; characterIndex++)
            {
                _outputManager.WriteLine($"{characterIndex}: {currentRoom.Characters[characterIndex].Name}");
            }

            _outputManager.Display();

            int selectedCharacterIndex;

            while (true)
            {
                string? input = Console.ReadLine();

                if (int.TryParse(input, out selectedCharacterIndex) && selectedCharacterIndex <= currentRoom.Characters.Count)
                {
                    break;
                }

                _outputManager.WriteLine("Invalid choice. Please select a valid character number.");
                _outputManager.Display();
            }

            selectedCharacter = currentRoom.Characters[selectedCharacterIndex];
        }

        _player.Attack(selectedCharacter);
    }

    private void GameLoop()
    {
        while (true)
        {
            _mapManager.DisplayMap();
            _outputManager.WriteLine("Choose an action:", ConsoleColor.Cyan);
            _outputManager.WriteLine("1. Move North");
            _outputManager.WriteLine("2. Move South");
            _outputManager.WriteLine("3. Move East");
            _outputManager.WriteLine("4. Move West");

            // Check if there are characters in the current room to attack
            if (_player.CurrentRoom.Characters.Any(c => c != _player))
            {
                _outputManager.WriteLine("5. Attack");
            }

            _outputManager.WriteLine("6. Exit Game");

            _outputManager.Display();

            var input = Console.ReadLine();

            string? direction = null;
            switch (input)
            {
                case "1":
                    direction = "north";
                    break;
                case "2":
                    direction = "south";
                    break;
                case "3":
                    direction = "east";
                    break;
                case "4":
                    direction = "west";
                    break;
                case "5":
                    if (_player.CurrentRoom.Characters.Any(c => c != _player))
                    {
                        AttackCharacter();
                    }
                    else
                    {
                        _outputManager.WriteLine("No characters to attack.", ConsoleColor.Red);
                    }

                    break;
                case "6":
                    _outputManager.WriteLine("Exiting game...", ConsoleColor.Red);
                    _outputManager.Display();
                    Environment.Exit(0);
                    break;
                default:
                    _outputManager.WriteLine("Invalid selection. Please choose a valid option.", ConsoleColor.Red);
                    break;
            }

            // Update map manager with the current room after movement
            if (!string.IsNullOrEmpty(direction))
            {
                _outputManager.Clear();
                _player.Move(direction);
                _mapManager.UpdateCurrentRoom(_player.CurrentRoom);
            }
        }
    }

    private void LoadMonsters()
    {
        _goblin = _context.Characters.OfType<Goblin>().FirstOrDefault();
        _wraith = _context.Characters.OfType<Wraith>().FirstOrDefault();
        _troll = _context.Characters.OfType<Troll>().FirstOrDefault();

        var random = new Random();
        var randomRoom = _rooms[random.Next(_rooms.Count)];

        randomRoom.AddCharacter(_goblin);
        randomRoom.AddCharacter(_wraith);
        randomRoom.AddCharacter(_troll);
    }

    private void SetupGame()
    {
        var startingRoom = SetupRooms();
        _mapManager.UpdateCurrentRoom(startingRoom);

        _player = _context.Characters.OfType<Player>().FirstOrDefault();
        _player.Move(startingRoom);
        _outputManager.WriteLine($"{_player.Name} has entered the game.", ConsoleColor.Green);

        // Load monsters into random rooms 
        LoadMonsters();

        // Pause for a second before starting the game loop
        Thread.Sleep(1000);
        GameLoop();
    }

    private IRoom SetupRooms()
    {
        // Create existing rooms
        var entrance = _roomFactory.CreateRoom("entrance", _outputManager);
        var treasureRoom = _roomFactory.CreateRoom("treasure", _outputManager);
        var dungeonRoom = _roomFactory.CreateRoom("dungeon", _outputManager);
        var library = _roomFactory.CreateRoom("library", _outputManager);
        var armory = _roomFactory.CreateRoom("armory", _outputManager);
        var garden = _roomFactory.CreateRoom("garden", _outputManager);

        // Create new rooms
        var laboratory = _roomFactory.CreateRoom("laboratory", _outputManager);
        var crypt = _roomFactory.CreateRoom("crypt", _outputManager);

        // Connect existing rooms
        entrance.North = treasureRoom;
        entrance.West = library;
        entrance.East = garden;

        treasureRoom.South = entrance;
        treasureRoom.West = dungeonRoom;

        dungeonRoom.East = treasureRoom;

        library.East = entrance;
        library.South = armory;

        armory.North = library;

        garden.West = entrance;

        // Connect new rooms
        library.North = laboratory;  // Connect library to laboratory
        laboratory.South = library;  // Connect laboratory back to library

        dungeonRoom.North = crypt;    // Connect dungeon to crypt
        crypt.South = dungeonRoom;     // Connect crypt back to dungeon

        // Store rooms in a list for later use
        _rooms = new List<IRoom>
        {
            entrance,
            treasureRoom,
            dungeonRoom,
            library,
            armory,
            garden,
            laboratory,
            crypt
        };

        return entrance;
    }

}
