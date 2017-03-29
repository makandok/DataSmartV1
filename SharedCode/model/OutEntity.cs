namespace JhpDataSystem.model
{
    [SQLite.Table(Constants.KIND_OUTTRANSPORT)]
    public class OutEntity
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }
        public string DataBlob { get; set; }
    }

    [SQLite.Table(Constants.KIND_FAILEDOUTTRANSPORT)]
    public class OutEntityUnsynced
    {
        [SQLite.PrimaryKey]
        public string Id { get; set; }
        public string DataBlob { get; set; }
        public OutEntityUnsynced load(OutEntity initial)
        {
            Id = initial.Id;
            DataBlob = initial.DataBlob;
            return this;
        }
    }
}