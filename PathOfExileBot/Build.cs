using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BuildType { Life, LowLife, CI, NoIdea }

namespace PathOfExileBot
{
    class Build
    {
        public string buildName;
        public string description;
        public string treeURL;
        public BuildType type;

        public Build(string buildName, string treeURL, BuildType type, string description)
        {
            this.buildName = buildName;
            this.description = description;
            this.treeURL = treeURL;
            this.type = type;
        }
    }
}
