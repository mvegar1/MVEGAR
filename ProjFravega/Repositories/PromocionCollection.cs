using MongoDB.Bson;
using MongoDB.Driver;
using ProjFravega.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjFravega.Repositories
{
    public class PromocionCollection : IPromocionCollection
    {
        internal MongoDBRepository _repository = new MongoDBRepository();
        private IMongoCollection<Promocion> Collection;
        private DateTime fechaActual = DateTime.Now;

        public PromocionCollection()
        {
            Collection = _repository.db.GetCollection<Promocion>("Promociones");
        }

        /// <summary>
        /// Obtener todas las promociones
        /// </summary>
        /// <returns>Lista de Promociones</returns>
        public async Task<List<Promocion>> ObtenerPromociones()
        {
            return await Collection.FindAsync(new BsonDocument()).Result.ToListAsync();
        }

        /// <summary>
        /// Obtener promoción por Id
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>Una promoción filtrada por Id</returns>
        public async Task<Promocion> ObtenerPromocionPorId(Guid Id)
        {
            var guid = new BsonBinaryData(Id, GuidRepresentation.Standard);

            return await Collection.FindAsync(
                new BsonDocument { { "_id", guid } }).Result.FirstAsync();
        }

        /// <summary>
        /// Obtener todas las promociones vigentes
        /// </summary>
        /// <returns>Lista de Promociones Vigentes</returns>
        public async Task<List<Promocion>> ObtenerPromocionesVigentes()
        {
            var filter = Builders<Promocion>.Filter.Where(p => p.FechaFin >= fechaActual & p.Activo == true);

            return await Collection.FindAsync(filter).Result.ToListAsync();
        }

        /// <summary>
        /// Obtener todas las promociones vigentes
        /// </summary>
        /// <returns>Lista de Promociones Vigentes</returns>
        public async Task<List<Promocion>> ObtenerPromocionesVigentesPorFecha(DateTime Fecha)
        {
            var filter = Builders<Promocion>.Filter.Where(p => p.FechaFin >= Fecha & p.Activo == true);

            return await Collection.FindAsync(filter).Result.ToListAsync();
        }

        /// <summary>
        /// Obtener las promociones vigentes para una venta
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns>Lista de Promociones Vigentes</returns>
        public async Task<List<PromocionVenta>> ObtenerPromocionesVigentesVenta(PromocionFilter promocionFilter)
        {
            var filterBuilder = Builders<Promocion>.Filter;

            var filter = filterBuilder.AnyIn(p => p.CategoriasProductos, promocionFilter.CategoriasProducto) &
                         filterBuilder.Where(p => p.MediosDePago.Contains(promocionFilter.MedioDePago) & p.Bancos.Contains(promocionFilter.Banco));

            var res = await Collection.FindAsync(filter).Result.ToListAsync();

            var data = res.Select(p => new PromocionVenta() { 
                Id = p.Id, 
                MedioPago = promocionFilter.MedioDePago,
                Banco = promocionFilter.Banco,
                CategoriasProductos = p.CategoriasProductos,
                CantidadDeCuotas = p.MaximaCantidadDeCuotas,
                PorcentajeInteresCuota = p.ValorInteresCuotas,
                ProcentajeDeDescuento = p.ProcentajeDeDescuento
            }).ToList();

            return data;
        }

        /// <summary>
        /// Crear promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        public async Task CrearPromocion(Promocion promocion)
        {
            await Collection.InsertOneAsync(promocion);
        }

        /// <summary>
        /// Actualizar promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        public async Task ActualizarPromocion(Promocion promocion)
        {
            var filter = Builders<Promocion>
                 .Filter.Eq(p => p.Id, promocion.Id);

            await Collection.ReplaceOneAsync(filter, promocion);
        }

        /// <summary>
        /// Eliminar promoción de manera lógica
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task EliminarPromocion(Promocion promocion, Guid Id)
        {
            var filter = Builders<Promocion>
                 .Filter.Eq(p => p.Id, promocion.Id);

            await Collection.ReplaceOneAsync(filter, promocion);
        }

        /// <summary>
        /// Modificar vigencia de promoción
        /// </summary>
        /// <param name="promocion"></param>
        /// <returns></returns>
        public async Task ActualizarVigenciaPromocion(Promocion promocion)
        {
            var filter = Builders<Promocion>
                 .Filter.Eq(p => p.Id, promocion.Id);

            await Collection.ReplaceOneAsync(filter, promocion);
        }
    }
}
