using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Senalandia.Models;
using Senalandia.Models.Roles;


namespace Senalandia.Controllers
{
    public class EstudianteController : Controller
    {
        private readonly SenalalandiaContext _context;

        public EstudianteController(SenalalandiaContext context)
        {
            _context = context;
        }

        // Vista principal del dashboard del estudiante
        public async Task<IActionResult> Index(int? idEstudiante)
        {
            // Si no se proporciona ID, usar el primer estudiante (en producción vendría de la sesión/autenticación)
            if (!idEstudiante.HasValue)
            {
                var primerEstudiante = await _context.Estudiantes.FirstOrDefaultAsync();
                if (primerEstudiante == null)
                    return NotFound("No se encontraron estudiantes");

                idEstudiante = primerEstudiante.IdEstudiante;
            }

            var viewModel = await ObtenerDashboardData(idEstudiante.Value);

            if (viewModel == null)
                return NotFound("Estudiante no encontrado");

            return View(viewModel);
        }

        // Método para obtener todos los datos del dashboard
        private async Task<EstudianteDashboardViewModel> ObtenerDashboardData(int idEstudiante)
        {
            var estudiante = await _context.Estudiantes
                .Include(e => e.IdUsuarioNavigation)
                .Include(e => e.EstudianteMedallas)
                    .ThenInclude(em => em.IdMedallaNavigation)
                .FirstOrDefaultAsync(e => e.IdEstudiante == idEstudiante);

            if (estudiante == null)
                return null;

            // Obtener nivel actual con valor por defecto
            var nivelActual = await _context.Niveles
                .FirstOrDefaultAsync(n => n.NumeroNivel == (estudiante.NivelActual ?? 1))
                ?? new Nivele
                {
                    NumeroNivel = 1,
                    Nombre = "Principiante",
                    PuntosRequeridos = 0
                };

            // Obtener siguiente nivel con valor por defecto
            var siguienteNivel = await _context.Niveles
                .FirstOrDefaultAsync(n => n.NumeroNivel == (estudiante.NivelActual ?? 1) + 1)
                ?? new Nivele
                {
                    NumeroNivel = (estudiante.NivelActual ?? 1) + 1,
                    Nombre = "Siguiente Nivel",
                    PuntosRequeridos = (estudiante.PuntosTotales ?? 0) + 100
                };

            // Obtener módulos con progreso - manejo de lista nula
            var modulos = await _context.Modulos
                .Where(m => m.Estado == "activo")
                .OrderBy(m => m.Orden)
                .Select(m => new ModuloEstudianteViewModel
                {
                    IdModulo = m.IdModulo,
                    Titulo = m.Titulo ?? "Módulo sin título",
                    Descripcion = m.Descripcion ?? "Descripción no disponible",
                    Color = m.Color ?? "blue",
                    Icono = m.Icono ?? "bi-book",
                    TotalTarjetas = _context.Tarjetas
                        .Where(t => t.IdModulo == m.IdModulo)
                        .Count(),
                    TarjetasCompletadas = _context.ProgresoTarjetas
                        .Where(pt => pt.IdEstudiante == idEstudiante
                            && pt.Completada == true
                            && pt.IdTarjetaNavigation.IdModulo == m.IdModulo)
                        .Count(),
                    PorcentajeCompletado = 0 // Se calculará después
                })
                .ToListAsync() ?? new List<ModuloEstudianteViewModel>();

            // Calcular porcentaje de completación para cada módulo
            foreach (var modulo in modulos)
            {
                modulo.PorcentajeCompletado = modulo.TotalTarjetas > 0
                    ? (int)Math.Round((modulo.TarjetasCompletadas * 100.0) / modulo.TotalTarjetas)
                    : 0;
            }

            // Obtener recomendación de módulo con manejo de nulos
            var recomendacion = await ObtenerRecomendacionModulo(idEstudiante, modulos)
                ?? new RecomendacionViewModel
                {
                    IdModulo = modulos.FirstOrDefault()?.IdModulo ?? 0,
                    NombreModulo = modulos.FirstOrDefault()?.Titulo ?? "Módulo Inicial",
                    Razonamiento = "Comienza tu aprendizaje con este módulo",
                    PorcentajeCompletado = 0
                };

            // *** SOLUCIÓN PARA LAS MEDALLAS ***
            // Primero obtener todas las medallas
            var todasLasMedallas = await _context.Medallas
                .Select(m => new
                {
                    m.IdMedalla,
                    m.Nombre,
                    m.Descripcion,
                    m.Icono
                })
                .ToListAsync();

            // Luego obtener los IDs de las medallas que el estudiante ya tiene
            var medallasEstudianteIds = estudiante.EstudianteMedallas?
                .Select(em => em.IdMedalla)
                .ToList() ?? new List<int>();

            // Finalmente construir el ViewModel en memoria
            var medallas = todasLasMedallas
                .Select(m => new MedallaViewModel
                {
                    IdMedalla = m.IdMedalla,
                    Nombre = m.Nombre ?? "Medalla",
                    Descripcion = m.Descripcion ?? "Logro obtenido por tu progreso",
                    Icono = m.Icono ?? "🏆",
                    Obtenida = medallasEstudianteIds.Contains(m.IdMedalla)
                })
                .ToList();

            // Calcular puntos para siguiente nivel
            int puntosParaSiguienteNivel = siguienteNivel != null
                ? Math.Max(0, siguienteNivel.PuntosRequeridos - (estudiante.PuntosTotales ?? 0))
                : 0;

            // Calcular porcentaje de progreso seguro
            int porcentajeProgreso = 0;
            if (siguienteNivel != null && siguienteNivel.PuntosRequeridos > 0)
            {
                var puntosActuales = estudiante.PuntosTotales ?? 0;
                porcentajeProgreso = (int)Math.Round(Math.Min(100, (puntosActuales * 100.0) / siguienteNivel.PuntosRequeridos));
            }
            else
            {
                porcentajeProgreso = 100; // Si no hay siguiente nivel, está completo
            }

            var viewModel = new EstudianteDashboardViewModel
            {
                IdEstudiante = estudiante.IdEstudiante,
                NombreCompleto = $"{estudiante.IdUsuarioNavigation?.Nombre ?? "Usuario"} {estudiante.IdUsuarioNavigation?.Apellido ?? ""}".Trim(),
                AvatarUrl = estudiante.IdUsuarioNavigation?.AvatarUrl ?? "/images/avatars/default.png",
                NivelActual = estudiante.NivelActual ?? 1,
                NombreNivel = nivelActual?.Nombre ?? "Principiante",
                PuntosTotales = estudiante.PuntosTotales ?? 0,
                PuntosNivelActual = nivelActual?.PuntosRequeridos ?? 0,
                PuntosSiguienteNivel = siguienteNivel?.PuntosRequeridos ?? 100,
                PorcentajeProgreso = porcentajeProgreso,
                PuntosParaSiguienteNivel = puntosParaSiguienteNivel,
                LeccionesCompletadas = estudiante.LeccionesCompletadas ?? 0,
                TiempoTotalMinutos = estudiante.TiempoTotalMinutos ?? 0,
                Modulos = modulos,
                Medallas = medallas,
                Recomendacion = recomendacion
            };

            return viewModel;
        }

        // Método auxiliar para obtener recomendación con manejo de errores
        private async Task<RecomendacionViewModel> ObtenerRecomendacionModulo(int idEstudiante, List<ModuloEstudianteViewModel> modulos)
        {
            try
            {
                if (modulos == null || !modulos.Any())
                {
                    return new RecomendacionViewModel
                    {
                        IdModulo = 0,
                        NombreModulo = "Módulo Inicial",
                        Razonamiento = "Comienza con el primer módulo disponible",
                        PorcentajeCompletado = 0
                    };
                }

                // Encontrar módulo con menor progreso que no esté completo
                var moduloRecomendado = modulos
                    .Where(m => m.PorcentajeCompletado < 100)
                    .OrderBy(m => m.PorcentajeCompletado)
                    .FirstOrDefault();

                // Si todos están completos, recomendar el primero
                if (moduloRecomendado == null)
                {
                    moduloRecomendado = modulos.First();
                }

                return new RecomendacionViewModel
                {
                    IdModulo = moduloRecomendado.IdModulo,
                    NombreModulo = moduloRecomendado.Titulo,
                    Razonamiento = moduloRecomendado.PorcentajeCompletado == 0
                        ? "Perfecto para comenzar"
                        : $"Continúa tu progreso ({moduloRecomendado.PorcentajeCompletado}% completado)",
                    PorcentajeCompletado = moduloRecomendado.PorcentajeCompletado
                };
            }
            catch (Exception ex)
            {
                // Log the error if needed
                return new RecomendacionViewModel
                {
                    IdModulo = modulos?.FirstOrDefault()?.IdModulo ?? 0,
                    NombreModulo = "Módulo Recomendado",
                    Razonamiento = "Basado en tu progreso de aprendizaje",
                    PorcentajeCompletado = 0
                };
            }
        }

        // API para obtener tarjetas de un módulo
        [HttpGet]
        public async Task<IActionResult> ObtenerTarjetasModulo(int idModulo, int idEstudiante)
        {
            var modulo = await _context.Modulos
                .FirstOrDefaultAsync(m => m.IdModulo == idModulo);

            if (modulo == null)
                return NotFound();

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
                    puntos = t.Puntos ?? 10,
                    completada = _context.ProgresoTarjetas
                        .Any(pt => pt.IdEstudiante == idEstudiante
                            && pt.IdTarjeta == t.IdTarjeta
                            && pt.Completada == true)
                })
                .ToListAsync();

            return Json(new
            {
                modulo = new
                {
                    titulo = modulo.Titulo,
                    descripcion = modulo.Descripcion
                },
                tarjetas = tarjetas
            });
        }

        // Marcar tarjeta como completada
        [HttpPost]
        public async Task<IActionResult> CompletarTarjeta([FromBody] CompletarTarjetaDto dto)
        {
            try
            {
                var progreso = await _context.ProgresoTarjetas
                    .FirstOrDefaultAsync(pt => pt.IdEstudiante == dto.IdEstudiante
                        && pt.IdTarjeta == dto.IdTarjeta);

                if (progreso == null)
                {
                    // Crear nuevo registro de progreso
                    progreso = new ProgresoTarjeta
                    {
                        IdEstudiante = dto.IdEstudiante,
                        IdTarjeta = dto.IdTarjeta,
                        Completada = true,
                        FechaCompletada = DateTime.Now,
                        Intentos = 1,
                        UltimaVisualizacion = DateTime.Now
                    };
                    _context.ProgresoTarjetas.Add(progreso);
                }
                else
                {
                    // Actualizar progreso existente
                    progreso.Completada = dto.Completada;
                    if (dto.Completada)
                    {
                        progreso.FechaCompletada = DateTime.Now;
                    }
                    progreso.UltimaVisualizacion = DateTime.Now;
                }

                // Obtener puntos de la tarjeta
                var tarjeta = await _context.Tarjetas.FindAsync(dto.IdTarjeta);
                int puntos = tarjeta?.Puntos ?? 10;

                // Actualizar puntos del estudiante si se marca como completada
                if (dto.Completada)
                {
                    var estudiante = await _context.Estudiantes
                        .FindAsync(dto.IdEstudiante);

                    if (estudiante != null)
                    {
                        estudiante.PuntosTotales = (estudiante.PuntosTotales ?? 0) + puntos;
                        estudiante.LeccionesCompletadas = (estudiante.LeccionesCompletadas ?? 0) + 1;
                    }
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, mensaje = "Progreso actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = ex.Message });
            }
        }
    }
}