using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Models
{
    public class PromocionFilter
    {
        public string MedioDePago { get; set; }
        public string Banco { get; set; }
        public IEnumerable<string> CategoriasProducto { get; set; }
    }
}
