namespace stonevox
{
    public class PictureData : AppearenceData
    {
        public string FilePath { get; set; }

        public PictureData() { }

        public PictureData(string filepath)
        {
            this.FilePath = filepath;
        }

        public override Appearence ToAppearence()
        {
            return new Picture(FilePath);
        }
    }
}
