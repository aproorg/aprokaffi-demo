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

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task PatchTask_ExistingTask_ReturnsOkWithCompletedTaskAndPreservesFields(bool withOwner)
    {
        int? ownerId = null;
        if (withOwner)
        {
            var owner = await CreateOwnerAsync("Completion owner");
            ownerId = owner.Id;
        }

        const string title = "  Finish report  ";
        var created = await CreateTaskAsync(title, ownerId);
        Assert.False(created.IsComplete);
        var idBeforeCompletion = created.Id;
        var titleBeforeCompletion = created.Title;
        var ownerIdBeforeCompletion = created.OwnerId;

        var response = await _client.PatchAsync($"/tasks/{idBeforeCompletion}/complete", content: null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        var completed = await response.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(completed, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);

        var getResponse = await _client.GetAsync($"/tasks/{idBeforeCompletion}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var found = await getResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(found, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);
    }

    [Fact]
    public async Task PatchTask_AlreadyCompletedTask_ReturnsOkWithUnchangedCompletedTask()
    {
        var owner = await CreateOwnerAsync("Repeat completion owner");
        var created = await CreateTaskAsync("  Finish report  ", owner.Id);
        var idBeforeCompletion = created.Id;
        var titleBeforeCompletion = created.Title;
        var ownerIdBeforeCompletion = created.OwnerId;

        var firstResponse = await _client.PatchAsync($"/tasks/{idBeforeCompletion}/complete", content: null);
        Assert.Equal(HttpStatusCode.OK, firstResponse.StatusCode);
        var firstCompletion = await firstResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(firstCompletion, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);

        var secondResponse = await _client.PatchAsync($"/tasks/{idBeforeCompletion}/complete", content: null);
        Assert.Equal(HttpStatusCode.OK, secondResponse.StatusCode);
        var secondCompletion = await secondResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(secondCompletion, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);

        var allResponse = await _client.GetAsync("/tasks");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
        var allTasks = await allResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(allTasks);
        var matchingTasks = allTasks.Where(task => task.Id == idBeforeCompletion).ToArray();
        Assert.Single(matchingTasks);
        AssertTask(matchingTasks[0], idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public async Task PatchTask_MissingOrNonpositiveId_ReturnsNotFoundWithoutChangingTasks(int id)
    {
        var sentinel = await CreateTaskAsync("Sentinel task");
        var sentinelId = sentinel.Id;
        var sentinelTitle = sentinel.Title;
        var sentinelOwnerId = sentinel.OwnerId;
        var sentinelIsComplete = sentinel.IsComplete;

        var response = await _client.PatchAsync($"/tasks/{id}/complete", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());

        var missingGetResponse = await _client.GetAsync($"/tasks/{id}");
        Assert.Equal(HttpStatusCode.NotFound, missingGetResponse.StatusCode);

        var sentinelGetResponse = await _client.GetAsync($"/tasks/{sentinelId}");
        Assert.Equal(HttpStatusCode.OK, sentinelGetResponse.StatusCode);
        var sentinelAfterPatch = await sentinelGetResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(sentinelAfterPatch, sentinelId, sentinelTitle, sentinelOwnerId, sentinelIsComplete);
    }

    [Theory]
    [InlineData("not-an-int")]
    [InlineData("2147483648")]
    [InlineData("-2147483649")]
    public async Task PatchTask_NonIntegerOrOutOfRangeId_ReturnsNotFound(string idSegment)
    {
        var response = await _client.PatchAsync($"/tasks/{idSegment}/complete", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(string.Empty, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task PatchTask_CompletionIsVisibleInTaskAndOwnerQueriesWithoutChangingOtherTasks()
    {
        var ownerA = await CreateOwnerAsync("Completion owner A");
        var ownerB = await CreateOwnerAsync("Completion owner B");
        const string targetTitle = "  Finish report  ";
        var target = await CreateTaskAsync(targetTitle, ownerA.Id);
        var siblingA = await CreateTaskAsync("Owner A sibling", ownerA.Id);
        var taskB = await CreateTaskAsync("Owner B task", ownerB.Id);
        var unowned = await CreateTaskAsync("Unowned task");

        var targetId = target.Id;
        var targetTitleBeforeCompletion = target.Title;
        var targetOwnerId = target.OwnerId;
        var siblingSnapshot = (siblingA.Id, siblingA.Title, siblingA.OwnerId, siblingA.IsComplete);
        var taskBSnapshot = (taskB.Id, taskB.Title, taskB.OwnerId, taskB.IsComplete);
        var unownedSnapshot = (unowned.Id, unowned.Title, unowned.OwnerId, unowned.IsComplete);
        var arrangedIds = new[] { target.Id, siblingA.Id, taskB.Id, unowned.Id };

        var patchResponse = await _client.PatchAsync($"/tasks/{targetId}/complete", content: null);
        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        var targetGetResponse = await _client.GetAsync($"/tasks/{targetId}");
        Assert.Equal(HttpStatusCode.OK, targetGetResponse.StatusCode);
        var targetFromGet = await targetGetResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(targetFromGet, targetId, targetTitleBeforeCompletion, targetOwnerId, true);

        var allResponse = await _client.GetAsync("/tasks");
        Assert.Equal(HttpStatusCode.OK, allResponse.StatusCode);
        var allTasks = await allResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(allTasks);
        var arrangedTasks = allTasks.Where(task => arrangedIds.Contains(task.Id)).ToArray();
        Assert.Equal(arrangedIds, arrangedTasks.Select(task => task.Id).ToArray());
        Assert.Equal(4, arrangedTasks.Length);
        AssertTask(arrangedTasks[0], targetId, targetTitleBeforeCompletion, targetOwnerId, true);
        AssertTask(arrangedTasks[1], siblingSnapshot.Id, siblingSnapshot.Title, siblingSnapshot.OwnerId, siblingSnapshot.IsComplete);
        AssertTask(arrangedTasks[2], taskBSnapshot.Id, taskBSnapshot.Title, taskBSnapshot.OwnerId, taskBSnapshot.IsComplete);
        AssertTask(arrangedTasks[3], unownedSnapshot.Id, unownedSnapshot.Title, unownedSnapshot.OwnerId, unownedSnapshot.IsComplete);

        var ownerAResponse = await _client.GetAsync($"/owners/{ownerA.Id}/tasks");
        Assert.Equal(HttpStatusCode.OK, ownerAResponse.StatusCode);
        var ownerATasks = await ownerAResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(ownerATasks);
        Assert.Equal(new[] { targetId, siblingSnapshot.Id }, ownerATasks.Select(task => task.Id).ToArray());
        AssertTask(ownerATasks[0], targetId, targetTitleBeforeCompletion, ownerA.Id, true);
        AssertTask(ownerATasks[1], siblingSnapshot.Id, siblingSnapshot.Title, siblingSnapshot.OwnerId, siblingSnapshot.IsComplete);

        var ownerBResponse = await _client.GetAsync($"/owners/{ownerB.Id}/tasks");
        Assert.Equal(HttpStatusCode.OK, ownerBResponse.StatusCode);
        var ownerBTasks = await ownerBResponse.Content.ReadFromJsonAsync<List<TaskItem>>();
        Assert.NotNull(ownerBTasks);
        Assert.Single(ownerBTasks);
        AssertTask(ownerBTasks[0], taskBSnapshot.Id, taskBSnapshot.Title, taskBSnapshot.OwnerId, taskBSnapshot.IsComplete);
        Assert.DoesNotContain(ownerBTasks, task => task.Id == targetId);
    }

    [Fact]
    public async Task PatchTask_WithBodyFields_CompletesWithoutChangingTaskIdentityTitleOrOwner()
    {
        var owner = await CreateOwnerAsync("Body ignored owner");
        const string title = "  Finish report  ";
        var created = await CreateTaskAsync(title, owner.Id);
        Assert.False(created.IsComplete);
        var idBeforeCompletion = created.Id;
        var titleBeforeCompletion = created.Title;
        var ownerIdBeforeCompletion = created.OwnerId;
        var body = JsonContent.Create(new
        {
            Id = -123,
            Title = "Changed",
            OwnerId = (int?)null,
            IsComplete = false
        });

        var response = await _client.PatchAsync($"/tasks/{idBeforeCompletion}/complete", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var completed = await response.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(completed, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);

        var getResponse = await _client.GetAsync($"/tasks/{idBeforeCompletion}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var found = await getResponse.Content.ReadFromJsonAsync<TaskItem>();
        AssertTask(found, idBeforeCompletion, titleBeforeCompletion, ownerIdBeforeCompletion, true);
    }

    private async Task<TaskOwnerDto> CreateOwnerAsync(string name)
    {
        var response = await _client.PostAsJsonAsync("/owners", new CreateTaskOwnerRequest(name, $"{name.Replace(' ', '.')}@example.test"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var owner = await response.Content.ReadFromJsonAsync<TaskOwnerDto>();
        Assert.NotNull(owner);
        return owner;
    }

    private async Task<TaskItem> CreateTaskAsync(string title, int? ownerId = null)
    {
        var response = await _client.PostAsJsonAsync("/tasks", new CreateTaskRequest(title, ownerId));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var task = await response.Content.ReadFromJsonAsync<TaskItem>();
        Assert.NotNull(task);
        return task;
    }

    private static void AssertTask(TaskItem? actual, int id, string title, int? ownerId, bool isComplete)
    {
        Assert.NotNull(actual);
        Assert.Equal(id, actual.Id);
        Assert.Equal(title, actual.Title);
        Assert.Equal(ownerId, actual.OwnerId);
        Assert.Equal(isComplete, actual.IsComplete);
    }
}
