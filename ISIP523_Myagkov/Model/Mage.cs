namespace Model
{
    public class Mage : Enemy
    {
        private const double FreezeChance = 0.25;

        public Mage() : base("Маг", 20, 12, 1) { }

        public override void SpecialAbility(Player player)
        {
            Console.WriteLine($"\n{Name} замораживает вас! Вы пропускаете следующий ход.");
            player.IsFrozen = true;
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < FreezeChance;
        }
    }
}
