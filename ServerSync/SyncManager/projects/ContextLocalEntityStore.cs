using JhpDataSystem.model;
using System.Collections.Generic;
using System;
using System.Linq;
using JhpDataSystem.store;

namespace JhpDataSystem.projects
{
    public class ContextLocalEntityStore : LocalEntityStore
    {
        public BaseContextManager ContextManager { get; private set; }
        public ContextLocalEntityStore(BaseContextManager context)
        {
            ContextManager = context;
        }

        public List<KindItem> GetAllBlobs(KindName entityKind)
        {
            return new TableStore(entityKind.Value).GetAllBlobs();
        }

        //public void updateRecordSummaryTable()
        //{
        //    var query = "select count(*) from {0}";
        //    var allBlobs = GetAllBobs();
        //    var clientRecords = (from record in allBlobs
        //                         select new PPDataSet().fromJson(record));
        //    var allRecords = (
        //        from record in clientRecords
        //        let val = record.FieldValues
        //            .FirstOrDefault(f => f.Name == Constants.FIELD_PPX_DATEOFVISIT)
        //        where val != null
        //        let visitDate = string.IsNullOrWhiteSpace(val.Value) ? DateTime.MinValue : Convert.ToDateTime(val.Value)
        //        select new RecordSummary()
        //        {
        //            Id = record.Id.Value,
        //            EntityId = record.EntityId.Value,
        //            KindName = record.FormName,
        //            //Constants.PPX_KIND_DISPLAYNAMES[record.FormName],
        //            VisitDate = visitDate
        //        }).ToList();

        //    var db = new LocalDB3().DB;
        //    allRecords.ForEach(t => db.InsertOrReplace(t));
        //    var allSaved = db.Table<RecordSummary>().ToList();
        //}

        public void updateRecordSummaryTable()
        {
            var allBlobs = GetAllBobs();
            var clientRecords = (from record in allBlobs
                                 select DbSaveableEntity.fromJson<GeneralEntityDataset>(record)
                ).ToList();
            var allRecords = (
                from record in clientRecords
                let val = record.FieldValues
                    .FirstOrDefault(f => f.Name == ContextManager.FIELD_VISITDATE)
                where val != null
                let visitDate = string.IsNullOrWhiteSpace(val.Value) ? DateTime.MinValue : Convert.ToDateTime(val.Value)
                select new RecordSummary()
                {
                    Id = record.Id.Value,
                    EntityId = record.EntityId.Value,
                    KindName = record.FormName,
                    VisitDate = visitDate
                }).ToList();

            var db = new LocalDB3().DB;
            //allRecords.ForEach(t => db.InsertOrReplace(t));
            //var allSaved = db.Table<RecordSummary>().ToList();
        }

        public List<NameValuePair> GetAllBobsCount()
        {
            var tables = ContextManager.KindDisplayNames.Keys.ToList();
            var store = new MultiTableStore()
            {
                Kinds =
                (from table in tables select new KindName(table)).ToList()
            };

            var allBlobs = store.getAllBobsCount();
            return allBlobs;
        }

        public List<KindItem> GetAllBobs()
        {
            var tables = ContextManager.KindDisplayNames.Keys.ToList();
            var store = new MultiTableStore()
            {
                Kinds =
                (from table in tables select new KindName(table)).ToList()
            };

            var allBlobs = store.getRecordBlobs();
            return allBlobs;
        }

    }
}
