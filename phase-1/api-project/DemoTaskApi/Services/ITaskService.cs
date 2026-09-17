using DemoTaskApi.Models;

namespace DemoTaskApi.Services;

public interface ITaskService
{
    List<TaskItem> GetAll();
    TaskItem? GetById(int id);
    TaskItem Create(string title, int? ownerId = null);
    List<TaskItem> GetByOwner(int ownerId);
}
