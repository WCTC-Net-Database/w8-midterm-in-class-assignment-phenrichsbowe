using W8_assignment_template.Helpers;
using W8_assignment_template.Interfaces;

namespace W8_assignment_template.Models.Rooms;

public class Room : IRoom
{
    private readonly OutputManager _outputManager;
    public List<ICharacter> Characters { get; }
    public string Name { get; }
    public string Description { get; }
    public IRoom? North { get; set; }
    public IRoom? South { get; set; }
    public IRoom? West { get; set; }
    public IRoom? East { get; set; }

    public Room(string name, string description, OutputManager outputManager)
    {
        Name = name;
        Description = description;
        _outputManager = outputManager;
        Characters = new List<ICharacter>();
    }

    public void Enter()
    {
        _outputManager.WriteLine($"You have entered {Name}. {Description}", ConsoleColor.Green);
        foreach (var character in Characters)
        {
            _outputManager.WriteLine($"{character.Name} is here.", ConsoleColor.Red);
        }
    }

    // The following methods exist to add and remove characters from the room (e.g. if they are defeated)
    public void AddCharacter(ICharacter character)
    {
        Characters.Add(character);
        _outputManager.WriteLine($"INFO: {character.Name} has been added to room {Name}", ConsoleColor.DarkGray);
    }

    public void RemoveCharacter(ICharacter character)
    {
        Characters.Remove(character);
        _outputManager.WriteLine($"INFO: {character.Name} has been removed from room {Name}.", ConsoleColor.DarkGray);
    }

}
