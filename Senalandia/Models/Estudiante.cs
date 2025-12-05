using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Estudiante
{
    public int IdEstudiante { get; set; }

    public int IdUsuario { get; set; }

    public int? NivelActual { get; set; }

    public int? PuntosTotales { get; set; }

    public decimal? ProgresoNivelActual { get; set; }

    public int? TiempoTotalMinutos { get; set; }

    public int? LeccionesCompletadas { get; set; }

    public DateTime? FechaInicioNivel { get; set; }

    public virtual ICollection<ActividadEstudiante> ActividadEstudiantes { get; set; } = new List<ActividadEstudiante>();

    public virtual ICollection<EstudianteMedalla> EstudianteMedallas { get; set; } = new List<EstudianteMedalla>();

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Observacione> Observaciones { get; set; } = new List<Observacione>();

    public virtual ICollection<ProgresoTarjeta> ProgresoTarjeta { get; set; } = new List<ProgresoTarjeta>();

    public virtual ICollection<TutorEstudiante> TutorEstudiantes { get; set; } = new List<TutorEstudiante>();
}
