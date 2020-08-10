using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Entity
{
    //public static class MenuId
    //{

    //}
    public class Menus
    {
        public Menus(string name,string url)
        {
            Name = name;
            Url = url;
            Hide = false;
        }
        //private string id;
        //public string Id { get => id; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Hide { get; set; }

        private List<string> authorizes;
        public List<string> Authorizes { get => authorizes; }

        private List<Menus> children;
        public List<Menus> Children { get=>children;}

        public Menus SetAuthorize(Type type)
        {
            if (authorizes==null)
            {
                authorizes = new List<string>();
            }
            authorizes.Add(type.ToString());
            return this;
        }

        public Menus SetAuthorize(Type[] types)
        {
            if (authorizes == null)
            {
                authorizes = new List<string>();
            }
            authorizes.AddRange(Array.ConvertAll<Type,string>(types,s=>s.ToString()));
            return this;
        }

        public Menus SetChildren(Menus menus)
        {
            if (children == null)
            {
                children = new List<Menus>();
            }
            children.Add(menus);
            return this;
        }

        public Menus SetChildren(Menus[] menus)
        {
            if (children == null)
            {
                children = new List<Menus>();
            }
            children.AddRange(menus);
            return this;
        }

    }

}