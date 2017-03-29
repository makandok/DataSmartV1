using JhpDataSystem.model;

namespace JhpDataSystem.projects
{
    public class DefaultLookupProvider<T> : ClientLookupProvider<T> where T : class, ILocalDbEntity, new()
    {
        public DefaultLookupProvider(string kindName):base(kindName)
        {
        }
    }
}