using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GpsUtil.Location
{
    public class NearbyAttractionDto
    {
        public string AttractionName { get; set; }
        public double AttractionLatitude { get; set; }
        public double AttractionLongitude { get; set; }
        public double UserLatitude { get; set; }
        public double UserLongitude { get; set; }
        public double Distance { get; set; }
        public int RewardPoints { get; set; }
    }

}
