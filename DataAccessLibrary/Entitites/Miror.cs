using SQLite;
using SQLiteNetExtensions.Attributes;

namespace DataAccessLibrary.Entitites
{
    public class Miror : IEntity
    {
        [PrimaryKey]
        public long Id { get; set; }
        public bool IsLinked { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }
        public long UserParentId { get; set; }
        [ManyToOne]
        public virtual User UserParent { get; set; }
        public bool IsVolatile { get; set; }
    }
}
