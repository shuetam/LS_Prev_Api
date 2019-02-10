using System;
using System.Collections.Generic;
using System.IO;

namespace Live
{
    public class Licension
    {

        public string googleKey {get; set;}

        public Licension(string _googleKey)
        {
            this.googleKey = _googleKey;
        }
    }
}