using CatalogoGalactico.Data;
using CatalogoGalactico.Models;
using CatalogoGalactico.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//-------------------------------------------
app.MapGet("/personajes", (Faccion? faccion, bool? fuerzaSensitivo) =>
{
    var personajes = CatalogoStore.Personajes.AsEnumerable();

    if (faccion.HasValue)
    {
        personajes = personajes
            .Where(p => p.Faccion == faccion.Value);
    }

    if (fuerzaSensitivo.HasValue)
    {
        personajes = personajes
            .Where(p => p.FuerzaSensitivo == fuerzaSensitivo.Value);
    }

    return Results.Ok(personajes.ToList());
})
.WithName("ObtenerPersonajes")
.WithSummary("Lista personajes y permite filtrar por facción y sensibilidad a la Fuerza.")
.WithTags("Personajes")
.Produces<List<Personaje>>(200);
//------------------------------------------------------------
app.MapGet("/personajes/{id:int}", (int id) =>
{
    var personaje = CatalogoStore.Personajes
        .FirstOrDefault(p => p.Id == id);

    return personaje is null
        ? Results.NotFound()
        : Results.Ok(personaje);
})
.WithName("ObtenerPersonajePorId")
.WithSummary("Obtiene un personaje específico por su id.")
.WithTags("Personajes")
.Produces<Personaje>(200)
.Produces(404);
//-----------------------------------------------
app.MapPost("/personajes", (Personaje personaje) =>
{
    var nuevoId = CatalogoStore.Personajes.Count == 0
        ? 1
        : CatalogoStore.Personajes.Max(p => p.Id) + 1;

    personaje = new Personaje
    {
        Id = nuevoId,
        Nombre = personaje.Nombre,
        Especie = personaje.Especie,
        Faccion = personaje.Faccion,
        Afiliacion = personaje.Afiliacion,
        Estado = personaje.Estado,
        FuerzaSensitivo = personaje.FuerzaSensitivo
    };

    CatalogoStore.Personajes.Add(personaje);

    return Results.Created($"/personajes/{personaje.Id}", personaje);
})
.WithName("CrearPersonaje")
.WithSummary("Crea un nuevo personaje.")
.WithTags("Personajes")
.Produces<Personaje>(201);
//------------------------------------------------------
app.MapPut("/personajes/{id:int}", (int id, Personaje datos) =>
{
    var personaje = CatalogoStore.Personajes
        .FirstOrDefault(p => p.Id == id);

    if (personaje is null)
    {
        return Results.NotFound();
    }

    personaje.Nombre = datos.Nombre;
    personaje.Especie = datos.Especie;
    personaje.Faccion = datos.Faccion;
    personaje.Afiliacion = datos.Afiliacion;
    personaje.Estado = datos.Estado;
    personaje.FuerzaSensitivo = datos.FuerzaSensitivo;

    return Results.Ok(personaje);
})
.WithName("ActualizarPersonaje")
.WithSummary("Actualiza un personaje existente.")
.WithTags("Personajes")
.Produces<Personaje>(200)
.Produces(404);
//------------------------------------------------------
app.MapDelete("/personajes/{id:int}", (int id) =>
{
    var personaje = CatalogoStore.Personajes
        .FirstOrDefault(p => p.Id == id);

    if (personaje is null)
    {
        return Results.NotFound();
    }

    CatalogoStore.Personajes.Remove(personaje);

    return Results.NoContent();
})
.WithName("EliminarPersonaje")
.WithSummary("Elimina un personaje existente.")
.WithTags("Personajes")
.Produces(204)
.Produces(404);
//----------------------------------------------------
// GET /personajes/{id}/eventos
//----------------------------------------------------
app.MapGet("/personajes/{id:int}/eventos", (int id) =>
{
    var personaje = CatalogoStore.Personajes
        .FirstOrDefault(p => p.Id == id);

    if (personaje is null)
    {
        return Results.NotFound();
    }

    var eventos = CatalogoStore.Eventos
        .Where(e => e.Participantes.Contains(id))
        .ToList();

    return Results.Ok(eventos);
})
.WithName("ObtenerEventosPorPersonaje")
.WithSummary("Obtiene los eventos en los que participó un personaje.")
.WithTags("Personajes")
.Produces<List<Evento>>(200)
.Produces(404);
//-----------------------------------------
//----------------------------------------------------
app.MapGet("/personajes/{id:int}/con-card", (int id) =>
{
    var personaje = CatalogoStore.Personajes
        .FirstOrDefault(p => p.Id == id);

    if (personaje is null)
    {
        return Results.NotFound();
    }

    var card = CatalogoStore.CardsPersonaje
        .FirstOrDefault(c => c.PersonajeId == id);

    return Results.Ok(new
    {
        Personaje = personaje,
        Card = card
    });
})
.WithName("ObtenerCardDePersonaje")
.WithSummary("Obtiene un personaje específico junto a su card.")
.WithTags("Personajes")
.Produces(200, typeof(object)) 
.Produces(404);


//----------------------------------------------------
// GET /personajes/ranking
//----------------------------------------------------
app.MapGet("/personajes/ranking", (string? por) =>
{
    if (por != "poder")
    {
        return Results.BadRequest(
            "El parámetro 'por' debe ser 'poder'.");
    }

    var ranking = EventoService.ObtenerRankingPorPoder();

    return Results.Ok(ranking);
})
.WithName("RankingPersonajes")
.WithSummary("Obtiene el ranking de personajes por poder.")
.WithTags("Personajes")
.Produces(200)
.Produces(400);
//------------------------------------------------------
//endpoints cards
//---------------------------------------------------
app.MapGet("/cards", () => CatalogoStore.CardsPersonaje)
    .WithName("ObtenerCards")
    .WithSummary("Lista todas las cards de personajes.")
    .WithTags("CardsPersonaje")
    .Produces<List<CardPersonaje>>(200);
//----------------------------------------------------
app.MapGet("/cards/{id:int}", (int id) =>
{
    var card = CatalogoStore.CardsPersonaje
        .FirstOrDefault(c => c.Id == id);

    return card is null
        ? Results.NotFound()
        : Results.Ok(card);
})
.WithName("ObtenerCardPorId")
.WithSummary("Obtiene una card específica por su id.")
.WithTags("CardsPersonaje")
.Produces<CardPersonaje>(200)
.Produces(404);
//---------------------------------------------------
//----------------------------------------------------
app.MapPost("/cards", (CardPersonaje card) =>
{
    var nuevoId = CatalogoStore.CardsPersonaje.Count == 0
        ? 1
        : CatalogoStore.CardsPersonaje.Max(c => c.Id) + 1;

    card = new CardPersonaje
    {
        Id = nuevoId,
        PersonajeId = card.PersonajeId,
        Poder = card.Poder,
        HabilidadEspecial = card.HabilidadEspecial,
        Arma = card.Arma,
        NivelPeligrosidad = card.NivelPeligrosidad,
        ImagenUrl = card.ImagenUrl
    };

    CatalogoStore.CardsPersonaje.Add(card);

    return Results.Created($"/cards/{card.Id}", card);
})
.WithName("CrearCard")
.WithSummary("Crea una nueva card de personaje.")
.WithTags("CardsPersonaje")
.Produces<CardPersonaje>(201);
//----------------------------------------------------
//----------------------------------------------------
app.MapPut("/cards/{id:int}", (int id, CardPersonaje datos) =>
{
    var card = CatalogoStore.CardsPersonaje
        .FirstOrDefault(c => c.Id == id);

    if (card is null)
    {
        return Results.NotFound();
    }

    card.PersonajeId = datos.PersonajeId;
    card.Poder = datos.Poder;
    card.HabilidadEspecial = datos.HabilidadEspecial;
    card.Arma = datos.Arma;
    card.NivelPeligrosidad = datos.NivelPeligrosidad;
    card.ImagenUrl = datos.ImagenUrl;

    return Results.Ok(card);
})
.WithName("ActualizarCard")
.WithSummary("Actualiza una card de personaje existente.")
.WithTags("CardsPersonaje")
.Produces<CardPersonaje>(200)
.Produces(404);
//----------------------------------------------------
//endpoints de EVENTOS
//----------------------------------------------------
app.MapGet("/eventos", () => CatalogoStore.Eventos)
    .WithName("ObtenerEventos")
    .WithSummary("Lista todos los eventos registrados.")
    .WithTags("Eventos")
    .Produces<List<Evento>>(200);
//----------------------------------------------------
//----------------------------------------------------
app.MapGet("/eventos/{id:int}", (int id) =>
{
    var evento = CatalogoStore.Eventos
        .FirstOrDefault(e => e.Id == id);

    return evento is null
        ? Results.NotFound()
        : Results.Ok(evento);
})
.WithName("ObtenerEventoPorId")
.WithSummary("Obtiene un evento específico por su id.")
.WithTags("Eventos")
.Produces<Evento>(200)
.Produces(404);
//----------------------------------------------------
// POST /eventos
//----------------------------------------------------
app.MapPost("/eventos", (Evento evento) =>
{
    if (!EventoService.TieneSuficientesParticipantes(evento.Participantes))
    {
        return Results.BadRequest("El evento debe tener al menos 2 participantes.");
    }

    if (!EventoService.ParticipantesExisten(evento.Participantes))
    {
        return Results.BadRequest("Uno o más participantes no existen.");
    }

    if (!EventoService.ParticipantesEstanVivos(evento.Participantes))
    {
        return Results.BadRequest("Uno o más participantes están muertos.");
    }

    if (!EventoService.NoHayParticipantesRepetidos(evento.Participantes))
    {
        return Results.BadRequest("No se permiten participantes repetidos.");
    }
    if (!EventoService.ParticipantesRespetanFechaDeMuerte(evento))
    {
        return Results.BadRequest("Un personaje no puede participar después de su muerte.");
    }
    if (!EventoService.FallecidosSonParticipantes(evento))
    {
        return Results.BadRequest("Los fallecidos deben ser participantes del evento.");
    }

    var nuevoId = CatalogoStore.Eventos.Count == 0
        ? 1
        : CatalogoStore.Eventos.Max(e => e.Id) + 1;

    evento = new Evento
{
    Id = nuevoId,
    Nombre = evento.Nombre,
    Fecha = evento.Fecha,
    Ubicacion = evento.Ubicacion,
    Descripcion = evento.Descripcion,
    Participantes = evento.Participantes,
    Fallecidos = evento.Fallecidos,
    Resultado = evento.Resultado
};

    CatalogoStore.Eventos.Add(evento);

    return Results.Created($"/eventos/{evento.Id}", evento);
})
.WithName("CrearEvento")
.WithSummary("Crea un nuevo evento.")
.WithTags("Eventos")
.Produces<Evento>(201)
.Produces(400);
//----------------------------------------------------
//----------------------------------------------------
app.MapPut("/eventos/{id:int}", (int id, Evento datos) =>
{
    var evento = CatalogoStore.Eventos
        .FirstOrDefault(e => e.Id == id);

    if (evento is null)
    {
        return Results.NotFound();
    }

    evento.Nombre = datos.Nombre;
    evento.Fecha = datos.Fecha;
    evento.Ubicacion = datos.Ubicacion;
    evento.Descripcion = datos.Descripcion;
    evento.Participantes = datos.Participantes;
    evento.Fallecidos = datos.Fallecidos;
    evento.Resultado = datos.Resultado;
    return Results.Ok(evento);
})
.WithName("ActualizarEvento")
.WithSummary("Actualiza un evento existente.")
.WithTags("Eventos")
.Produces<Evento>(200)
.Produces(404);
//----------------------------------------------------
// POST /eventos/{id}/simular
//----------------------------------------------------
app.MapPost("/eventos/{id:int}/simular", (int id) =>
    {
        var evento = CatalogoStore.Eventos
            .FirstOrDefault(e => e.Id == id);

        if (evento is null)
        {
            return Results.NotFound();
        }

        if (!EventoService.TieneSuficientesParticipantes(evento.Participantes))
        {
            return Results.BadRequest(
                "El evento debe tener al menos 2 participantes.");
        }

        if (!EventoService.TodosTienenCard(evento.Participantes))
        {
            return Results.BadRequest(
                "Todos los participantes deben tener una card.");
        }

        var resultado = EventoService.SimularEvento(evento);

        EventoService.ActualizarFallecidos(evento);

        return Results.Ok(resultado);
    })
.WithName("SimularEvento")
.WithSummary("Simula el resultado de un evento.")
.WithTags("Eventos")
.Produces(200)
.Produces(400)
.Produces(404);
//---------------------------------------------
// GET /eventos/{id}/mvp
//----------------------------------------------------
app.MapGet("/eventos/{id:int}/mvp", (int id) =>
{
    var evento = CatalogoStore.Eventos
        .FirstOrDefault(e => e.Id == id);

    if (evento is null)
    {
        return Results.NotFound();
    }

    var mvp = EventoService.ObtenerMvp(evento);

    if (mvp is null)
    {
        return Results.BadRequest(
            "Ningún participante tiene una card.");
    }

    return Results.Ok(mvp);
})
.WithName("ObtenerMvp")
.WithSummary("Obtiene el personaje con mayor poder de un evento.")
.WithTags("Eventos")
.Produces(200)
.Produces(400)
.Produces(404);
//-------------------------------------------------------
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();