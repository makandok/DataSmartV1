using JhpDataSystem;
using JhpDataSystem.model;
using JhpDataSystem.store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncManager.store
{
    public static class Deidentifier
    {
        public static string deidentifyBlob(this string dataBlob, string formName)
        {
            var toReturn = string.Empty;
            //we deserialise the blob to its base form
            var deserialised = DbSaveableEntity.fromJson<GeneralEntityDataset>(
                new KindItem(dataBlob)
                );
            var fieldValues = deserialised.FieldValues;

            //we get the field dictionary for this entity

            //we read each field, and deidentify if needed based on field attributes

            //and copy to out object

            //copy out object to the out entity

            return dataBlob;
        }
    }
}
