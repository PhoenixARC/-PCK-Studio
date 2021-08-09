using System;
using System.Collections.Generic;

namespace stonevox
{
    public class WidgetAppearence
    {
        private Widget owner;

        Dictionary<string, Appearence> appearences;

        public int Count { get { return appearences.Count; } }

        public WidgetAppearence(Widget owner)
        {
            this.owner = owner;
            appearences = new Dictionary<string, Appearence>();
        }

        public Appearence this[string name]
        {
            get { return appearences[name]; }
        }

        public T Get<T>(string name) where T : Appearence 
        {
            return appearences[name] as T;
        }

        public Appearence AddAppearence(string name, Appearence appearence)
        {
            appearence.Owner = owner;
            appearence.Initialize();
            appearences.Add(name, appearence);
            return appearence;
        }

        public T AddAppearence<T>(string name, T appearence) where T : Appearence
        {
            appearence.Owner = owner;
            appearence.Initialize();
            appearences.Add(name, appearence);
            return appearence;
        }

        public T Add<T>(string name) where T : Appearence
        {
            T Type = Activator.CreateInstance<T>();
            Type.Owner = owner;
            Type.Initialize();
            appearences.Add(name, Type);
            return Type;
        }

        public void Remove(string name)
        {
            appearences.Remove(name);
        }

        public void Clear()
        {
            appearences.Clear();
        }

        public void Render(float x, float y, float width, float height)
        {
            foreach (var app in appearences.Values)
            {
                if (app.Enabled)
                    app.Render(x, y, width, height);
            }
        }

        public void Foreach(Action<Appearence> appFunc)
        {
            foreach (var app in appearences.Values)
            {
                appFunc(app);
            }
        }
    }
}
