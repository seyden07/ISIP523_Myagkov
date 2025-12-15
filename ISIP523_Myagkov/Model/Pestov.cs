namespace Model
{
    public class Pestov : BossEnemy
    {
        private const double FreezeChance = 0.4;

        public Pestov() : base("Пестов С--", 25, 10, 2, 1.3, 1.8, 0.6) { }

        public override void SpecialAbility(Player player)
        {
            Console.WriteLine($"\n{Name} игнорирует защиту И замораживает вас!");
            player.TakeDamage(Attack, true);
            player.IsFrozen = true;
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < FreezeChance;
        }
    }
}