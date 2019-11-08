using SQLite;

namespace DataAccessLibrary.Entitites
{
    public class Miror : IEntity
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string MAC { get; set; }
        public bool IsPaired { get; set; }
        public string Usermail { get; set; }
    }
}
