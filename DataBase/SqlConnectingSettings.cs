using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Live.Core;
using Live;

namespace Live
{
    public class SqlConnectingSettings
    {
         public string ConnectionString {get; set;}

         public SqlConnectingSettings(string connection)
         {
             this.ConnectionString = connection;
         }
    }
}