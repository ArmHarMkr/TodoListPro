using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPro.Domain.Entity;
using ToDoListPro.Domain.Filters.Task;
using ToDoListPro.Domain.Response;
using ToDoListPro.Domain.ViewModels.Task;

namespace ToDoListPro.Service.Interfaces
{
    public interface ITaskService
    {
        Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks();
        Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetCompletedTasks();
        Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model);
        Task<IBaseResponse<bool>> EndTask(long id); 
        Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter);
    }
}
