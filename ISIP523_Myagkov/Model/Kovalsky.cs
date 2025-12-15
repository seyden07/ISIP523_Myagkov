namespace Model
{
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
            return RandomChoice.NextDouble() < 0.4;
        }
    }
}