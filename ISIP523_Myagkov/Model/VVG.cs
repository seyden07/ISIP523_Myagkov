namespace Model
{
    public class VVG : BossEnemy
    {
        private const double CritChance = 0.3;

        public VVG() : base("ВВГ", 30, 8, 3, 2.0, 1.5, 1.2) { }

        public override void SpecialAbility(Player player)
        {
            int critDamage = (int)(Attack * 2.0);
            Console.WriteLine($"\n{Name} наносит СУПЕР-критический удар! Урон: {critDamage}");
            player.TakeDamage(critDamage);
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < CritChance;
        }
    }
}