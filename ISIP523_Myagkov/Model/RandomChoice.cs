namespace Model
{
    public static class RandomChoice
    {
        private static Random random = new Random();

        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }
    }
}
