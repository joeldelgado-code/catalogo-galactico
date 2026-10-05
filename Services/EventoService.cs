using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

namespace CatalogoGalactico.Services;

public static class EventoService
{
    public static bool ParticipantesExisten(List<int> participantes)
    {
        foreach (var participanteId in participantes)
        {
            var existe = CatalogoStore.Personajes
                .Any(p => p.Id == participanteId);

            if (!existe)
            {
                return false;
            }
        }

        return true;
    }
    public static bool ParticipantesEstanVivos(List<int> participantes)
    {
        foreach (var participanteId in participantes)
        {
            var personaje = CatalogoStore.Personajes
                .FirstOrDefault(p => p.Id == participanteId);

            if (personaje is not null && personaje.Estado == Estado.Muerto)
            {
                return false;
            }
        }

        return true;
    }
    public static bool TieneSuficientesParticipantes(List<int> participantes)
    {
        return participantes.Count >= 2;
    }

    public static bool NoHayParticipantesRepetidos(List<int> participantes)
    {
        return participantes.Distinct().Count() == participantes.Count;
    }
    public static bool TodosTienenCard(List<int> participantes)
    {
        foreach (var participanteId in participantes)
        {
            var tieneCard = CatalogoStore.CardsPersonaje
                .Any(c => c.PersonajeId == participanteId);

            if (!tieneCard)
            {
                return false;
            }
        }

        return true;
    }
    public static int? ObtenerFechaMuerte(int personajeId)
    {
        var eventoMuerte = CatalogoStore.Eventos
            .Where(e => e.Fallecidos.Contains(personajeId))
            .OrderBy(e => e.Fecha)
            .FirstOrDefault();

        return eventoMuerte?.Fecha;
    }
    public static bool ParticipantesRespetanFechaDeMuerte(Evento evento)
    {
        foreach (var participanteId in evento.Participantes)
        {
            var fechaMuerte = ObtenerFechaMuerte(participanteId);

            if (fechaMuerte.HasValue && evento.Fecha > fechaMuerte.Value)
            {
                return false;
            }
        }

        return true;
    }
    public static bool FallecidosSonParticipantes(Evento evento)
    {
        foreach (var fallecidoId in evento.Fallecidos)
        {
            if (!evento.Participantes.Contains(fallecidoId))
            {
                return false;
            }
        }

        return true;
    }
}