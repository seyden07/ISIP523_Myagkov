namespace Model
{
    public class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(string name, int defense, int value) : base(name, value)
        {
            Defense = defense;
        }
    }
}