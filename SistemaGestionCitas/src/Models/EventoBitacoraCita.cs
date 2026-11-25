namespace SistemaGestionCitas.src.Models;

internal sealed record EventoBitacoraCita(DateTime FechaEvento, string NombreUsuario, string Accion,
    int IdCita, string? EstadoAnterior, string EstadoNuevo);
