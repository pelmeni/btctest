using BtcWebSrvApp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitTestProject
{
    public class WebAppConnectionStringProvider : IConnectionStringProvider
    {
        public string ConnectionString => "Data Source=192.168.0.45;Initial Catalog=btcsrvdb;Persist Security Info=True;User ID=test;Password=test";
    }
}