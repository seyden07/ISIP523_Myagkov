namespace Model
{
    public abstract class BossEnemy : Enemy
    {
        protected BossEnemy(string name, int baseHealth, int baseAttack, int baseDefense,
                           double healthMultiplier, double attackMultiplier, double defenseMultiplier)
            : base(name,
                  (int)(baseHealth * healthMultiplier),
                  (int)(baseAttack * attackMultiplier),
                  (int)(baseDefense * defenseMultiplier))
        {
        }
    }
}