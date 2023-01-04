using System;
using System.ComponentModel.DataAnnotations;

namespace Todo.Data
{
    public class TodoItem
    {     
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Please provide the task name.")]
        public string TaskName { get; set; }
        public bool IsDone { get; set; }

        [Required(ErrorMessage = "Please provide a due date.")]
        public DateTime DueDate { get; set; } = DateTime.Today;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int QTYName { get; set; }
       /* public DateTime DateTakenOut { get; set; } = DateTime.Now;*/
        public string TakenBy { get; set; }
        public string ApprovedBy { get; set; }

        public string Item { get; set; }
        public int QuantityInInventory { get; set; }
        public DateTime TakenOutOn { get; set; } = DateTime.Now;
    }
}
