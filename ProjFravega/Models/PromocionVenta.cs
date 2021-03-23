using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Models
{
    public class PromocionVenta
    {
        public Guid Id { get; set; }
        public string MedioPago { get; set; }
        public string Banco { get; set; }
        public IEnumerable<string> CategoriasProductos { get; set; }
        public int? CantidadDeCuotas { get; set; }
        public decimal? PorcentajeInteresCuota { get; set; }
        public decimal? ProcentajeDeDescuento { get; set; }    
    }
}
