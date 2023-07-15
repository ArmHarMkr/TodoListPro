using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoListPro.Domain.Entity;

public class TaskEntity
{
    public long Id { get; set; }

    public string Name { get; set; }

    public bool IsDone { get; set; }

    public string Description { get; set; }

    public DateTime Created { get; set; }

    public Priority Priority { get; set; }
}

public enum Priority
{
    [Display(Name = "Простая")]
    Easy = 1,
    [Display(Name = "Важная")]
    Medium = 2,
    [Display(Name = "Критичная")]
    Hard = 3
}