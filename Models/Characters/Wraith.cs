using W8_assignment_template.Interfaces;

namespace W8_assignment_template.Models.Characters;

public class Wraith : CharacterBase, IFlyable, ILootable
{
  public string Treasure { get; set; }

  public Wraith()
  {
  }

  public Wraith(string name, string type, int level, int hp, string treasure, IRoom startingRoom)
      : base(name, type, level, hp, startingRoom)
  {
    Treasure = treasure;
  }

  public void Fly()
  {
    OutputManager.WriteLine($"{Name} glides silently through the shadows.", ConsoleColor.DarkGray);
  }

  public override void UniqueBehavior()
  {
    OutputManager.WriteLine($"{Name} drains the life force from its surroundings.", ConsoleColor.Red);
  }
}
