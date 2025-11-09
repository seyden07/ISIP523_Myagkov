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

public abstract class Enemy : IEnemy
{
    public string Name { get; protected set; }
    public int Health { get; set; }
    public int Attack { get; protected set; }
    public int Defense { get; protected set; }

    protected Random random;

    protected Enemy(string name, int health, int attack, int defense)
    {
        Name = name;
        Health = health;
        Attack = attack;
        Defense = defense;
        random = new Random();
    }

    public abstract void SpecialAbility(Player player);
    public abstract bool TrySpecialAbility();
}

// Goblin.cs
public class Goblin : Enemy
{
    private double critChance = 0.2;

    public Goblin() : base("Гоблин", 30, 8, 3) { }

    public override void SpecialAbility(Player player)
    {
        int critDamage = (int)(Attack * 1.5);
        Console.WriteLine($"{Name} наносит критический удар! Урон: {critDamage}");
        player.TakeDamage(critDamage);
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < critChance;
    }
}

public class Skeleton : Enemy
{
    public Skeleton() : base("Скелет", 25, 10, 2) { }

    public override void SpecialAbility(Player player)
    {
        Console.WriteLine($"{Name} игнорирует вашу защиту! Урон: {Attack}");
        player.TakeDamage(Attack, true);
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < 0.3;
    }
}

public class Mage : Enemy
{
    private double freezeChance = 0.25;

    public Mage() : base("Маг", 20, 12, 1) { }

    public override void SpecialAbility(Player player)
    {
        Console.WriteLine($"{Name} замораживает вас! Вы пропускаете следующий ход.");
        player.IsFrozen = true;
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < freezeChance;
    }
}