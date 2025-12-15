namespace Model
{
    public class Goblin : Enemy
    {
        private const double CritChance = 0.2;

        public Goblin() : base("Гоблин", 30, 8, 3) { }

        public override void SpecialAbility(Player player)
        {
            int critDamage = (int)(Attack * 1.5);
            Console.WriteLine($"\n{Name} наносит критический удар! Урон: {critDamage}");
            player.TakeDamage(critDamage);
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < CritChance;
        }
    }
}
