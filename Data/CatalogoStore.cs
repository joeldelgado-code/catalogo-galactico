using CatalogoGalactico.Models;

namespace CatalogoGalactico.Data;

public static class CatalogoStore
{
    public static List<Personaje> Personajes { get; } = new()
    {
        new Personaje
        {
            Id = 1,
            Nombre = "Luke Skywalker",
            Especie = "Humano",
            Faccion = Faccion.Rebelde,
            Afiliacion = "Alianza Rebelde",
            Estado = Estado.Vivo,
            FuerzaSensitivo = true
        },

        new Personaje
        {
            Id = 2,
            Nombre = "Darth Vader",
            Especie = "Humano",
            Faccion = Faccion.Imperio,
            Afiliacion = "Imperio Galáctico",
            Estado = Estado.Muerto,
            FuerzaSensitivo = true
        },

        new Personaje
        {
            Id = 3,
            Nombre = "Han Solo",
            Especie = "Humano",
            Faccion = Faccion.Rebelde,
            Afiliacion = "Alianza Rebelde",
            Estado = Estado.Vivo,
            FuerzaSensitivo = false
        },

        new Personaje
        {
            Id = 4,
            Nombre = "Boba Fett",
            Especie = "Humano",
            Faccion = Faccion.Neutral,
            Afiliacion = "Cazarrecompensas",
            Estado = Estado.Vivo,
            FuerzaSensitivo = false
        },

        new Personaje
        {
            Id = 5,
            Nombre = "Yoda",
            Especie = "Desconocida",
            Faccion = Faccion.Rebelde,
            Afiliacion = "Orden Jedi",
            Estado = Estado.Muerto,
            FuerzaSensitivo = true
        }
    };

    public static List<CardPersonaje> CardsPersonaje { get; } = new()
{
    new CardPersonaje
    {
        Id = 1,
        PersonajeId = 1,
        Poder = 95,
        HabilidadEspecial = "Maestro Jedi",
        Arma = "Sable de luz",
        NivelPeligrosidad = 8,
        ImagenUrl = "https://example.com/luke.jpg"
    },

    new CardPersonaje
    {
        Id = 2,
        PersonajeId = 2,
        Poder = 100,
        HabilidadEspecial = "Fuerza del lado oscuro",
        Arma = "Sable de luz rojo",
        NivelPeligrosidad = 10,
        ImagenUrl = "https://example.com/vader.jpg"
    },

    new CardPersonaje
    {
        Id = 3,
        PersonajeId = 3,
        Poder = 70,
        HabilidadEspecial = "Piloto experto",
        Arma = "Bláster DL-44",
        NivelPeligrosidad = 6,
        ImagenUrl = "https://example.com/han.jpg"
    },

    new CardPersonaje
    {
        Id = 4,
        PersonajeId = 4,
        Poder = 80,
        HabilidadEspecial = "Cazarrecompensas experto",
        Arma = "Bláster EE-3",
        NivelPeligrosidad = 8,
        ImagenUrl = "https://example.com/boba.jpg"
    },

    new CardPersonaje
    {
        Id = 5,
        PersonajeId = 5,
        Poder = 98,
        HabilidadEspecial = "Maestro Jedi legendario",
        Arma = "Sable de luz verde",
        NivelPeligrosidad = 9,
        ImagenUrl = "https://example.com/yoda.jpg"
    }
};

   public static List<Evento> Eventos { get; } = new()
{
    new Evento
    {
        Id = 1,
        Nombre = "Batalla de Yavin",
        Fecha = 0,
        Ubicacion = "Yavin 4",
        Descripcion = "La Alianza Rebelde ataca la primera Estrella de la Muerte.",
        Participantes = new List<int> { 1, 3 },
        Resultado = "Victoria Rebelde"
    },

    new Evento
    {
        Id = 2,
        Nombre = "Batalla de Hoth",
        Fecha = 3,
        Ubicacion = "Hoth",
        Descripcion = "El Imperio ataca la base de la Alianza Rebelde.",
        Participantes = new List<int> { 1, 2 },
        Resultado = "Victoria Imperial"
    },

    new Evento
    {
        Id = 3,
        Nombre = "Batalla de Endor",
        Fecha = 4,
        Ubicacion = "Endor",
        Descripcion = "La Alianza Rebelde intenta destruir la segunda Estrella de la Muerte.",
        Participantes = new List<int> { 1, 2, 4 },
        Resultado = "Victoria Rebelde"
    }
};
}