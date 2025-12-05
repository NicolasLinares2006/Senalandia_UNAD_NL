using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senalandia.Models;
using Senalandia.Models.Roles;

namespace Senalandia.Controllers
{
    public class TutorController : Controller
    {
        private readonly SenalalandiaContext _context;

        public TutorController(SenalalandiaContext context)
        {
            _context = context;
        }

        // Vista principal del dashboard del tutor
        public async Task<IActionResult> Index(int? idTutor)
        {
            // Si no se proporciona ID, usar el primer tutor (en producción vendría de la sesión/autenticación)
            if (!idTutor.HasValue)
            {
                var primerTutor = await _context.Tutores.FirstOrDefaultAsync();
                if (primerTutor == null)
                    return NotFound("No se encontraron tutores");

                idTutor = primerTutor.IdTutor;
            }

            var viewModel = await ObtenerDashboardTutor(idTutor.Value);

            if (viewModel == null)
                return NotFound("Tutor no encontrado");

            return View(viewModel);
        }

        // Método para obtener todos los datos del dashboard
        private async Task<TutorDashboardViewModel> ObtenerDashboardTutor(int idTutor)
        {
            var tutor = await _context.Tutores
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(t => t.IdTutor == idTutor);

            if (tutor == null)
                return null;

            // Obtener estudiantes asignados al tutor
            var estudiantesAsignados = await _context.TutorEstudiantes
                .Where(te => te.IdTutor == idTutor && te.Estado == "activo")
                .Include(te => te.IdEstudianteNavigation)
                    .ThenInclude(e => e.IdUsuarioNavigation)
                .Select(te => te.IdEstudianteNavigation)
                .ToListAsync();

            // Crear lista de estudiantes con su progreso
            var estudiantesConProgreso = new List<EstudianteTutorViewModel>();

            foreach (var estudiante in estudiantesAsignados)
            {
                var nivel = await _context.Niveles
                    .FirstOrDefaultAsync(n => n.NumeroNivel == estudiante.NivelActual);

                var siguienteNivel = await _context.Niveles
                    .FirstOrDefaultAsync(n => n.NumeroNivel == estudiante.NivelActual + 1);

                // Calcular progreso del nivel actual
                int porcentajeProgreso = 0;
                if (siguienteNivel != null && estudiante.PuntosTotales.HasValue)
                {
                    var puntosNivelActual = nivel?.PuntosRequeridos ?? 0;
                    var puntosSiguienteNivel = siguienteNivel.PuntosRequeridos;
                    var puntosEnNivel = estudiante.PuntosTotales.Value - puntosNivelActual;
                    var puntosNecesarios = puntosSiguienteNivel - puntosNivelActual;

                    if (puntosNecesarios > 0)
                    {
                        porcentajeProgreso = (int)Math.Round((puntosEnNivel * 100.0) / puntosNecesarios);
                    }
                }

                // Calcular última actividad
                var ultimaConexion = estudiante.IdUsuarioNavigation.UltimaConexion ?? DateTime.Now;
                var tiempoTranscurrido = DateTime.Now - ultimaConexion;
                string ultimaActividad = FormatearTiempoTranscurrido(tiempoTranscurrido);
                string estadoActividad = CalcularEstadoActividad(tiempoTranscurrido);

                estudiantesConProgreso.Add(new EstudianteTutorViewModel
                {
                    IdEstudiante = estudiante.IdEstudiante,
                    NombreCompleto = $"{estudiante.IdUsuarioNavigation.Nombre} {estudiante.IdUsuarioNavigation.Apellido}",
                    AvatarUrl = estudiante.IdUsuarioNavigation.AvatarUrl ?? "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&h=200&fit=crop",
                    NivelActual = estudiante.NivelActual ?? 1,
                    NombreNivel = nivel?.Nombre ?? "Principiante",
                    PorcentajeProgreso = porcentajeProgreso,
                    UltimaActividad = ultimaActividad,
                    EstadoActividad = estadoActividad,
                    PuntosTotales = estudiante.PuntosTotales ?? 0,
                    LeccionesCompletadas = estudiante.LeccionesCompletadas ?? 0,
                    TiempoTotalMinutos = estudiante.TiempoTotalMinutos ?? 0
                });
            }

            var viewModel = new TutorDashboardViewModel
            {
                IdTutor = tutor.IdTutor,
                NombreTutor = $"{tutor.IdUsuarioNavigation.Nombre} {tutor.IdUsuarioNavigation.Apellido}",
                Estudiantes = estudiantesConProgreso.OrderByDescending(e => e.UltimaConexion).ToList()
            };

            return viewModel;
        }

        // API para obtener el reporte detallado de un estudiante
        [HttpGet]
        public async Task<IActionResult> ObtenerReporteEstudiante(int idEstudiante, int idTutor)
        {
            // Verificar que el estudiante esté asignado al tutor
            var asignacion = await _context.TutorEstudiantes
                .FirstOrDefaultAsync(te => te.IdTutor == idTutor
                    && te.IdEstudiante == idEstudiante
                    && te.Estado == "activo");

            if (asignacion == null)
                return Forbid();

            var estudiante = await _context.Estudiantes
                .Include(e => e.IdUsuarioNavigation)
                .FirstOrDefaultAsync(e => e.IdEstudiante == idEstudiante);

            if (estudiante == null)
                return NotFound();

            var nivel = await _context.Niveles
                .FirstOrDefaultAsync(n => n.NumeroNivel == estudiante.NivelActual);

            var siguienteNivel = await _context.Niveles
                .FirstOrDefaultAsync(n => n.NumeroNivel == estudiante.NivelActual + 1);

            // Calcular progreso del nivel
            int porcentajeProgreso = 0;
            if (siguienteNivel != null && estudiante.PuntosTotales.HasValue)
            {
                var puntosNivelActual = nivel?.PuntosRequeridos ?? 0;
                var puntosSiguienteNivel = siguienteNivel.PuntosRequeridos;
                var puntosEnNivel = estudiante.PuntosTotales.Value - puntosNivelActual;
                var puntosNecesarios = puntosSiguienteNivel - puntosNivelActual;

                if (puntosNecesarios > 0)
                {
                    porcentajeProgreso = (int)Math.Round((puntosEnNivel * 100.0) / puntosNecesarios);
                }
            }

            // Obtener actividad de los últimos 7 días
            var fechaInicio = DateTime.Now.AddDays(-6).Date;
            var actividadDiaria = await _context.ActividadEstudiantes
                .Where(a => a.IdEstudiante == idEstudiante && a.Fecha >= fechaInicio)
                .OrderBy(a => a.Fecha)
                .Select(a => new
                {
                    fecha = a.Fecha.ToString("dd/MM"),
                    minutos = a.TiempoMinutos ?? 0,
                    lecciones = a.LeccionesCompletadas ?? 0,
                    puntos = a.PuntosGanados ?? 0
                })
                .ToListAsync();

            // Completar días faltantes con 0
            var actividadCompleta = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                var fecha = fechaInicio.AddDays(i);
                var actividad = actividadDiaria.FirstOrDefault(a => a.fecha == fecha.ToString("dd/MM"));

                if (actividad != null)
                {
                    actividadCompleta.Add(actividad);
                }
                else
                {
                    actividadCompleta.Add(new
                    {
                        fecha = fecha.ToString("dd/MM"),
                        minutos = 0,
                        lecciones = 0,
                        puntos = 0
                    });
                }
            }

            // Obtener distribución de módulos completados
            var modulosCompletados = await _context.ProgresoTarjetas
                .Where(pt => pt.IdEstudiante == idEstudiante && pt.Completada == true)
                .Include(pt => pt.IdTarjetaNavigation)
                    .ThenInclude(t => t.IdModuloNavigation)
                .GroupBy(pt => new
                {
                    IdModulo = pt.IdTarjetaNavigation.IdModulo,
                    NombreModulo = pt.IdTarjetaNavigation.IdModuloNavigation.Titulo
                })
                .Select(g => new
                {
                    modulo = g.Key.NombreModulo,
                    cantidad = g.Count()
                })
                .ToListAsync();

            // Formatear tiempo total
            int horas = (estudiante.TiempoTotalMinutos ?? 0) / 60;
            int minutos = (estudiante.TiempoTotalMinutos ?? 0) % 60;
            string tiempoFormateado = $"{horas}h {minutos}m";

            // Obtener observaciones recientes
            var observaciones = await _context.Observaciones
                .Where(o => o.IdEstudiante == idEstudiante && o.IdTutor == idTutor)
                .OrderByDescending(o => o.FechaCreacion)
                .Take(5)
                .Select(o => new
                {
                    idObservacion = o.IdObservacion,
                    contenido = o.Contenido,
                    fecha = o.FechaCreacion
                })
                .ToListAsync();

            var reporte = new
            {
                estudiante = new
                {
                    idEstudiante = estudiante.IdEstudiante,
                    nombreCompleto = $"{estudiante.IdUsuarioNavigation.Nombre} {estudiante.IdUsuarioNavigation.Apellido}",
                    avatarUrl = estudiante.IdUsuarioNavigation.AvatarUrl ?? "https://images.unsplash.com/photo-1494790108377-be9c29b29330?w=200&h=200&fit=crop",
                    nivelActual = estudiante.NivelActual ?? 1,
                    nombreNivel = nivel?.Nombre ?? "Principiante",
                    puntosTotales = estudiante.PuntosTotales ?? 0,
                    leccionesCompletadas = estudiante.LeccionesCompletadas ?? 0,
                    tiempoTotal = tiempoFormateado,
                    porcentajeProgreso = porcentajeProgreso
                },
                actividadDiaria = actividadCompleta,
                modulosCompletados = modulosCompletados,
                observaciones = observaciones
            };

            return Json(reporte);
        }

        // Agregar observación
        [HttpPost]
        public async Task<IActionResult> AgregarObservacion([FromBody] AgregarObservacionDto dto)
        {
            try
            {
                // Verificar que el estudiante esté asignado al tutor
                var asignacion = await _context.TutorEstudiantes
                    .FirstOrDefaultAsync(te => te.IdTutor == dto.IdTutor
                        && te.IdEstudiante == dto.IdEstudiante
                        && te.Estado == "activo");

                if (asignacion == null)
                    return Json(new { success = false, mensaje = "No tienes permiso para agregar observaciones a este estudiante" });

                var observacion = new Observacione
                {
                    IdTutor = dto.IdTutor,
                    IdEstudiante = dto.IdEstudiante,
                    Contenido = dto.Contenido,
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                };

                _context.Observaciones.Add(observacion);
                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Observación agregada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }

        // Métodos auxiliares
        private string FormatearTiempoTranscurrido(TimeSpan tiempo)
        {
            if (tiempo.TotalMinutes < 60)
                return $"hace {(int)tiempo.TotalMinutes} minutos";
            else if (tiempo.TotalHours < 24)
                return $"hace {(int)tiempo.TotalHours} horas";
            else if (tiempo.TotalDays < 7)
                return $"hace {(int)tiempo.TotalDays} días";
            else if (tiempo.TotalDays < 30)
                return $"hace {(int)(tiempo.TotalDays / 7)} semanas";
            else
                return $"hace {(int)(tiempo.TotalDays / 30)} meses";
        }

        private string CalcularEstadoActividad(TimeSpan tiempo)
        {
            if (tiempo.TotalHours < 24)
                return "activo";
            else if (tiempo.TotalDays < 3)
                return "reciente";
            else
                return "inactivo";
        }
    }
}