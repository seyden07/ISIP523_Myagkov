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

public class Goblin : Enemy
{
    private double critChance = 0.2;

    public Goblin() : base("Гоблин", 30, 8, 3) { }

    public override void SpecialAbility(Player player)
    {
        int critDamage = (int)(Attack * 1.5);
        Console.WriteLine($"\n{Name} наносит критический удар! Урон: {critDamage}");
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
        Console.WriteLine($"\n{Name} игнорирует вашу защиту! Урон: {Attack}");
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
        Console.WriteLine($"\n{Name} замораживает вас! Вы пропускаете следующий ход.");
        player.IsFrozen = true;
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < freezeChance;
    }
}

public abstract class BossEnemy : Enemy
{
    protected BossEnemy(string name, int baseHealth, int baseAttack, int baseDefense,
                       double healthMultiplier, double attackMultiplier, double defenseMultiplier)
        : base(name,
              (int)(baseHealth * healthMultiplier),
              (int)(baseAttack * attackMultiplier),
              (int)(baseDefense * defenseMultiplier))
    {
    }
}

public class VVG : BossEnemy
{
    private double critChance = 0.3;

    public VVG() : base("ВВГ", 30, 8, 3, 2.0, 1.5, 1.2) { }

    public override void SpecialAbility(Player player)
    {
        int critDamage = (int)(Attack * 2.0);
        Console.WriteLine($"\n{Name} наносит СУПЕР-критический удар! Урон: {critDamage}");
        player.TakeDamage(critDamage);
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < critChance;
    }
}

public class Kovalsky : BossEnemy
{
    public Kovalsky() : base("Ковальский", 25, 10, 2, 2.5, 1.3, 1.4) { }

    public override void SpecialAbility(Player player)
    {
        Console.WriteLine($"\n{Name} полностью игнорирует вашу защиту! Урон: {Attack}");
        player.TakeDamage(Attack, true);
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < 0.4;
    }
}

public class Archmage : BossEnemy
{
    private double freezeChance = 0.35;

    public Archmage() : base("Архимаг C++", 20, 12, 1, 1.8, 1.6, 1.1) { }

    public override void SpecialAbility(Player player)
    {
        Console.WriteLine($"\n{Name} накладывает СИЛЬНУЮ заморозку! Вы пропускаете следующий ход.");
        player.IsFrozen = true;
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < freezeChance;
    }
}

public class Pestov : BossEnemy
{
    private double freezeChance = 0.4;

    public Pestov() : base("Пестов С--", 25, 10, 2, 1.3, 1.8, 0.6) { }

    public override void SpecialAbility(Player player)
    {
        Console.WriteLine($"\n{Name} игнорирует защиту И замораживает вас!");
        player.TakeDamage(Attack, true);
        player.IsFrozen = true;
    }

    public override bool TrySpecialAbility()
    {
        return random.NextDouble() < freezeChance;
    }
}

public class Player
{
    public int Health { get; set; }
    public int MaxHealth { get; private set; }
    public Weapon CurrentWeapon { get; set; }
    public Armor CurrentArmor { get; set; }
    public bool IsFrozen { get; set; }
    public int TurnCount { get; set; }

    private Random random;

    public Player()
    {
        MaxHealth = 100;
        Health = MaxHealth;
        CurrentWeapon = new Weapon("Кулаки", 5, 0);
        CurrentArmor = new Armor("Одежда", 2, 0);
        random = new Random();
    }

    public void TakeDamage(int damage, bool ignoreArmor = false)
    {
        int actualDamage = damage;

        if (!ignoreArmor && CurrentArmor != null)
        {
            actualDamage = Math.Max(1, damage - CurrentArmor.Defense);
        }

        Health -= actualDamage;
        Console.WriteLine($"\nВы получаете {actualDamage} урона. Здоровье: {Health}/{MaxHealth}");
    }

    public void Heal()
    {
        Health = MaxHealth;
        Console.WriteLine($"\nВы полностью исцелены! Здоровье: {Health}/{MaxHealth}");
    }

    public int CalculateAttack()
    {
        int baseAttack = CurrentWeapon?.Attack ?? 5;
        return baseAttack;
    }

    public bool TryDodge()
    {
        return random.NextDouble() < 0.4;
    }

    public int CalculateBlock()
    {
        int baseDefense = CurrentArmor?.Defense ?? 2;
        return (int)(baseDefense * (0.7 + random.NextDouble() * 0.3));
    }

    public void DisplayStatus()
    {
        Console.WriteLine($"\n=== СТАТУС ИГРОКА ===");
        Console.WriteLine($"Здоровье: {Health}/{MaxHealth}");
        Console.WriteLine($"Оружие: {CurrentWeapon.Name} (Атака: {CurrentWeapon.Attack})");
        Console.WriteLine($"Доспехи: {CurrentArmor.Name} (Защита: {CurrentArmor.Defense})");
        Console.WriteLine($"Ход: {TurnCount}");
    }
}

public static class ItemGenerator
{
    private static Random random = new Random();

    private static readonly Weapon[] Weapons = new[]
    {
        new Weapon("Деревянный меч", 8, 10),
        new Weapon("Железный меч", 12, 25),
        new Weapon("Стальной меч", 16, 40),
        new Weapon("Магический посох", 14, 35),
        new Weapon("Легендарный клинок", 20, 60)
    };

    private static readonly Armor[] Armors = new[]
    {
        new Armor("Кожаная броня", 4, 15),
        new Armor("Кольчуга", 6, 30),
        new Armor("Латная броня", 8, 45),
        new Armor("Магические доспехи", 7, 40),
        new Armor("Легендарные доспехи", 10, 55)
    };

    public static Item GenerateRandomItem()
    {
        int itemType = random.Next(3);

        switch (itemType)
        {
            case 0: 
                return Weapons[random.Next(Weapons.Length)];
            case 1: 
                return Armors[random.Next(Armors.Length)];
            case 2: 
                return new HealthPotion();
            default:
                return new HealthPotion();
        }
    }
}

public class HealthPotion : Item
{
    public HealthPotion() : base("Зелье здоровья", 15) { }

    public void Use(Player player)
    {
        player.Heal();
    }
}

public class Game
{
    private Player player;
    private Random random;
    private bool gameRunning;

    private readonly Enemy[] normalEnemies;
    private readonly BossEnemy[] bossEnemies;

    public Game()
    {
        player = new Player();
        random = new Random();
        gameRunning = true;

        normalEnemies = new Enemy[]
        {
            new Goblin(),
            new Skeleton(),
            new Mage()
        };

        bossEnemies = new BossEnemy[]
        {
            new VVG(),
            new Kovalsky(),
            new Archmage(),
            new Pestov()
        };
    }

    public void Start()
    {
        Console.WriteLine("\nДобро пожаловать в текстовый рогалик!");
        Console.WriteLine("Ваша цель - выживать как можно дольше...\n");

        while (gameRunning && player.Health > 0)
        {
            player.TurnCount++;
            Console.WriteLine($"\n--- Ход {player.TurnCount} ---");

            if (player.IsFrozen)
            {
                Console.WriteLine("\nВы заморожены и пропускаете ход!");
                player.IsFrozen = false;
                continue;
            }

            if (player.TurnCount % 10 == 0)
            {
                Console.WriteLine("\n * * * ПОЯВИЛСЯ БОСС! * * *");
                Enemy boss = bossEnemies[random.Next(bossEnemies.Length)];
                FightEnemy(boss);
            }
            else
            {
                if (random.Next(2) == 0)
                {
                    FindChest();
                }
                else
                {
                    Enemy enemy = normalEnemies[random.Next(normalEnemies.Length)];
                    FightEnemy(enemy);
                }
            }

            if (player.Health <= 0)
            {
                GameOver();
                break;
            }
        }
    }

    private void FindChest()
    {
        Console.WriteLine("\nВы нашли сундук!");
        Item item = ItemGenerator.GenerateRandomItem();

        if (item is HealthPotion potion)
        {
            Console.WriteLine("\nВы нашли Зелье здоровья!");
            potion.Use(player);
        }
        else if (item is Weapon weapon)
        {
            HandleWeaponChoice(weapon);
        }
        else if (item is Armor armor)
        {
            HandleArmorChoice(armor);
        }
    }

    private void HandleWeaponChoice(Weapon newWeapon)
    {
        Console.WriteLine($"\nВы нашли новое оружие: {newWeapon.Name}");
        Console.WriteLine($"Атака: {newWeapon.Attack} (текущее: {player.CurrentWeapon.Attack})");

        Console.Write("\nВзять новое оружие? (y/n): ");
        string choice = Console.ReadLine()?.ToLower();

        if (choice == "y" || choice == "д")
        {
            player.CurrentWeapon = newWeapon;
            Console.WriteLine($"\nВы экипировали: {newWeapon.Name}");
        }
        else
        {
            Console.WriteLine("\nВы оставили оружие в сундуке.");
        }
    }

    private void HandleArmorChoice(Armor newArmor)
    {
        Console.WriteLine($"\nВы нашли новые доспехи: {newArmor.Name}");
        Console.WriteLine($"Защита: {newArmor.Defense} (текущая: {player.CurrentArmor.Defense})");

        Console.Write("\nВзять новые доспехи? (y/n): ");
        string choice = Console.ReadLine()?.ToLower();

        if (choice == "y" || choice == "д")
        {
            player.CurrentArmor = newArmor;
            Console.WriteLine($"\nВы экипировали: {newArmor.Name}");
        }
        else
        {
            Console.WriteLine("\nВы оставили доспехи в сундуке.");
        }
    }

    private void FightEnemy(Enemy enemy)
    {
        Console.WriteLine($"\nВы встретили {enemy.Name}!");
        Console.WriteLine($"Здоровье врага: {enemy.Health}");

        while (enemy.Health > 0 && player.Health > 0)
        {
            PlayerTurn(enemy);
            if (enemy.Health <= 0) break;

            EnemyTurn(enemy);
            if (player.Health <= 0) break;
        }

        if (enemy.Health <= 0)
        {
            Console.WriteLine($"\nВы победили {enemy.Name}!");
        }
    }

    private void PlayerTurn(Enemy enemy)
    {
        Console.WriteLine("\n--- Ваш ход ---");
        Console.WriteLine("1. Атаковать");
        Console.WriteLine("2. Защищаться");
        Console.Write("Выберите действие: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                int playerDamage = player.CalculateAttack();
                enemy.Health -= playerDamage;
                Console.WriteLine($"\nВы атакуете {enemy.Name} и наносите {playerDamage} урона!");
                Console.WriteLine($"\nЗдоровье {enemy.Name}: {Math.Max(0, enemy.Health)}");
                break;

            case "2":
                Console.WriteLine("\nВы готовитесь к защите...");
                break;

            default:
                Console.WriteLine("\nНеверный выбор, вы пропускаете ход!");
                break;
        }
    }

    private void EnemyTurn(Enemy enemy)
    {
        Console.WriteLine($"\n--- Ход {enemy.Name} ---");

        bool isDefending = false;
        int blockAmount = 0;


        if (enemy.TrySpecialAbility())
        {
            enemy.SpecialAbility(player);
        }
        else
        {
            int enemyDamage = enemy.Attack;

            if (isDefending)
            {
                if (player.TryDodge())
                {
                    Console.WriteLine("\nВы уворачиваетесь от атаки!");
                    return;
                }
                else
                {
                    blockAmount = player.CalculateBlock();
                    enemyDamage = Math.Max(1, enemyDamage - blockAmount);
                    Console.WriteLine($"\nВы блокируете {blockAmount} урона!");
                }
            }

            player.TakeDamage(enemyDamage);
        }
    }

    private void GameOver()
    {
        Console.WriteLine("\n * * * ИГРА ОКОНЧЕНА! * * * ");
        Console.WriteLine($"Вы продержались {player.TurnCount} ходов");
        player.DisplayStatus();
        gameRunning = false;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        try
        {
            Game game = new Game();
            game.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}