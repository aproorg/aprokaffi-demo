using DemoTaskApi.Models;

namespace DemoTaskApi.Services;

public interface ITaskOwnerService
{
    List<TaskOwner> GetAll();
    TaskOwner? GetById(int id);
    TaskOwner Create(string name, string email);
}
