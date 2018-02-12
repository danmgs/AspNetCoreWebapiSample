using AspNetCoreWebapiSample.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCoreWebapiSample.Domain.Entities
{
    public class User : EntityBase
    {        
        public char Age { get; set; }
        public string Gender { get; set; }
    }
}
