using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Estructuras
{
    public class Parametros
    {

        public IDictionary<string, string> parametrosPromocion = new Dictionary<string, string>();

        public Parametros()
        {
            parametrosPromocion.Add("Medios de pago", "TARJETA_CREDITO, TARJETA_DEBITO, EFECTIVO, GIFT_CARD");
            parametrosPromocion.Add("Bancos", "Galicia, Santander Rio, Ciudad, Nacion, ICBC, BBVA, Macro");
            parametrosPromocion.Add("Categorías productos", "Hogar, Jardin, ElectroCocina, GrandesElectro, Colchones, Celulares, Tecnologia, Audio");  
        }
    }
}
