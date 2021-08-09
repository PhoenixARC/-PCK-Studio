namespace stonevox
{
    public class PlainBackgroundData : AppearenceData
    {
        public ColorData Color { get; set; }

        public PlainBackgroundData()
        {
            Color = new ColorData();
        }

        public override Appearence ToAppearence()
        {
            return new PlainBackground(Color);
        }
    }
}
