﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPro.Domain.Entity;

namespace ToDoListPro.Domain.ViewModels.Task;

public class CreateTaskViewModel
{
    public string Name { get; set; }
    public string Description { get; set; } 
    public Priority Priority { get; set; }
    public void Validate()
    {
        if(string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentNullException(Name, "Укажите название задачи");
        }
        if(string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentNullException(Name, "Укажите описание задачи");
        }
    }
}
