using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMB_Kuwait.Models
{
    public class MemberData
    {
        public string Id { get; set; }
        public string Floor { get; set; }
        public string Flat { get; set; }
        public string HouseId { get; set; }
        public string DeliveryPerson { get; set; }
        public string DeliveryPerson2 { get; set; }
        public string MasoolName { get; set; }
        public string Mobile { get; set; }
        public string ThaliNumber { get; set; }
        public string ThaliCode { get; set; }
        public int Size { get; set; }
        public string NewCode { get; set; }
        public int Count { get; internal set; }
    }

    public class MemberDataViewModel
    {
        public List<MemberData> memberDatasList { get; set; }
    }
}