using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LestateTSD
{
	public class Goods
	{
		public string VendorCode { get; set; }
		public bool Marking { get; set; }
		public string Barcode { get; set; }
		public string Batch { get; set; }
		public string DataMatrix { get; set; } = string.Empty;
		public int Count { get; set; } = 1;
	}
}
