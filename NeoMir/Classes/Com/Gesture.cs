namespace NeoMir.Classes.Com
{
    public class Gesture
    {
        public string Name { get; private set; }
        public bool IsConsumed { get; set; }

        public Gesture(string name)
        {
            Name = name;
            IsConsumed = false;
        }
    }
}