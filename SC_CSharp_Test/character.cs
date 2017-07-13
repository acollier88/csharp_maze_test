namespace SC_CSharp_Test
{
    public class Character
    {
        public int maxLives { get; set; }
        public int deathCounter { get; set; }

        public Character()
        {
            this.maxLives = 3;
            this.deathCounter = 0;
        }

        public Character(int maxLives)
        {
            this.maxLives = maxLives;
            this.deathCounter = 0;
        }

    }
}