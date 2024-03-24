using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfFilter.Application.Services.Models;
internal class TypeDefinition
{
    public string Name { get; set; }
    public List<ItemAttribute> Attributes { get; set; }
}

internal class ItemAttribute
{
    Type Type { get; set; }
    bool Required { get; set; }
}
