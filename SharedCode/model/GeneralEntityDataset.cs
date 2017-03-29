using System.Collections.Generic;
using System.Linq;

namespace JhpDataSystem.model
{
    public class GeneralEntityDataset : ISaveableEntity
    {
        public KindKey Id { get; set; }
        public KindKey EntityId { get; set; }
        public string KindMetaData { get; set; }

        public string FormName { get; set; }
        public List<NameValuePair> FieldValues { get; set; }

        public NameValuePair GetValue(string fieldName)
        {
            var toReturn = FieldValues
                .Where(t => t.Name.Contains(fieldName))
                .FirstOrDefault();
            return toReturn;
        }
        public long getItemId()
        {
            return Id == null ? -1L : Id.GetHashCode();
        }
    }
}