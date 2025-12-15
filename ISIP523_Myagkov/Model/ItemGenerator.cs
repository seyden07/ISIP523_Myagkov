namespace Model
{
    public static class ItemGenerator
    {
        private static readonly Weapon[] Weapons = new[]
        {
            new Weapon("Деревянный меч", 8, 10),
            new Weapon("Железный меч", 12, 25),
            new Weapon("Стальной меч", 16, 40),
            new Weapon("Магический посох", 14, 35),
            new Weapon("Легендарный клинок", 20, 60)
        };

        private static readonly Armor[] Armors = new[]
        {
            new Armor("Кожаная броня", 4, 15),
            new Armor("Кольчуга", 6, 30),
            new Armor("Латная броня", 8, 45),
            new Armor("Магические доспехи", 7, 40),
            new Armor("Легендарные доспехи", 10, 55)
        };

        public static Item GenerateRandomItem()
        {
            int itemType = RandomChoice.Next(3);

            return itemType switch
            {
                0 => Weapons[RandomChoice.Next(Weapons.Length)],
                1 => Armors[RandomChoice.Next(Armors.Length)],
                2 => new HealthPotion(),
                _ => new HealthPotion()
            };
        }
    }
}