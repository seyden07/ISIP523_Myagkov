namespace Model
{
    public class Slime : Enemy
    {
        public Slime() : base("Слизень", 35, 7, 1) { }

        public override void SpecialAbility(Player player)
        {
            Console.WriteLine($"\n{Name} поглощает урон, уменьшая его на 2 единицы!");
        }

        public override bool TrySpecialAbility()
        {
            return RandomChoice.NextDouble() < 0.25;
        }
    }
}
