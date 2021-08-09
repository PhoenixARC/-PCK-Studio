namespace stonevox
{
    public abstract class AppearenceData
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }

        public AppearenceData()
        {
            Name = "";
        }

        public abstract Appearence ToAppearence();
    }
}
