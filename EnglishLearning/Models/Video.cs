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
    
    public partial class Video
    {
        public int Id { get; set; }
        public string VideoHtml { get; set; }
        public string Genre { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string AuthorLink { get; set; }
        public int UserId { get; set; }
        public string Difficult { get; set; }
        public Nullable<int> Order { get; set; }
    
        public virtual User User { get; set; }
    }
}