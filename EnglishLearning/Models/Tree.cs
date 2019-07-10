using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace EnglishLearning.Models
{

    public abstract class TreeComponent
    {
        public virtual void Add(TreeComponent item)
        {

        }

        public virtual void AddRange(List<TreeComponent> item)
        {

        }

        public virtual void Remove(TreeComponent item)
        {

        }

        public virtual TreeComponent GetChild(int id)
        {
            throw new NotImplementedException();
        }

        public abstract string Print(User user, UrlHelper url);
    }

    public class TreeItem : TreeComponent
    {
        public int Id { get; set; }
        public string Difficult { get; set; }
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

        public TreeItem() { }

        public override string Print(User user, UrlHelper url)
        {
            string userComplexity = GroupName == "Тести до озвучених історій" ? user.LvlListening : (GroupName == "Тести до текстів" ? user.LvlReading : user.Level);
            Enum.TryParse(this.Difficult.Replace('-', '_'), out ExtendClasses.Difficult difficult);
            Enum.TryParse(userComplexity.Replace('-', '_'), out ExtendClasses.Difficult userLvl);
            StringBuilder sb = new StringBuilder(difficult == userLvl ? "success" : (difficult < userLvl ? "purple" : "info"), 400);
            sb.Insert(0, "<li class='list-group-item list-group-item-action list-group-item-");
            sb.AppendFormat("'><a class='text-dark my-1 treeNode' data-toggle='tooltip' data-placement='bottom' title='{0}' href='{1}'>{2}<a></li>", Name, url.Action(Controller, Action, new { area = "", id = this.Id }), Name);
            return sb.ToString();
        }

    }

    public class SubTreeComponent : TreeComponent
    {
        List<TreeComponent> tree = new List<TreeComponent>();
        public string Name { get; set; }
        public int Id { get; set; }

        public SubTreeComponent() { }

        public SubTreeComponent(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override void Add(TreeComponent component)
        {
            tree.Add(component);
        }

        public override void AddRange(List<TreeComponent> component)
        {
            tree.AddRange(component);
        }

        public override void Remove(TreeComponent component)
        {
            tree.Remove(component);
        }

        public override TreeComponent GetChild(int index)
        {
            return tree.ElementAt(index);
        }

        public override string Print(User user, UrlHelper url)
        {
            StringBuilder html = new StringBuilder(5000);
            if (tree.Count > 0)
            {
                html.AppendFormat("<a class='btn btn-purple m-1 treeNode' data-toggle='collapse' href='#Collapse_{0}' role='button' aria-expanded='false' aria-controls='#Collapse_{1}'>{2}</a><div class='collapse multi-collapse' id='Collapse_{3}'><ol type='I'>", Id, Id, Name, Id);
                foreach (var el in tree)
                {
                    html.Append(el.Print(user, url));
                }
                html.Append("</ol></div>");
            }
            else
            {
                html.AppendFormat("<a class='btn btn-secondary disabled m-1 treeNode' disabled data-toggle='collapse' href='#Collapse_{0}' role='button' aria-expanded='false' aria-controls='#Collapse_'{1}'>{2}</a>", Id, Id, Name);
            }
            return html.ToString();

        }

    }
}