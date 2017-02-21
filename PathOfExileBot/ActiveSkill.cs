using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum SkillType { RangedSpell, MeleeSpell, RangedAttack, MeleeAttack, Totems, Traps, Mines, Minions }

namespace PathOfExileBot
{
    class ActiveSkill
    {
        public string name;
        public SkillType type;
        public int level;
        public bool vaal;
        public List<string> tags;

        public ActiveSkill(string name, SkillType type, int level)
        {
            this.name = name;
            this.type = type;
            this.level = level;
            this.tags = new List<string>();

            if (Data.vaalSkills.Contains(name))
                vaal = true;
        }
    }
}
