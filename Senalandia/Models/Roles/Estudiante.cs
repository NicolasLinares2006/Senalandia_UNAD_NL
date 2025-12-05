namespace Senalandia.Models.Roles
{
    // ViewModels
    public class EstudianteDashboardViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; }
        public string AvatarUrl { get; set; }
        public int NivelActual { get; set; }
        public string NombreNivel { get; set; }
        public int PuntosTotales { get; set; }
        public int PuntosNivelActual { get; set; }
        public int PuntosSiguienteNivel { get; set; }
        public int PorcentajeProgreso { get; set; }
        public int PuntosParaSiguienteNivel { get; set; }
        public int LeccionesCompletadas { get; set; }
        public int TiempoTotalMinutos { get; set; }
        public List<ModuloEstudianteViewModel> Modulos { get; set; }
        public List<MedallaViewModel> Medallas { get; set; }
        public RecomendacionViewModel Recomendacion { get; set; }
    }

    public class ModuloEstudianteViewModel
    {
        public int IdModulo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Color { get; set; }
        public string Icono { get; set; }
        public int TotalTarjetas { get; set; }
        public int TarjetasCompletadas { get; set; }
        public int PorcentajeCompletado { get; set; }
    }

    public class MedallaViewModel
    {
        public int IdMedalla { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Icono { get; set; }
        public bool Obtenida { get; set; }
    }

    public class RecomendacionViewModel
    {
        public int IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public string Razonamiento { get; set; }
        public int PorcentajeCompletado { get; set; }
    }

    // DTOs
    public class CompletarTarjetaDto
    {
        public int IdEstudiante { get; set; }
        public int IdTarjeta { get; set; }
        public bool Completada { get; set; }
    }
}
