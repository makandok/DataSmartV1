namespace JhpDataSystem.store
{
    public class LocalEntityStore
    {

    }
    //public class LocalEntityStore
    //{
    //    public string ConnectionString;
    //    LocalDB _localDb;
    //    public LocalDB InstanceLocalDb { get { return _localDb; } }
    //    internal TableStore defaultTableStore { get; set; }

    //    public LocalEntityStore()
    //    {
    //        return;

    //        var dbString = new LocalDB3().defaultConnectionString;
    //        //var db = new DB();
    //        var ctx = new SyncManager.localdb.DBContainer();

    //        var entity = new SyncManager.localdb.OutEntity() { Id = "123", DataBlob = "I'm a blob" };

    //        var sctxt = ctx.Set<SyncManager.localdb.OutEntity>();
    //        //sctxt.

    //        ctx.OutEntities.Add(entity);
    //        ctx.SaveChanges();

    //        var entity2 = new SyncManager.localdb.OutEntity() { Id = "1223", DataBlob = "I'm a blob" };
    //        ctx.OutEntities.Add(entity2);
    //        var entity3 = new SyncManager.localdb.OutEntity() { Id = "123", DataBlob = "I'm a blob Reloaded" };
    //        ctx.OutEntities.Add(entity3);
    //        ctx.SaveChanges();

    //        //ctx.OutEntity.
    //        _localDb = new LocalDB();
    //        defaultTableStore = new TableStore(Constants.KIND_DEFAULT);
    //    }

    //    public void RebuildRecordSummaryIndexes()
    //    {
    //        //var db = new LocalDB3().DB;
    //        //db.DeleteAll<RecordSummary>();
    //        //db.DropTable<RecordSummary>();
    //        //db.CreateTable<RecordSummary>();

    //        //var ppxKinds = Constants.PPX_KIND_DISPLAYNAMES.Keys;
    //        //var vmmcKinds = Constants.VMMC_KIND_DISPLAYNAMES.Keys;
    //        //var combined = new List<string>();
    //        //combined.AddRange(ppxKinds);
    //        //combined.AddRange(vmmcKinds);

    //        //var multiStore = new MultiTableStore()
    //        //{
    //        //    Kinds = (from kind in combined select new KindName(kind)).ToList()
    //        //};
    //        //var kindRecords = multiStore.getRecordBlobs();
    //        //var recordSummaries = new List<RecordSummary>();
    //        //foreach (var record in kindRecords)
    //        //{
    //        //    var saveable = Newtonsoft.Json.JsonConvert
    //        //        .DeserializeObject<GeneralEntityDataset>(record.Value);
    //        //    var editDateObj = saveable.GetValue(Constants.FIELD_PPX_DATEOFVISIT);
    //        //    editDateObj = editDateObj ?? saveable.GetValue(Constants.FIELD_VMMC_DATEOFVISIT);
    //        //    DateTime dateEdited;
    //        //    if (editDateObj == null || string.IsNullOrWhiteSpace(editDateObj.Value))
    //        //    {
    //        //        dateEdited = DateTime.MinValue;
    //        //    }
    //        //    else
    //        //    {
    //        //        if (!DateTime.TryParse(editDateObj.Value, out dateEdited))
    //        //        {
    //        //            dateEdited = DateTime.MinValue;
    //        //        }
    //        //    }

    //        //    var recSummary = new RecordSummary()
    //        //    {
    //        //        Id = saveable.Id.Value,
    //        //        EntityId =
    //        //        saveable.EntityId == null ?
    //        //        saveable.Id.Value
    //        //        : saveable.EntityId.Value,
    //        //        VisitDate = dateEdited,
    //        //        KindName = saveable.FormName
    //        //    };
    //        //    recordSummaries.Add(recSummary);
    //        //}
    //        //db.InsertAll(recordSummaries);
    //    }

    //    public void RebuildClientSummaryIndexes<T, U>(KindName name) where T : class, ILocalDbEntity, new() where U : ClientLookupProvider<T>, new()
    //    {
    //        //var db = new LocalDB3().DB;
    //        //db.DeleteAll<T>();
    //        //db.DropTable<T>();
    //        //db.CreateTable<T>();

    //        //var vmmcReg = new TableStore(name.Value).GetAllBlobs();
    //        //var allSummaries = new List<T>();
    //        //foreach(var reg in vmmcReg)
    //        //{
    //        //    var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<GeneralEntityDataset>(reg.Value);
    //        //    var mySummary = new T().Load(entity) as T;
    //        //    allSummaries.Add(mySummary);
    //        //}
    //        //new U().InsertOrReplace(allSummaries);
    //    }

    //    internal void ClearAllData()
    //    {
    //        clearTables();
    //    }

    //    private void clearTables()
    //    {
    //        return;

    //        buildTables(false);

    //        var db = new LocalDB3().DB;
    //        //db.DeleteAll<OutEntity>();
    //        //db.DeleteAll<OutEntityUnsynced>();

    //        //db.DeleteAll<projects.ppx.PPClientSummary>();
    //        //db.DeleteAll<projects.vmc.VmmcClientSummary>();
    //        //db.DeleteAll<RecordSummary>();

    //        new TableStore(Constants.KIND_DEFAULT).DeleteAll();

    //        new TableStore(Constants.KIND_SITESESSION).DeleteAll();
    //        new TableStore(Constants.KIND_SITEPROVIDER).DeleteAll();

    //        //prepex clients
    //        new TableStore(Constants.KIND_PPX_CLIENTEVAL).DeleteAll();
    //        new TableStore(Constants.KIND_PPX_DEVICEREMOVAL).DeleteAll();
    //        new TableStore(Constants.KIND_PPX_POSTREMOVAL).DeleteAll();
    //        new TableStore(Constants.KIND_PPX_UNSCHEDULEDVISIT).DeleteAll();

    //        //VMMC
    //        new TableStore(Constants.KIND_VMMC_POSTOP).DeleteAll();
    //        new TableStore(Constants.KIND_VMMC_REGANDPROCEDURE).DeleteAll();
    //    }

    //    public void buildTables(bool rebuildIndexes)
    //    {
    //        return;

    //        //var db = SyncManager.localdb.DataClasses1DataContext

    //        //db.CreateTable(new TableDef() {name="OutEntity" });
    //        //db.CreateTable<OutEntity>();
    //        //db.CreateTable<OutEntityUnsynced>();

    //        //db.CreateTable<projects.ppx.PPClientSummary>();
    //        //db.CreateTable<projects.vmc.VmmcClientSummary>();
    //        //db.CreateTable<RecordSummary>();

    //        //db.CreateTable<DeviceConfiguration>();
    //        ////we load from kinds
    //        if (rebuildIndexes)
    //        {
    //            //RebuildClientSummaryIndexes<
    //            //    projects.vmc.VmmcClientSummary,
    //            //    projects.vmc.VmmcLookupProvider>(
    //            //    new KindName(Constants.KIND_VMMC_REGANDPROCEDURE));

    //            //RebuildClientSummaryIndexes<
    //            //    projects.ppx.PPClientSummary,
    //            //    projects.ppx.PpxLookupProvider>(
    //            //    new KindName(Constants.KIND_PPX_CLIENTEVAL));

    //            RebuildRecordSummaryIndexes();
    //        }

    //        new TableStore(Constants.KIND_DEFAULT).build();
    //        new TableStore(Constants.KIND_APPUSERS).build();

    //        //new TableStore(Constants.KIND_SITEPROVIDER).build();
    //        new TableStore(Constants.KIND_SITESESSION).build();
    //        new TableStore(Constants.KIND_SITEPROVIDER).build();

    //        //prepex clients
    //        new TableStore(Constants.KIND_PPX_CLIENTEVAL).build();
    //        new TableStore(Constants.KIND_PPX_DEVICEREMOVAL).build();
    //        new TableStore(Constants.KIND_PPX_POSTREMOVAL).build();
    //        new TableStore(Constants.KIND_PPX_UNSCHEDULEDVISIT).build();

    //        //VMMC
    //        new TableStore(Constants.KIND_VMMC_POSTOP).build();
    //        new TableStore(Constants.KIND_VMMC_REGANDPROCEDURE).build();
    //    }

    //    internal KindKey Save(KindKey entityId, KindName entityKind, KindItem dataToSave)
    //    {
    //        //we save to both kindregister and entity table
    //        return new TableStore(entityKind.Value).Update(entityId, dataToSave);
    //    }
    //}
}