using System.Drawing;
using static System.Net.Mime.MediaTypeNames;

namespace Model
{
    public static class EnemyFactory
    {
        public static IEnemy CreateEnemy(EnemyType type)
        {
            return type switch
            {
                EnemyType.Goblin => new Goblin(),
                EnemyType.Skeleton => new Skeleton(),
                EnemyType.Mage => new Mage(),
                EnemyType.Slime => new Slime(),
                _ => throw new ArgumentException("Неизвестный тип врага")
            };
        }

        public static IEnemy CreateRandomEnemy()
        {
            var enemyTypes = new[] { EnemyType.Goblin, EnemyType.Skeleton, EnemyType.Mage, EnemyType.Slime };
            var randomType = enemyTypes[RandomChoice.Next(enemyTypes.Length)];
            return CreateEnemy(randomType);
        }

        public static BossEnemy CreateBoss(BossType type)
        {
            return type switch
            {
                BossType.VVG => new VVG(),
                BossType.Kovalsky => new Kovalsky(),
                BossType.Archmage => new Archmage(),
                BossType.Pestov => new Pestov(),
                _ => throw new ArgumentException("Неизвестный тип босса")
            };
        }

        public static BossEnemy CreateRandomBoss()
        {
            var bossTypes = new[] { BossType.VVG, BossType.Kovalsky, BossType.Archmage, BossType.Pestov };
            var randomType = bossTypes[RandomChoice.Next(bossTypes.Length)];
            return CreateBoss(randomType);
        }
    }

    public enum EnemyType
    {
        Goblin,
        Skeleton,
        Mage,
        Slime
    }

    public enum BossType
    {
        VVG,
        Kovalsky,
        Archmage,
        Pestov
    }
}