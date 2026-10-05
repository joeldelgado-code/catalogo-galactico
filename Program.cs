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
//---------------
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
//----------------
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
//--------------------------------------
app.MapGet("/", () => Results.Redirect("/swagger"))
   .ExcludeFromDescription();

app.Run();