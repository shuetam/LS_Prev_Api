using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Live.Core
{
    public class Entity
    {
        public Guid ID {get; protected set;}
        public Entity()
        {
            this.ID = new Guid();
        }

    }

}