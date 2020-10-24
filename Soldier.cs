using System.Collections.Generic;

namespace SpaceMercs
{
    public class Soldier
    {
        public int Defense {get; set;}
        public int Attack {get; set;}
        //public int Health {get; set;}
        public int CurrentActions {get; set;}
        public int MaxActions{get; set;}
        public int Initiative {get; set;}
        public int Skill { get; set; }
        public List<string> Wounds {get; set;}
    }
}