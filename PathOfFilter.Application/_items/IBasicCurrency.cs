using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfFilter.Application.Items;
public interface IBasicCurrency 
{
    public string Name { get; set; }
    public int? MinimumStackSize { get; set; }
    public int? MaximumStackSize { get; set; }
}
