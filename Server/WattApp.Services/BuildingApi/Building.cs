using BuildingApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WattApp.data.Webjobs.API
{
    public class Space : EntityLink
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public EntityLink Type { get; set; }
        [JsonProperty(PropertyName = "isLocatedInSpace")]
        public EntityLink Location { get; set; }
    }

    public class Building : Space
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
