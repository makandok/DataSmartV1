using System;
using System.Collections.Generic;

namespace JhpDataSystem.model
{
    public interface ILocalDbEntity: ISaveableEntity
    {
        DateTime CoreActivityDate { get; set; }
        ISaveableEntity build();
        List<NameValuePair> ToValuesList();
        ILocalDbEntity Load(GeneralEntityDataset clientSummary);
    }

    public interface ISaveableEntity
    {
        KindKey Id { get; set; }
        KindKey EntityId { get; set; }
        string KindMetaData { get; set; }
        long getItemId();
    }

    public class EncryptedKindEntity
    {
        public EncryptedKindEntity()
        {
        }
        public EncryptedKindEntity(EncryptedText name)
        {
            Value = name;
        }
        public EncryptedText Value;
    }

    public class KindViewDefinition
    {
        public int ViewId { get; set; }
        public Type ViewActivity { get; set; }
    }

    public class KindDefinition
    {
        public KindName _kindName { get; set; }
        public KindViewDefinition[] KindViews { get; set; }
        public bool Matches(string kindname)
        {
            return _kindName.Value == kindname;
        }
        public bool Matches(KindName kindName)
        {
            return Matches(kindName.Value); 
        }
    }

    public class KindEntity
    {
        public KindItem kindItem { get; set; }
        public KindName kindName { get; set; }
        public KindKey kindKey { get; set; }
    }

    public class KindItem
    {
        public KindItem()
        {
        }
        public KindItem(string value)
        {
            Value = value;
        }
        public string Value;
    }

    public class KindKey
    {
        public KindKey()
        {
        }
        public KindKey(string id)
        {
            Value = id;
        }
        public string Value;
    }

    public class KindName
    {
        public KindName()
        {
        }

        public KindName(string name)
        {
            Value = name;
        }
        public string Value;
    }
}