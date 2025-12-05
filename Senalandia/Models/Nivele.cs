using System;
using System.Collections.Generic;

namespace Senalandia.Models;

public partial class Nivele
{
    public int IdNivel { get; set; }

    public int NumeroNivel { get; set; }

    public string Nombre { get; set; } = null!;

    public int PuntosRequeridos { get; set; }

    public string? Descripcion { get; set; }

    public string? Color { get; set; }
}
