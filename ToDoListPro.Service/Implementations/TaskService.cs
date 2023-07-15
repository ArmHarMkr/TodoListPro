using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPro.DAL.Interfaces;
using ToDoListPro.DAL.Repositories;
using ToDoListPro.Domain.Entity;
using ToDoListPro.Domain.Extensions;
using ToDoListPro.Domain.Filters.Task;
using ToDoListPro.Domain.Response;
using ToDoListPro.Domain.ViewModels.Task;
using ToDoListPro.Service.Interfaces;

namespace ToDoListPro.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<TaskEntity> _taskRepository;
    private ILogger<TaskService> _logger;

    public TaskService(IBaseRepository<TaskEntity> taskRepository,
        ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.Created == DateTime.Today)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDone = x.IsDone == true ? "Done" : "Not done",
                    Description = x.Description.Substring(0, 5),
                    Priority = x.Priority.ToString(),
                    Created = x.Created.ToString(CultureInfo.InvariantCulture)
                })
                .ToListAsync();
            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.CalculateCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();
            _logger.LogInformation($"Запрос на создании задачи - {model.Name}");

            var task = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .FirstOrDefaultAsync(x => x.Name == model.Name);
            if (task != null)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача с таким названием уже есть",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }

            task = new TaskEntity()
            {
                Name = model.Name,
                Description = model.Description,
                IsDone = false,
                Priority = model.Priority,
                Created = DateTime.Now
            };
            await _taskRepository.Create(task);

            _logger.LogInformation($"Задача создалась: {task.Name} {task.Created}");
            return new BaseResponse<TaskEntity>()
            {
                Description = "Задача создалась",
                StatusCode = StatusCode.OK
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
            return new BaseResponse<TaskEntity>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id)
    {
        try
        {
            var task = await _taskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if(task == null)
            {
                return new BaseResponse<bool>
                {
                    StatusCode = StatusCode.TaskNotFound,
                    Description = "Задача не найдена",
                };
            }
            task.IsDone = true;
            await _taskRepository.Update(task);
            return new BaseResponse<bool>
            {
                StatusCode = StatusCode.OK,
                Description = "Задача завершена "
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.EndTask]: {ex.Message}");
            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetCompletedTasks()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.IsDone)
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDone = x.IsDone == true ? "Готово" : "Не готово",
                    Description = x.Description.Substring(0, 5),
                    Priority = x.Priority.GetDisplayName(),
                    Created = x.Created.ToLongDateString()
                }).ToListAsync();
            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };


        }
        catch (Exception ex)
        {

            _logger.LogError(ex, $"[TaskService.GetCompletedTasks]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>() { StatusCode = StatusCode.InternalServerError };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => !x.IsDone)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова",
                    Priority = x.Priority.GetDisplayName(),
                    Created = x.Created.ToLongDateString()
                })
                .ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}