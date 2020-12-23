using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLiNhaSach.Models
{
    public class Rule
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Decription { get; set; }

        public bool UseThisRule { get; set; }

        public bool IsCheckRange { get; set; }
        
        public int Min { get; set; }

        public int Max { get; set; }
    }
}
