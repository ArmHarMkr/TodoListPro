using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ToDoListPro.Domain.Filters.Task;
using ToDoListPro.Domain.Utils;
using ToDoListPro.Domain.ViewModels.Task;
using ToDoListPro.Service.Interfaces;

namespace ToDoListPro.Controllers;

public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }


    public IActionResult Index()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var response = await _taskService.Create(model);
        if (response.StatusCode == Domain.Response.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }




    [HttpPost]
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var response = await _taskService.GetTasks(filter);
        return Json(new {data = response.Data});
    }



    [HttpPost]
    public async Task<IActionResult> EndTask(long id)
    {
        var response = await _taskService.EndTask(id);
        if(response.StatusCode == Domain.Response.StatusCode.OK)
        {
            return Ok(new {description = response.Description});
        }
        return BadRequest(new {description = response.Description});
    }


    [HttpPost]
    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await _taskService.CalculateCompletedTasks();
        if (response.StatusCode == Domain.Response.StatusCode.OK)
        {
            if (response.Data != null)
            {
                var csvService = new CsvBaseService<IEnumerable<TaskViewModel>>();
                var uploadFile = csvService.UploadFile(response.Data);
                return File(uploadFile, "text/csv", $"Statistics for {DateTime.Now.ToLongDateString()}.csv");
            }
            else
            {
                return BadRequest(new { description = "No data available for the completed tasks." });
            }
        }
        return BadRequest(new { description = response.Description });
    }


    public async Task<IActionResult> GetCompletedTasks()
    {
        var result = await _taskService.GetCompletedTasks();
        return Json(new
        {
            data = result.Data
        });
    }

}
