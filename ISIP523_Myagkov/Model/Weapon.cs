namespace Model
{
    public class Weapon : Item
    {
        public int Attack { get; private set; }

        public Weapon(string name, int attack, int value) : base(name, value)
        {
            Attack = attack;
        }
    }
}