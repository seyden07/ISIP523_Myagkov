using System.Numerics;

namespace Model
{
    public interface IEnemy
    {
        string Name { get; }
        int Health { get; set; }
        int Attack { get; }
        int Defense { get; }
        void SpecialAbility(Player player);
        bool TrySpecialAbility();
    }

    public abstract class Enemy : IEnemy
    {
        public string Name { get; protected set; }
        public int Health { get; set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }

        protected Enemy(string name, int health, int attack, int defense)
        {
            Name = name;
            Health = health;
            Attack = attack;
            Defense = defense;
        }

        public abstract void SpecialAbility(Player player);
        public abstract bool TrySpecialAbility();
    }
}