using System.Numerics;

public abstract class Item
{
    public string Name { get; protected set; }
    public int Value { get; protected set; }

    protected Item(string name, int value)
    {
        Name = name;
        Value = value;
    }
}

public class Weapon : Item
{
    public int Attack { get; private set; }

    public Weapon(string name, int attack, int value) : base(name, value)
    {
        Attack = attack;
    }
}

public class Armor : Item
{
    public int Defense { get; private set; }

    public Armor(string name, int defense, int value) : base(name, value)
    {
        Defense = defense;
    }
}

public interface IEnemy
{
    string Name { get; }
    int Health { get; set; }
    int Attack { get; }
    int Defense { get; }
    void SpecialAbility(Player player);
    bool TrySpecialAbility();
}