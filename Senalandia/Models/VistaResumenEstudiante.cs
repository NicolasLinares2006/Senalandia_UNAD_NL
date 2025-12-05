using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class VistaResumenEstudiante
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public DateTime? UltimaConexion { get; set; }

    public int IdEstudiante { get; set; }

    public int? NivelActual { get; set; }

    public int? PuntosTotales { get; set; }

    public decimal? ProgresoNivelActual { get; set; }

    public int? TiempoTotalMinutos { get; set; }

    public int? LeccionesCompletadas { get; set; }

    public string? NombreNivel { get; set; }

    public string EstadoActividad { get; set; } = null!;
}
