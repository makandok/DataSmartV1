using Newtonsoft.Json;

namespace JhpDataSystem.model
{
    public class KindMetaData
    {
        public string devid { get; set; }
        public int chksum { get; set; }
        public int facidx { get; set; }
        public string getJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
        public KindMetaData fromJson(KindItem jsonValue)
        {
            var obj = JsonConvert.DeserializeObject<KindMetaData>(jsonValue.Value);
            devid = obj.devid;
            chksum = obj.chksum;
            facidx = obj.facidx;
            return this;
        }
    }
}