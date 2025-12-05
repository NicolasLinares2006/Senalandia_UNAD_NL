using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senalandia.Models;
using Senalandia.Models.Roles;

namespace Senalandia.Controllers
{
    public class AdminController : Controller
    {
        private readonly SenalalandiaContext _context;

        public AdminController(SenalalandiaContext context)
        {
            _context = context;
        }

        // Vista principal del dashboard
        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel
            {
                // Estadísticas generales
                TotalUsuarios = await _context.Usuarios
                    .Where(u => u.TipoUsuario == "estudiante")
                    .CountAsync(),

                ModulosActivos = await _context.Modulos
                    .Where(m => m.Estado == "activo")
                    .CountAsync(),

                ModulosTotales = await _context.Modulos.CountAsync(),

                // Módulo más usado (basado en tarjetas completadas)
                ModuloMasUsado = await ObtenerModuloMasUsado(),

                // Tasa de completación promedio
                TasaCompletacion = await CalcularTasaCompletacion(),

                // Lista de módulos con cantidad de tarjetas
                Modulos = await _context.Modulos
                    .Select(m => new ModuloViewModel
                    {
                        IdModulo = m.IdModulo,
                        Titulo = m.Titulo,
                        Descripcion = m.Descripcion,
                        Estado = m.Estado,
                        CantidadTarjetas = _context.Tarjetas
                            .Where(t => t.IdModulo == m.IdModulo)
                            .Count()
                    })
                    .OrderBy(m => m.Titulo)
                    .ToListAsync()
            };

            return View(dashboardData);
        }

        // API para obtener tarjetas de un módulo
        [HttpGet]
        public async Task<IActionResult> ObtenerTarjetas(int idModulo)
        {
            var tarjetas = await _context.Tarjetas
                .Where(t => t.IdModulo == idModulo)
                .OrderBy(t => t.Orden)
                .Select(t => new
                {
                    idTarjeta = t.IdTarjeta,
                    titulo = t.Titulo,
                    descripcion = t.Descripcion,
                    imagenUrl = t.ImagenUrl,
                    videoUrl = t.VideoUrl,
                    puntos = t.Puntos ?? 0
                })
                .ToListAsync();

            return Json(tarjetas);
        }

        // Métodos auxiliares privados
        private async Task<string> ObtenerModuloMasUsado()
        {
            var moduloMasUsado = await _context.ProgresoTarjetas
                .Include(pt => pt.IdTarjetaNavigation)
                    .ThenInclude(t => t.IdModuloNavigation)
                .Where(pt => pt.Completada == true)
                .GroupBy(pt => pt.IdTarjetaNavigation.IdModuloNavigation.Titulo)
                .Select(g => new { Modulo = g.Key, Cantidad = g.Count() })
                .OrderByDescending(x => x.Cantidad)
                .FirstOrDefaultAsync();

            return moduloMasUsado?.Modulo ?? "N/A";
        }

        private async Task<decimal> CalcularTasaCompletacion()
        {
            var totalEstudiantes = await _context.Estudiantes.CountAsync();

            if (totalEstudiantes == 0)
                return 0;

            var promedioLecciones = await _context.Estudiantes
                .AverageAsync(e => (decimal)e.LeccionesCompletadas);

            return Math.Round(promedioLecciones, 2);
        }

        // API Endpoints para CRUD de módulos
        [HttpPost]
        public async Task<IActionResult> CrearModulo([FromBody] CrearModuloDto dto)
        {
            try
            {
                var ultimoOrden = await _context.Modulos
                    .MaxAsync(m => (int?)m.Orden) ?? 0;

                var modulo = new Modulo
                {
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    Estado = dto.Estado,
                    Orden = ultimoOrden + 1,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };

                _context.Modulos.Add(modulo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Módulo creado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearTarjeta([FromBody] CrearTarjetaDto dto)
        {
            try
            {
                var ultimoOrden = await _context.Tarjetas
                    .Where(t => t.IdModulo == dto.IdModulo)
                    .MaxAsync(t => (int?)t.Orden) ?? 0;

                var tarjeta = new Tarjeta
                {
                    IdModulo = dto.IdModulo,
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion,
                    ImagenUrl = dto.ImagenUrl,
                    VideoUrl = dto.VideoUrl,
                    Orden = ultimoOrden + 1,
                    Puntos = dto.Puntos,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };

                _context.Tarjetas.Add(tarjeta);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Tarjeta creada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarModulo(int id)
        {
            try
            {
                var modulo = await _context.Modulos.FindAsync(id);
                if (modulo == null)
                    return Json(new { success = false, mensaje = "Módulo no encontrado" });

                _context.Modulos.Remove(modulo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Módulo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> EliminarTarjeta(int id)
        {
            try
            {
                var tarjeta = await _context.Tarjetas.FindAsync(id);
                if (tarjeta == null)
                    return Json(new { success = false, mensaje = "Tarjeta no encontrada" });

                _context.Tarjetas.Remove(tarjeta);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Tarjeta eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}