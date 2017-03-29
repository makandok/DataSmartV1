using System;
using System.Collections.Generic;
using System.Linq;
using DataSmart.store;
using JhpDataSystem.model;
using JhpDataSystem;

namespace DataSmart.Security
{
    public class UserAuthenticator
    {
        internal static KindName KindName = new KindName(Constants.KIND_APPUSERS);
        internal string computeHash(string userName, int passCode)
        {
            var name = userName.ToLowerInvariant();
            return JhpSecurity.Encrypt(name + Constants.MOTHER_OFBOLG + passCode);
        }

        internal UserSession Authenticate(string userName, int passCode)
        {
            var name = userName.ToLowerInvariant();
            var knownBolg = string.Empty;
            var spawnOfAzog = string.Empty;
            if (name == Constants.ADMIN_USERNAME)
            {
                knownBolg = AppInstance.Instance.ApiAssets[Constants.ASSET_ADMIN_HASH];
                spawnOfAzog = JhpSecurity.Encrypt(Constants.ADMIN_USERNAME + Constants.MOTHER_OFALLBOLGS + passCode);
                if (spawnOfAzog == knownBolg)
                {
                    //you are an admin orc :)
                    var rootUser = new AppUser() { Id = new KindKey("root"), Names = "JHP Admin", UserId = Constants.ADMIN_USERNAME };
                    return new UserSession() { AuthorisationToken = Guid.NewGuid().ToString("N"), Id = rootUser.Id, User = rootUser };
                }
                return null;
            }

            //var keys = LocalEntityStore.Instance.GetKeys(authenticationStore);            
            var bolg = LoadCredentials().Where(t => t.UserId == name).FirstOrDefault();
            if (bolg == null)
                return null;

            knownBolg = bolg.KnownBolg;
            spawnOfAzog = JhpSecurity.Encrypt(name + Constants.MOTHER_OFBOLG + passCode);
            if (spawnOfAzog == bolg.KnownBolg)
            {
                //you are an orc :)
                return new UserSession() { AuthorisationToken = Guid.NewGuid().ToString("N"), Id = bolg.Id, User = bolg };
            }

            return null;
        }

        internal List<AppUser> LoadCredentials()
        {
            var allCreds = new TableStore(KindName).GetAllBlobs();
            if (allCreds == null)
                return new List<AppUser>();

            return (from credString in allCreds
                     select DbSaveableEntity.fromJson<AppUser>(credString)).ToList();
        }
    }
}