namespace Model
{
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
            return RandomChoice.NextDouble() < 0.3;
        }
    }
}
