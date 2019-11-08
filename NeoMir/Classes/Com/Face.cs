namespace NeoMir.Classes.Com
{
    public class Face
    {
        public string Name { get; private set; }
        public bool IsConsumed { get; set; }

        public Face(string name)
        {
            Name = name;
            IsConsumed = false;
        }
    }
}
