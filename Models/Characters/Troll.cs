using W8_assignment_template.Interfaces;


namespace W8_assignment_template.Models.Characters;

public class Troll : CharacterBase, ILootable
{
  public string Treasure { get; set; }

  public Troll()
  {
  }

  public Troll(string name, string type, int level, int hp, string treasure, IRoom startingRoom): base(name, type, level, hp, startingRoom)
  {
    Treasure = treasure;
  }

  public void Smash()
  {
    OutputManager.WriteLine($"{Name} smashes the ground with incredible force!", ConsoleColor.Green);
  }

  public override void UniqueBehavior()
  {
    OutputManager.WriteLine($"{Name} regenerates health over time.", ConsoleColor.Yellow);
  }
}
