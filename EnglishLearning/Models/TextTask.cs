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
    
    public partial class TextTask
    {
        public int TextId { get; set; }
        public int AuthorId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Words { get; set; }
        public string Difficult { get; set; }
    
        public virtual User User { get; set; }
    }
}