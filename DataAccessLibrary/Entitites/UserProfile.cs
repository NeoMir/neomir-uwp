using SQLite;

namespace DataAccessLibrary.Entitites
{
    public class UserProfile : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Face { get; set; }
        public string FaceLink { get; set; }
        public bool IsFaceLinked { get; set; }
    }
}
