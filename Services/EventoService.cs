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
}