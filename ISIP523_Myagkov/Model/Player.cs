namespace Model
{
    public class Player
    {
        public int Health { get; set; }
        public int MaxHealth { get; private set; }
        public Weapon CurrentWeapon { get; set; }
        public Armor CurrentArmor { get; set; }
        public bool IsFrozen { get; set; }
        public int TurnCount { get; set; }

        public Player()
        {
            MaxHealth = 100;
            Health = MaxHealth;
            CurrentWeapon = new Weapon("Кулаки", 5, 0);
            CurrentArmor = new Armor("Одежда", 2, 0);
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
            return RandomChoice.NextDouble() < 0.4;
        }

        public int CalculateBlock()
        {
            int baseDefense = CurrentArmor?.Defense ?? 2;
            return (int)(baseDefense * (0.7 + RandomChoice.NextDouble() * 0.3));
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
}