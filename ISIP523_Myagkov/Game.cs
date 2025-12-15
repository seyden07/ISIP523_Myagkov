using Model;

public class Game
{
    private Player player;
    private bool gameRunning;

    public Game()
    {
        player = new Player();
        gameRunning = true;
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
                BossEnemy boss = EnemyFactory.CreateRandomBoss();
                FightEnemy(boss);
            }
            else
            {
                if (RandomChoice.Next(2) == 0)
                {
                    FindChest();
                }
                else
                {
                    IEnemy enemy = EnemyFactory.CreateRandomEnemy();
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

    private void FightEnemy(IEnemy enemy)
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

    private void PlayerTurn(IEnemy enemy)
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

                if (enemy is Slime && RandomChoice.NextDouble() < 0.25)
                {
                    playerDamage = Math.Max(1, playerDamage - 2);
                    Console.WriteLine($"\n{enemy.Name} уменьшил ваш урон на 2 единицы!");
                }

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

    private void EnemyTurn(IEnemy enemy)
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