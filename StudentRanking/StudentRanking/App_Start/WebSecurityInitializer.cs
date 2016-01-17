using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StudentRanking.App_Start
{
    // Call this with WebSecurityInitializer.Instance.EnsureInitialize()
    public class WebSecurityInitializer
    {
        private WebSecurityInitializer() { }
        public static readonly WebSecurityInitializer Instance = new WebSecurityInitializer();
        private bool isNotInit = true;
        private readonly object SyncRoot = new object();
        public void EnsureInitialize()
        {
            if (isNotInit)
            {
                lock (this.SyncRoot)
                {
                    if (isNotInit)
                    {
                        isNotInit = false;
                        WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("DefaultConnection",
                            userTableName: "UserProfile", userIdColumn: "UserId", userNameColumn: "UserName",
                            autoCreateTables: true);

                         
                    }
                }
            }
        }
    }
}