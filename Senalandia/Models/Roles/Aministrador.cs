namespace Senalandia.Models.Roles
{
    // ViewModels
    public class DashboardViewModel
    {
        public int TotalUsuarios { get; set; }
        public int ModulosActivos { get; set; }
        public int ModulosTotales { get; set; }
        public string ModuloMasUsado { get; set; }
        public decimal TasaCompletacion { get; set; }
        public List<ModuloViewModel> Modulos { get; set; }
    }

    public class ModuloViewModel
    {
        public int IdModulo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public int CantidadTarjetas { get; set; }
    }

    public class TarjetasModuloViewModel
    {
        public int IdModulo { get; set; }
        public string NombreModulo { get; set; }
        public int CantidadTarjetas { get; set; }
        public List<TarjetaViewModel> Tarjetas { get; set; }
    }

    public class TarjetaViewModel
    {
        public int IdTarjeta { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Puntos { get; set; }
    }

    // DTOs
    public class CrearModuloDto
    {
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
    }

    public class CrearTarjetaDto
    {
        public int IdModulo { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string ImagenUrl { get; set; }
        public string VideoUrl { get; set; }
        public int Puntos { get; set; }
    }
}
