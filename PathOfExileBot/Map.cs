using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfExileBot
{
    class Map
    {
        public string Name;
        public string Level;
        public string Tier;
        public string BossDifficulty;
        public string LayoutSet;
        public string LayoutType;
        public string NumberOfBosses;
        public bool Unique;
        public string UniqueBoss;
        public List<string> uniqueBases = new List<string>();

        public override string ToString()
        {
            string ouput = Name + " | " + Level + " | " + LayoutType + " | ";
            foreach (var item in uniqueBases)
            {
                ouput += item + " | ";
            }

            return ouput;
        }

        public void AddBase(string name)
        {
            uniqueBases.Add(name.Replace('_', ' ').ToLower());
        }
    }
}
