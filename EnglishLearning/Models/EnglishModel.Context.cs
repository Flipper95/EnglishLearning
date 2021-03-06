﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class EnglishLearningEntities : DbContext
    {
        public EnglishLearningEntities()
            : base("name=EnglishLearningEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Answer> Answer { get; set; }
        public virtual DbSet<Group> Group { get; set; }
        public virtual DbSet<LearningWord> LearningWord { get; set; }
        public virtual DbSet<Lection> Lection { get; set; }
        public virtual DbSet<Question> Question { get; set; }
        public virtual DbSet<Test> Test { get; set; }
        public virtual DbSet<TestHistory> TestHistory { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Word> Word { get; set; }
        public virtual DbSet<LectionGroup> LectionGroup { get; set; }
        public virtual DbSet<TestGroup> TestGroup { get; set; }
        public virtual DbSet<Grammar> Grammar { get; set; }
        public virtual DbSet<GrammarGroup> GrammarGroup { get; set; }
        public virtual DbSet<ELTask> ELTask { get; set; }
        public virtual DbSet<UserELTask> UserELTask { get; set; }
        public virtual DbSet<TextTask> TextTask { get; set; }
        public virtual DbSet<Video> Video { get; set; }
    }
}
