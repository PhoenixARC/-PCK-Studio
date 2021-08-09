namespace stonevox
{
    public abstract class Appearence
    {
        public virtual bool Enabled { get; set; }
        public Widget Owner;

        public Appearence()
        {
            Enabled = true;
        }

        public abstract void Initialize();

        public abstract void Render(float x, float y, float width, float height);

        public abstract Appearence FromData(AppearenceData data);
        public abstract AppearenceData ToData();
    }
}
