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
    public static object SimularEvento(Evento evento)
    {
        var participantes = CatalogoStore.Personajes
            .Where(p => evento.Participantes.Contains(p.Id))
            .ToList();

        var resultados = new Dictionary<Faccion, int>();

        foreach (var faccion in Enum.GetValues<Faccion>())
        {
            var poder = 0;

            foreach (var personaje in participantes.Where(p => p.Faccion == faccion))
            {
                var card = CatalogoStore.CardsPersonaje
                    .FirstOrDefault(c => c.PersonajeId == personaje.Id);

                if (card is not null)
                {
                    poder += card.Poder;
                }
            }

            if (poder > 0)
            {
                resultados[faccion] = poder;
            }
        }

        var random = new Random();

        var poderesFinales = resultados.ToDictionary(
            x => x.Key,
            x => x.Value * (random.Next(90, 111) / 100.0)
        );

        var ganador = poderesFinales
            .OrderByDescending(x => x.Value)
            .First();

        return new
        {
            Evento = evento.Nombre,
            PoderPorFaccion = poderesFinales,
            Ganador = ganador.Key.ToString(),
            PoderGanador = ganador.Value
        };
    }
    public static void ActualizarFallecidos(Evento evento)
    {
        foreach (var fallecidoId in evento.Fallecidos)
        {
            var personaje = CatalogoStore.Personajes
                .FirstOrDefault(p => p.Id == fallecidoId);

            if (personaje is not null)
            {
                personaje.Estado = Estado.Muerto;
            }
        }
    }
    public static List<object> ObtenerRankingPorPoder()
    {
        var ranking = CatalogoStore.Personajes
            .Select(personaje =>
            {
                var card = CatalogoStore.CardsPersonaje
                    .FirstOrDefault(c => c.PersonajeId == personaje.Id);

                return new
                {
                    Personaje = personaje.Nombre,
                    Poder = card?.Poder ?? 0
                };
            })
            .OrderByDescending(x => x.Poder)
            .ToList();

        return ranking.Cast<object>().ToList();
    }
    public static object? ObtenerMvp(Evento evento)
    {
        var mvp = CatalogoStore.Personajes
            .Where(p => evento.Participantes.Contains(p.Id))
            .Select(personaje =>
            {
                var card = CatalogoStore.CardsPersonaje
                    .FirstOrDefault(c => c.PersonajeId == personaje.Id);

                return new
                {
                    Personaje = personaje,
                    Card = card
                };
            })
            .Where(x => x.Card is not null)
            .OrderByDescending(x => x.Card!.Poder)
            .FirstOrDefault();

        if (mvp is null)
        {
            return null;
        }

        return new
        {
            Personaje = mvp.Personaje.Nombre,
            Poder = mvp.Card!.Poder
        };
    }
}