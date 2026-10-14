using System.Net;
using System.Net.Http.Json;
using DemoTaskApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DemoTaskApi.Tests;

public class TaskEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaskEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("InProgress", false)]
    [InlineData("Done", true)]
    public async Task PatchTaskStatus_UpdatesStatus(string status, bool isComplete)
    {
        var created = await (await _client.PostAsJsonAsync("/tasks", new { Title = "Status task" }))
            .Content.ReadFromJsonAsync<TaskItem>();

        var response = await _client.PatchAsJsonAsync($"/tasks/{created!.Id}/status", new { Status = status });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal(Enum.Parse<TaskItemStatus>(status), updated!.Status);
        Assert.Equal(isComplete, updated.IsComplete);
    }

    [Fact]
    public async Task PatchTaskStatus_UnknownTask_ReturnsNotFound()
    {
        var response = await _client.PatchAsJsonAsync("/tasks/9999/status", new { Status = "Done" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchTaskStatus_InvalidStatus_ReturnsBadRequest()
    {
        var created = await (await _client.PostAsJsonAsync("/tasks", new { Title = "Bad status" }))
            .Content.ReadFromJsonAsync<TaskItem>();

        var response = await _client.PatchAsJsonAsync($"/tasks/{created!.Id}/status", new { Status = "Nope" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTasks_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostTask_WithTitle_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { Title = "Demo task" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);
        Assert.Equal("Demo task", task.Title);
    }

    [Fact]
    public async Task PostTask_EmptyTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/tasks", new { Title = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTaskById_NonExistent_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/tasks/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
