using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BaseClass { Witch, Shadow, Ranger, Duelist, Marauder, Templar, Scion }

namespace PathOfExileBot
{
    class Ascendancy
    {
        public string name;
        public BaseClass baseClass;
        public List<SkillType> skilltype;

        public Ascendancy(string name, BaseClass baseClass, List<SkillType> skilltype = null)
        {
            this.name = name;
            this.baseClass = baseClass;
            this.skilltype = new List<SkillType> {
                    SkillType.MeleeAttack,
                    SkillType.MeleeSpell,
                    SkillType.RangedAttack,
                    SkillType.RangedSpell,
                    SkillType.Mines,
                    SkillType.Minions,
                    SkillType.Totems,
                    SkillType.Traps
                };

            //skilltype argument isn't empty we delete the given skilltypes from the list.
            if (skilltype != null)
            {
                foreach (SkillType type in skilltype)
                {
                    this.skilltype.Remove(type);
                }
            }
        }
    }
}
