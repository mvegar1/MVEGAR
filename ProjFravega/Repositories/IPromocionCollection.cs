using ProjFravega.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Repositories
{
    public interface IPromocionCollection
    {
        /// <summary>
        /// Obtener todas las promociones
        /// </summary>
        /// <returns>Lista de Promociones</returns>
        Task<List<Promocion>> ObtenerPromociones();

        /// <summary>
        /// Obtener promoción por Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Una promoción filtrada por Id</returns>
        Task<Promocion> ObtenerPromocionPorId(Guid Id);

        /// <summary>
        /// Obtener todas las promociones vigentes
        /// </summary>
        /// <returns>Lista de Promociones Vigentes</returns>
        Task<List<Promocion>> ObtenerPromocionesVigentes();

        /// <summary>
        /// Obtener todas las promociones vigentes por fecha
        /// </summary>
        /// <param name="Fecha"></param>
        /// <returns>Lista de Promociones vigentes</returns>
        Task<List<Promocion>> ObtenerPromocionesVigentesPorFecha(DateTime Fecha);

        /// <summary>
        /// Obtiene todas las promociones vigentes
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns>Lista de Promociones vigentes</returns>
        Task<List<PromocionVenta>> ObtenerPromocionesVigentesVenta(PromocionFilter promocionFilter);


        /// <summary>
        /// Crear promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        Task CrearPromocion(Promocion promocion);

        /// <summary>
        /// Actualizar promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        Task ActualizarPromocion(Promocion promocion);

        /// <summary>
        /// Eliminar promoción
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task EliminarPromocion(Promocion promocion, Guid Id);

        /// <summary>
        /// Actualizar vigencia de promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        Task ActualizarVigenciaPromocion(Promocion promocion);
    }
}
