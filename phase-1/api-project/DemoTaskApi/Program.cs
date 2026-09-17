using DemoTaskApi.Models;
using DemoTaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ITaskService, TaskService>();
builder.Services.AddSingleton<ITaskOwnerService, TaskOwnerService>();

var app = builder.Build();

// GET /tasks — list all tasks
app.MapGet("/tasks", (ITaskService service) =>
    Results.Ok(service.GetAll()));

// GET /tasks/{id} — get a single task
app.MapGet("/tasks/{id:int}", (int id, ITaskService service) =>
{
    var task = service.GetById(id);
    return task is not null ? Results.Ok(task) : Results.NotFound();
});

// POST /tasks — create a task (optionally assigned to an owner)
app.MapPost("/tasks", (CreateTaskRequest request, ITaskService service, ITaskOwnerService owners) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required");
    }

    if (request.OwnerId is int ownerId && owners.GetById(ownerId) is null)
    {
        return Results.BadRequest($"Owner {ownerId} does not exist");
    }

    var task = service.Create(request.Title, request.OwnerId);
    return Results.Created($"/tasks/{task.Id}", task);
});

// POST /owners — create an owner
app.MapPost("/owners", (CreateTaskOwnerRequest request, ITaskOwnerService owners) =>
{
    if (string.IsNullOrWhiteSpace(request.Name))
    {
        return Results.BadRequest("Name is required");
    }

    var owner = owners.Create(request.Name, request.Email);
    return Results.Created($"/owners/{owner.Id}", ToDto(owner));
});

// GET /owners/{id} — get a single owner
app.MapGet("/owners/{id:int}", (int id, ITaskOwnerService owners) =>
{
    var owner = owners.GetById(id);
    return owner is not null ? Results.Ok(ToDto(owner)) : Results.NotFound();
});

// GET /owners/{id}/tasks — list the tasks belonging to an owner
app.MapGet("/owners/{id:int}/tasks", (int id, ITaskOwnerService owners, ITaskService tasks) =>
{
    if (owners.GetById(id) is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(tasks.GetByOwner(id));
});

app.Run();

// Map the storage model to the public DTO returned by the API.
static TaskOwnerDto ToDto(TaskOwner owner) => new(owner.Id, owner.Name, owner.Email);

// Make Program accessible for integration tests
public partial class Program { }
