﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WattApp.api.Models
{
    public class DashboardItemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public double Demand { get; set; }
        public double Inc { get; set; }
        public DateTime lastUpdate { get; set; }
    }
}