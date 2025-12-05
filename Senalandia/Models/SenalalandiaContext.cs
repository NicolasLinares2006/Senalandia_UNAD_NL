using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Senalandia.Models;

public partial class SenalalandiaContext : DbContext
{
    public SenalalandiaContext()
    {
    }

    public SenalalandiaContext(DbContextOptions<SenalalandiaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ActividadEstudiante> ActividadEstudiantes { get; set; }

    public virtual DbSet<Estudiante> Estudiantes { get; set; }

    public virtual DbSet<EstudianteMedalla> EstudianteMedallas { get; set; }

    public virtual DbSet<Medalla> Medallas { get; set; }

    public virtual DbSet<Modulo> Modulos { get; set; }

    public virtual DbSet<Nivele> Niveles { get; set; }

    public virtual DbSet<Notificacione> Notificaciones { get; set; }

    public virtual DbSet<Observacione> Observaciones { get; set; }

    public virtual DbSet<ProgresoTarjeta> ProgresoTarjetas { get; set; }

    public virtual DbSet<Tarjeta> Tarjetas { get; set; }

    public virtual DbSet<TutorEstudiante> TutorEstudiantes { get; set; }

    public virtual DbSet<Tutore> Tutores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VistaProgresoModulo> VistaProgresoModulos { get; set; }

    public virtual DbSet<VistaResumenEstudiante> VistaResumenEstudiantes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ActividadEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdActividad).HasName("PK__activida__DCD3488304331858");

            entity.ToTable("actividad_estudiante");

            entity.HasIndex(e => e.IdEstudiante, "idx_estudiante");

            entity.HasIndex(e => e.Fecha, "idx_fecha");

            entity.HasIndex(e => new { e.IdEstudiante, e.Fecha }, "unique_estudiante_fecha").IsUnique();

            entity.Property(e => e.IdActividad).HasColumnName("id_actividad");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.LeccionesCompletadas)
                .HasDefaultValue(0)
                .HasColumnName("lecciones_completadas");
            entity.Property(e => e.PuntosGanados)
                .HasDefaultValue(0)
                .HasColumnName("puntos_ganados");
            entity.Property(e => e.TiempoMinutos)
                .HasDefaultValue(0)
                .HasColumnName("tiempo_minutos");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.ActividadEstudiantes)
                .HasForeignKey(d => d.IdEstudiante)
                .HasConstraintName("FK__actividad__id_es__04E4BC85");
        });

        modelBuilder.Entity<Estudiante>(entity =>
        {
            entity.HasKey(e => e.IdEstudiante).HasName("PK__estudian__E0B2763CA7038505");

            entity.ToTable("estudiantes");

            entity.HasIndex(e => e.NivelActual, "idx_nivel");

            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.FechaInicioNivel)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_inicio_nivel");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.LeccionesCompletadas)
                .HasDefaultValue(0)
                .HasColumnName("lecciones_completadas");
            entity.Property(e => e.NivelActual)
                .HasDefaultValue(1)
                .HasColumnName("nivel_actual");
            entity.Property(e => e.ProgresoNivelActual)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progreso_nivel_actual");
            entity.Property(e => e.PuntosTotales)
                .HasDefaultValue(0)
                .HasColumnName("puntos_totales");
            entity.Property(e => e.TiempoTotalMinutos)
                .HasDefaultValue(0)
                .HasColumnName("tiempo_total_minutos");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Estudiantes)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__estudiant__id_us__5629CD9C");
        });

        modelBuilder.Entity<EstudianteMedalla>(entity =>
        {
            entity.HasKey(e => e.IdEstudianteMedalla).HasName("PK__estudian__0D06BBB17A067A3A");

            entity.ToTable("estudiante_medallas");

            entity.HasIndex(e => e.IdEstudiante, "idx_estudiante");

            entity.HasIndex(e => new { e.IdEstudiante, e.IdMedalla }, "unique_estudiante_medalla").IsUnique();

            entity.Property(e => e.IdEstudianteMedalla).HasColumnName("id_estudiante_medalla");
            entity.Property(e => e.FechaObtencion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_obtencion");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdMedalla).HasColumnName("id_medalla");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.EstudianteMedallas)
                .HasForeignKey(d => d.IdEstudiante)
                .HasConstraintName("FK__estudiant__id_es__7D439ABD");

            entity.HasOne(d => d.IdMedallaNavigation).WithMany(p => p.EstudianteMedallas)
                .HasForeignKey(d => d.IdMedalla)
                .HasConstraintName("FK__estudiant__id_me__7E37BEF6");
        });

        modelBuilder.Entity<Medalla>(entity =>
        {
            entity.HasKey(e => e.IdMedalla).HasName("PK__medallas__3D8D7E537E093138");

            entity.ToTable("medallas");

            entity.Property(e => e.IdMedalla).HasColumnName("id_medalla");
            entity.Property(e => e.Criterio)
                .HasColumnType("ntext")
                .HasColumnName("criterio");
            entity.Property(e => e.Descripcion)
                .HasColumnType("ntext")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Icono)
                .HasMaxLength(50)
                .HasColumnName("icono");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PuntosRequeridos).HasColumnName("puntos_requeridos");
        });

        modelBuilder.Entity<Modulo>(entity =>
        {
            entity.HasKey(e => e.IdModulo).HasName("PK__modulos__B2584DFC2F02F3F9");

            entity.ToTable("modulos");

            entity.HasIndex(e => e.Estado, "idx_estado");

            entity.HasIndex(e => e.Orden, "idx_orden");

            entity.Property(e => e.IdModulo).HasColumnName("id_modulo");
            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .HasColumnName("color");
            entity.Property(e => e.Descripcion)
                .HasColumnType("ntext")
                .HasColumnName("descripcion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Icono)
                .HasMaxLength(50)
                .HasColumnName("icono");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");
        });

        modelBuilder.Entity<Nivele>(entity =>
        {
            entity.HasKey(e => e.IdNivel).HasName("PK__niveles__9CAF1C537334FFBB");

            entity.ToTable("niveles");

            entity.HasIndex(e => e.NumeroNivel, "UQ__niveles__982C683992BF3B2D").IsUnique();

            entity.HasIndex(e => e.NumeroNivel, "idx_numero_nivel");

            entity.Property(e => e.IdNivel).HasColumnName("id_nivel");
            entity.Property(e => e.Color)
                .HasMaxLength(20)
                .HasColumnName("color");
            entity.Property(e => e.Descripcion)
                .HasColumnType("ntext")
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NumeroNivel).HasColumnName("numero_nivel");
            entity.Property(e => e.PuntosRequeridos).HasColumnName("puntos_requeridos");
        });

        modelBuilder.Entity<Notificacione>(entity =>
        {
            entity.HasKey(e => e.IdNotificacion).HasName("PK__notifica__8270F9A5887D7ED6");

            entity.ToTable("notificaciones");

            entity.HasIndex(e => e.FechaCreacion, "idx_fecha");

            entity.HasIndex(e => e.Leida, "idx_leida");

            entity.HasIndex(e => e.IdUsuario, "idx_usuario");

            entity.Property(e => e.IdNotificacion).HasColumnName("id_notificacion");
            entity.Property(e => e.Contenido)
                .HasColumnType("ntext")
                .HasColumnName("contenido");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Leida)
                .HasDefaultValue(false)
                .HasColumnName("leida");
            entity.Property(e => e.Tipo)
                .HasMaxLength(20)
                .HasColumnName("tipo");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__notificac__id_us__10566F31");
        });

        modelBuilder.Entity<Observacione>(entity =>
        {
            entity.HasKey(e => e.IdObservacion).HasName("PK__observac__4CA8E723C2148C75");

            entity.ToTable("observaciones");

            entity.HasIndex(e => e.IdEstudiante, "idx_estudiante");

            entity.HasIndex(e => e.FechaCreacion, "idx_fecha");

            entity.HasIndex(e => e.IdTutor, "idx_tutor");

            entity.Property(e => e.IdObservacion).HasColumnName("id_observacion");
            entity.Property(e => e.Contenido)
                .HasColumnType("ntext")
                .HasColumnName("contenido");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdTutor).HasColumnName("id_tutor");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.Observaciones)
                .HasForeignKey(d => d.IdEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__observaci__id_es__245D67DE");

            entity.HasOne(d => d.IdTutorNavigation).WithMany(p => p.Observaciones)
                .HasForeignKey(d => d.IdTutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__observaci__id_tu__236943A5");
        });

        modelBuilder.Entity<ProgresoTarjeta>(entity =>
        {
            entity.HasKey(e => e.IdProgreso).HasName("PK__progreso__D5CCDD4BAD01347A");

            entity.ToTable("progreso_tarjetas");

            entity.HasIndex(e => e.Completada, "idx_completada");

            entity.HasIndex(e => e.IdEstudiante, "idx_estudiante");

            entity.HasIndex(e => new { e.IdEstudiante, e.Completada }, "idx_progreso_estudiante_completada");

            entity.HasIndex(e => new { e.IdEstudiante, e.IdTarjeta }, "unique_estudiante_tarjeta").IsUnique();

            entity.Property(e => e.IdProgreso).HasColumnName("id_progreso");
            entity.Property(e => e.Completada)
                .HasDefaultValue(false)
                .HasColumnName("completada");
            entity.Property(e => e.FechaCompletada)
                .HasColumnType("datetime")
                .HasColumnName("fecha_completada");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdTarjeta).HasColumnName("id_tarjeta");
            entity.Property(e => e.Intentos)
                .HasDefaultValue(0)
                .HasColumnName("intentos");
            entity.Property(e => e.TiempoDedicadoMinutos)
                .HasDefaultValue(0)
                .HasColumnName("tiempo_dedicado_minutos");
            entity.Property(e => e.UltimaVisualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("ultima_visualizacion");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.ProgresoTarjeta)
                .HasForeignKey(d => d.IdEstudiante)
                .HasConstraintName("FK__progreso___id_es__74AE54BC");

            entity.HasOne(d => d.IdTarjetaNavigation).WithMany(p => p.ProgresoTarjeta)
                .HasForeignKey(d => d.IdTarjeta)
                .HasConstraintName("FK__progreso___id_ta__75A278F5");
        });

        modelBuilder.Entity<Tarjeta>(entity =>
        {
            entity.HasKey(e => e.IdTarjeta).HasName("PK__tarjetas__E92BCFEA59971CCE");

            entity.ToTable("tarjetas");

            entity.HasIndex(e => e.IdModulo, "idx_modulo");

            entity.HasIndex(e => e.Orden, "idx_orden");

            entity.HasIndex(e => new { e.IdModulo, e.Orden }, "idx_tarjetas_modulo_orden");

            entity.Property(e => e.IdTarjeta).HasColumnName("id_tarjeta");
            entity.Property(e => e.Descripcion)
                .HasColumnType("ntext")
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaActualizacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_actualizacion");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdModulo).HasColumnName("id_modulo");
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(255)
                .HasColumnName("imagen_url");
            entity.Property(e => e.Orden).HasColumnName("orden");
            entity.Property(e => e.Puntos)
                .HasDefaultValue(10)
                .HasColumnName("puntos");
            entity.Property(e => e.Titulo)
                .HasMaxLength(150)
                .HasColumnName("titulo");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .HasColumnName("video_url");

            entity.HasOne(d => d.IdModuloNavigation).WithMany(p => p.Tarjeta)
                .HasForeignKey(d => d.IdModulo)
                .HasConstraintName("FK__tarjetas__id_mod__6D0D32F4");
        });

        modelBuilder.Entity<TutorEstudiante>(entity =>
        {
            entity.HasKey(e => e.IdAsignacion).HasName("PK__tutor_es__C3F7F9665B8119FE");

            entity.ToTable("tutor_estudiante");

            entity.HasIndex(e => e.IdEstudiante, "idx_estudiante");

            entity.HasIndex(e => e.IdTutor, "idx_tutor");

            entity.HasIndex(e => new { e.IdTutor, e.IdEstudiante }, "unique_tutor_estudiante").IsUnique();

            entity.Property(e => e.IdAsignacion).HasColumnName("id_asignacion");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_asignacion");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdTutor).HasColumnName("id_tutor");

            entity.HasOne(d => d.IdEstudianteNavigation).WithMany(p => p.TutorEstudiantes)
                .HasForeignKey(d => d.IdEstudiante)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tutor_est__id_es__1EA48E88");

            entity.HasOne(d => d.IdTutorNavigation).WithMany(p => p.TutorEstudiantes)
                .HasForeignKey(d => d.IdTutor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__tutor_est__id_tu__1DB06A4F");
        });

        modelBuilder.Entity<Tutore>(entity =>
        {
            entity.HasKey(e => e.IdTutor).HasName("PK__tutores__C176593D88CEE38D");

            entity.ToTable("tutores");

            entity.Property(e => e.IdTutor).HasColumnName("id_tutor");
            entity.Property(e => e.Descripcion)
                .HasColumnType("ntext")
                .HasColumnName("descripcion");
            entity.Property(e => e.Especialidad)
                .HasMaxLength(100)
                .HasColumnName("especialidad");
            entity.Property(e => e.FechaVinculacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_vinculacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tutores)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__tutores__id_usua__59FA5E80");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04ADED38C79F");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Email, "UQ__usuarios__AB6E61645E27FDA2").IsUnique();

            entity.HasIndex(e => e.Email, "idx_email");

            entity.HasIndex(e => e.TipoUsuario, "idx_tipo_usuario");

            entity.HasIndex(e => e.Estado, "idx_usuarios_estado");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("activo")
                .HasColumnName("estado");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.TipoUsuario)
                .HasMaxLength(20)
                .HasColumnName("tipo_usuario");
            entity.Property(e => e.UltimaConexion)
                .HasColumnType("datetime")
                .HasColumnName("ultima_conexion");
        });

        modelBuilder.Entity<VistaProgresoModulo>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_progreso_modulos");

            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdModulo).HasColumnName("id_modulo");
            entity.Property(e => e.Modulo)
                .HasMaxLength(150)
                .HasColumnName("modulo");
            entity.Property(e => e.PorcentajeCompletado)
                .HasColumnType("numeric(26, 12)")
                .HasColumnName("porcentaje_completado");
            entity.Property(e => e.TarjetasCompletadas).HasColumnName("tarjetas_completadas");
            entity.Property(e => e.TotalTarjetas).HasColumnName("total_tarjetas");
        });

        modelBuilder.Entity<VistaResumenEstudiante>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vista_resumen_estudiantes");

            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .HasColumnName("apellido");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.EstadoActividad)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("estado_actividad");
            entity.Property(e => e.IdEstudiante).HasColumnName("id_estudiante");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.LeccionesCompletadas).HasColumnName("lecciones_completadas");
            entity.Property(e => e.NivelActual).HasColumnName("nivel_actual");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.NombreNivel)
                .HasMaxLength(100)
                .HasColumnName("nombre_nivel");
            entity.Property(e => e.ProgresoNivelActual)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("progreso_nivel_actual");
            entity.Property(e => e.PuntosTotales).HasColumnName("puntos_totales");
            entity.Property(e => e.TiempoTotalMinutos).HasColumnName("tiempo_total_minutos");
            entity.Property(e => e.UltimaConexion)
                .HasColumnType("datetime")
                .HasColumnName("ultima_conexion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
