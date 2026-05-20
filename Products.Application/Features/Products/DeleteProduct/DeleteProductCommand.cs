using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Application.Features.Products.DeleteProduct
{
    public sealed record DeleteProductCommand(Guid Id);
}