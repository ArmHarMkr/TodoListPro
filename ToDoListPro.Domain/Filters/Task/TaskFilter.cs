using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoListPro.Domain.Entity;

namespace ToDoListPro.Domain.Filters.Task
{
    public class TaskFilter
    {
        public string Name { get; set; }
        public Priority? Priority { get; set; } 
    }
}
