namespace Model
{
    public class HealthPotion : Item
    {
        public HealthPotion() : base("Зелье здоровья", 15) { }

        public void Use(Player player)
        {
            player.Heal();
        }
    }
}