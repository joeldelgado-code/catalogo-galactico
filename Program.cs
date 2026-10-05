using CatalogoGalactico.Data;
using CatalogoGalactico.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/personajes", () => CatalogoStore.Personajes)
    .WithName("ObtenerPersonajes")
    .WithSummary("Lista todos los personajes registrados.")
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
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();