using Microsoft.AspNetCore.Mvc;
using ProjFravega.Estructuras;
using ProjFravega.Models;
using ProjFravega.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjFravega.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromocionController : Controller
    {
        private DateTime fechaActual = DateTime.Now;
        private IPromocionCollection db = new PromocionCollection();

        [HttpGet]
        public async Task<IActionResult> ObtenerPromociones()
        {
            return Ok(await db.ObtenerPromociones());
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> ObtenerPromocionPorId(Guid Id)
        {
            return Ok(await db.ObtenerPromocionPorId(Id));
        }

        [HttpGet("PromocionesVigentes")]
        public async Task<IActionResult> ObtenerPromocionesVigentes()
        {
            return Ok(await db.ObtenerPromocionesVigentes());
        }

        [HttpGet("PromocionesVigentes/{Fecha}")]
        public async Task<IActionResult> ObtenerPromocionesVigentesPorFecha(DateTime Fecha)
        {
            return Ok(await db.ObtenerPromocionesVigentesPorFecha(Fecha));
        }

        [HttpPost("PromocionesVigentesVenta")]
        public async Task<IActionResult> ObtenerPromocionesVigentesVenta([FromBody] PromocionFilter promocionFilter)
        {
            return Ok(await db.ObtenerPromocionesVigentesVenta(promocionFilter));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromocion([FromBody] Promocion promocion)
        {
            promocion.Id = Guid.NewGuid();

            if (promocion == null)
            {
                return BadRequest();
            }        

            if (!Validaciones(promocion))
            {
                return BadRequest();
            }

            promocion.FechaCreacion = fechaActual;
            promocion.Activo = true;

            await db.CrearPromocion(promocion);

            return Created("Create", true);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdatePromocion([FromBody] Promocion promocion, Guid Id)
        {
            if (!Validaciones(promocion))
            {
                return BadRequest();
            }
            if (promocion.Id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Se debe suministrar el Id de la promoción");
                return BadRequest();
            }

            promocion.Id = Id;
            promocion.FechaModificacion = fechaActual;

            await db.ActualizarPromocion(promocion);

            return Created("Update", true);
        }

        [HttpPut("ActualizarVigencia/{Id}")]
        public async Task<IActionResult> UpdateVigenciaPromocion([FromBody] PromocionVigencia promocionVigencia, Guid Id)
        {
            if (promocionVigencia == null)
            {
                return BadRequest();
            }
            if (!ValidacionesVigencia(promocionVigencia))
            {
                return BadRequest();
            }
            if (promocionVigencia.Id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Se debe suministrar el Id de la promoción");
                return BadRequest();
            }

            Promocion promocion = new Promocion();
            promocion.FechaInicio = promocionVigencia.FechaInicio;
            promocion.FechaFin = promocionVigencia.FechaFin;
            promocion.Id = Id;

            await db.ActualizarPromocion(promocion);

            return Created("Update", true);
        }

        [HttpPut("EliminarPromocion/{Id}")]
        public async Task<IActionResult> DeletePromocion(Guid Id)
        {
            if (Id == Guid.Empty)
            {
                ModelState.AddModelError("Id", "Se debe suministrar el Id de la promoción");
                return BadRequest();
            }

            Promocion promocion = new Promocion();
            promocion.Activo = false;
            promocion.Id = Id;
            promocion.FechaModificacion = fechaActual;

            await db.EliminarPromocion(promocion, Id);

            return Created("Delete", true);
        }

        private bool Validaciones(Promocion promocion)
        {
            bool Resultado = true;
            Parametros parametros = new Parametros();

            var items = db.ObtenerPromociones();

            int registrosExistentes = 0;

            foreach (var item in items.Result)
            {
                int cantidadCoincidencias = 0;

                if (item.CategoriasProductos != null)
                {
                    foreach (var row in promocion.CategoriasProductos)
                    {
                        var coincidencias = item.CategoriasProductos.Where(p => p.Contains(row));
                        cantidadCoincidencias = +cantidadCoincidencias + coincidencias.Count();
                    }
                }
                if (item.Bancos != null)
                {
                    foreach (var row in promocion.Bancos)
                    {
                        var coincidencias = item.Bancos.Where(p => p.Contains(row));
                        cantidadCoincidencias = +cantidadCoincidencias + coincidencias.Count();
                    }
                }
                if (item.MediosDePago != null)
                {
                    foreach (var row in promocion.MediosDePago)
                    {
                        var coincidencias = item.MediosDePago.Where(p => p.Contains(row));
                        cantidadCoincidencias = +cantidadCoincidencias + coincidencias.Count();
                    }
                }

                if (cantidadCoincidencias == 3)
                    registrosExistentes = +1;

            }
            
            
            if (registrosExistentes > 0)
            {
                ModelState.AddModelError("Promocion", "Ya existe una promoción con las cararetisticas ingresadas");
                Resultado = false;
            }

            if (promocion == null)
            {
                Resultado = false;
            }            
            else if (promocion.MaximaCantidadDeCuotas == null && promocion.ProcentajeDeDescuento == null)
            {
                ModelState.AddModelError("MaximaCantidadDeCuotas", "Se requiere suministrar el número de máximo de cuotas o el porcentaje de descuento");
                ModelState.AddModelError("ProcentajeDeDescuento", "Se requiere suministrar el número de máximo de cuotas o el porcentaje de descuento");
                Resultado = false;
            }
            else if (promocion.ValorInteresCuotas != null && promocion.MaximaCantidadDeCuotas == null)
            {
                ModelState.AddModelError("MaximaCantidadDeCuotas", "El número de cuotas es necesario si se suministra el valor de interés de la cuota");
                ModelState.AddModelError("ValorInteresCuotas", "El número de cuotas es necesario si se suministra el valor de interés de la cuota");
                Resultado = false;
            }
            else if ((promocion.ProcentajeDeDescuento != null && promocion.ProcentajeDeDescuento > 0) && (promocion.MaximaCantidadDeCuotas != null && promocion.MaximaCantidadDeCuotas > 0))
            {
                ModelState.AddModelError("ProcentajeDeDescuento", "La promoción puede tener porcentaje de descuento o cuotas. NO ambas.");
                ModelState.AddModelError("MaximaCantidadDeCuotas", "La promoción puede tener porcentaje de descuento o cuotas. NO ambas.");
            }
            else if (promocion.ProcentajeDeDescuento != null)
            {
                if (promocion.ProcentajeDeDescuento <= 5 || promocion.ProcentajeDeDescuento > 80)
                {
                    ModelState.AddModelError("ProcentajeDeDescuento", "El porcentaje de descuento debe estar comprendido entre 5 y 80");
                    Resultado = false;
                }
            }
            else if (promocion.FechaFin < fechaActual)
            {
                ModelState.AddModelError("FechaFin", "La fecha final de la promoción no puede ser menor que la fecha actual");
                Resultado = false;
            }
            else if (promocion.FechaFin < promocion.FechaInicio)
            {
                ModelState.AddModelError("FechaInicio", "La fecha final de la promoción no puede ser mayo que la fecha inicial");
                ModelState.AddModelError("FechaFin", "La fecha final de la promoción no puede ser mayo que la fecha inicial");
                Resultado = false;
            }

            int Categorias = 0, Bancos = 0, MediosPago = 0;
            int CantidadCategorias = promocion.CategoriasProductos.Count(), CantidadBancos = promocion.Bancos.Count(), CantidadMediosPago = promocion.MediosDePago.Count();

            if (CantidadCategorias > 0)
            {
                foreach (var prom in promocion.CategoriasProductos)
                {
                    var coincidencias = parametros.parametrosPromocion.Where(p => p.Value.Contains(prom));

                    if (coincidencias.Count() > 0)
                    {
                        Categorias += 1;
                    }
                }

                if (Categorias != CantidadCategorias)
                {
                    Resultado = false;
                    ModelState.AddModelError("CategoriasProductos", "La categoria de producto suministrada no es válida");
                }
            }

            if (CantidadBancos > 0)
            {
                foreach (var prom in promocion.Bancos)
                {
                    var coincidencias = parametros.parametrosPromocion.Where(p => p.Value.Contains(prom));
                    
                    if (coincidencias.Count() > 0)
                    {
                        Bancos += 1;
                    }
                }

                if (Bancos != CantidadBancos)
                {
                    Resultado = false;
                    ModelState.AddModelError("Bancos", "El banco suministrado no es válido");
                }
            }

            if (CantidadMediosPago > 0)
            {
                foreach (var prom in promocion.MediosDePago)
                {
                    var coincidencias = parametros.parametrosPromocion.Where(p => p.Value.Contains(prom));

                    if (coincidencias.Count() > 0)
                    {
                        MediosPago += 1;
                    }
                }

                if (MediosPago != CantidadMediosPago)
                {
                    Resultado = false;
                    ModelState.AddModelError("MediosPago", "El medio de pago suministrado no es válido");
                }
            }

            return Resultado;
        }

        private bool ValidacionesVigencia(PromocionVigencia promocionVigencia)
        {
            bool Resultado = true;

            if (promocionVigencia.FechaFin < fechaActual)
            {
                ModelState.AddModelError("FechaFin", "La fecha final de la promoción no puede ser menor que la fecha actual");
                Resultado = false;
            }
            else if (promocionVigencia.FechaFin < promocionVigencia.FechaInicio)
            {
                ModelState.AddModelError("FechaInicio", "La fecha inicial de la promoción no puede ser mayor que la fecha inicial");
                ModelState.AddModelError("FechaFin", "La fecha final de la promoción no puede ser menor que la fecha inicial");
                Resultado = false;
            }

            return Resultado;
        }        
    }
}
