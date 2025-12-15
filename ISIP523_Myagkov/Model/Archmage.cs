namespace Model
{
    public class Archmage : BossEnemy
    {
        private const double FreezeChance = 0.35;

        public Archmage() : base("Архимаг C++", 20, 12, 1, 1.8, 1.6, 1.1) { }

        public override void SpecialAbility(Player player)
        {
            Console.WriteLine($"\n{Name} накладывает СИЛЬНУЮ заморозку! Вы пропускаете следующий ход.");
            player.IsFrozen = true;
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < FreezeChance;
        }
    }
}