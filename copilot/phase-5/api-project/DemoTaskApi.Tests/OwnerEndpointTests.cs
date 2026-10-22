using System.Net;
using System.Net.Http.Json;
using DemoTaskApi.Models;
using DemoTaskApi.Tests.MockData;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DemoTaskApi.Tests;

public class OwnerEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OwnerEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostOwner_WithNameAndEmail_ReturnsCreatedOwner()
    {
        // Arrange — build the request body from the mock factory, override only what matters
        var sample = TaskOwnerMockData.CreateMockOwner(name: "Grace Hopper", email: "grace@example.com");

        // Act
        var response = await _client.PostAsJsonAsync(
            "/owners",
            new CreateTaskOwnerRequest(sample.Name, sample.Email));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var owner = await response.Content.ReadFromJsonAsync<TaskOwnerDto>();
        Assert.NotNull(owner);
        Assert.Equal("Grace Hopper", owner.Name);
        Assert.Equal("grace@example.com", owner.Email);
        Assert.True(owner.Id > 0);
    }

    [Fact]
    public async Task PostOwner_EmptyName_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/owners",
            new CreateTaskOwnerRequest("", "noname@example.com"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetOwnerById_NonExistent_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/owners/99999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetOwnerTasks_AfterAssigningTask_ReturnsThatTask()
    {
        // Arrange — create an owner, then a task assigned to that owner
        var ownerResponse = await _client.PostAsJsonAsync(
            "/owners",
            new CreateTaskOwnerRequest("Ada Lovelace", "ada@example.com"));
        var owner = await ownerResponse.Content.ReadFromJsonAsync<TaskOwnerDto>();
        Assert.NotNull(owner);

        await _client.PostAsJsonAsync("/tasks", new CreateTaskRequest("Owned task", owner.Id));

        // Act
        var tasksResponse = await _client.GetAsync($"/owners/{owner.Id}/tasks");

        // Assert
        Assert.Equal(HttpStatusCode.OK, tasksResponse.StatusCode);
        var tasks = await tasksResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal("Owned task", tasks[0].Title);
        Assert.Equal(owner.Id, tasks[0].OwnerId);
    }

    [Fact]
    public async Task GetOwnerTasks_NonExistentOwner_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/owners/99999/tasks");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
