//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnglishLearning.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserELTask
    {
        public int UserTaskId { get; set; }
        public int TaskId { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public bool Done { get; set; }
        public int UserId { get; set; }
        public string ResultDocPath { get; set; }
    
        public virtual ELTask ELTask { get; set; }
        public virtual User User { get; set; }
    }
}
