namespace Senalandia.Models.Roles
{
    // ViewModels
    public class TutorDashboardViewModel
    {
        public int IdTutor { get; set; }
        public string NombreTutor { get; set; }
        public List<EstudianteTutorViewModel> Estudiantes { get; set; }
    }

    public class EstudianteTutorViewModel
    {
        public int IdEstudiante { get; set; }
        public string NombreCompleto { get; set; }
        public string AvatarUrl { get; set; }
        public int NivelActual { get; set; }
        public string NombreNivel { get; set; }
        public int PorcentajeProgreso { get; set; }
        public string UltimaActividad { get; set; }
        public string EstadoActividad { get; set; }
        public int PuntosTotales { get; set; }
        public int LeccionesCompletadas { get; set; }
        public int TiempoTotalMinutos { get; set; }
        public DateTime UltimaConexion { get; set; }
    }

    // DTOs
    public class AgregarObservacionDto
    {
        public int IdTutor { get; set; }
        public int IdEstudiante { get; set; }
        public string Contenido { get; set; }
    }
}
