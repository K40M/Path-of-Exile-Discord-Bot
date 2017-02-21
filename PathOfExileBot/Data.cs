using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathOfExileBot
{
    class Data
    {
        //List with available ascendancies
        public static List<Ascendancy> Ascendancies = new List<Ascendancy>()
        {
            new Ascendancy("Slayer", BaseClass.Duelist, new List<SkillType>() {SkillType.Mines, SkillType.Minions, SkillType.Totems, SkillType.Traps }),
            new Ascendancy("Gladiator", BaseClass.Duelist),
            new Ascendancy("Champion", BaseClass.Duelist),
            new Ascendancy("Assasin",BaseClass.Shadow, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Saboteur",BaseClass.Shadow, new List<SkillType>() {SkillType.Totems, SkillType.Minions }),
            new Ascendancy("Trickster", BaseClass.Shadow),
            new Ascendancy("Juggernaut", BaseClass.Marauder),
            new Ascendancy("Berserker", BaseClass.Marauder, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Chieftain", BaseClass.Marauder),
            new Ascendancy("Necromancer", BaseClass.Witch),
            new Ascendancy("Elementalist",BaseClass.Witch, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Occultist", BaseClass.Witch, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Deadeye", BaseClass.Ranger, new List<SkillType>() {SkillType.MeleeAttack, SkillType.MeleeSpell, SkillType.Mines, SkillType.Minions, SkillType.Minions, SkillType.Totems, SkillType.Traps }),
            new Ascendancy("Raider", BaseClass.Ranger, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Pathfinder", BaseClass.Ranger, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Inquisitor", BaseClass.Templar, new List<SkillType>() {SkillType.Minions }),
            new Ascendancy("Hierophant", BaseClass.Templar, new List<SkillType>() {SkillType.Minions, SkillType.Traps, SkillType.Mines }),
            new Ascendancy("Guardian", BaseClass.Templar),
            new Ascendancy("Ascendant", BaseClass.Scion)
        };

        //List of active skill gems
        public static List<ActiveSkill> skills = new List<ActiveSkill>()
        {
            new ActiveSkill("Ancestral Protector", SkillType.Totems, 4),
            new ActiveSkill("Ancestral Warchief", SkillType.Totems, 28),
            new ActiveSkill("Cleave", SkillType.MeleeAttack, 1),
            new ActiveSkill("Dominating Blow", SkillType.MeleeAttack, 28),
            new ActiveSkill("Earthquake", SkillType.MeleeAttack, 28),
            new ActiveSkill("Flame Totem", SkillType.Totems, 4),
            new ActiveSkill("Glacial Hammer", SkillType.MeleeAttack, 1),
            new ActiveSkill("Ground Slam", SkillType.MeleeAttack, 1),
            new ActiveSkill("Heavy Strike", SkillType.MeleeAttack, 1),
            new ActiveSkill("Ice Crash", SkillType.MeleeAttack, 28),
            new ActiveSkill("Infernal Blow", SkillType.MeleeAttack, 1),
            new ActiveSkill("Leap Slam", SkillType.MeleeAttack, 10),
            new ActiveSkill("Molten Strike", SkillType.MeleeAttack, 1),
            new ActiveSkill("Searing Bond", SkillType.Totems, 12),
            new ActiveSkill("Shield Charge", SkillType.MeleeAttack, 10),
            new ActiveSkill("Shockwave Totem", SkillType.Totems, 28),
            new ActiveSkill("Static Strike", SkillType.MeleeAttack, 12),
            new ActiveSkill("Sunder", SkillType.MeleeAttack, 12),
            new ActiveSkill("Sweep", SkillType.MeleeAttack, 12),
            new ActiveSkill("Animate Weapon", SkillType.RangedSpell, 4),
            new ActiveSkill("Barrage", SkillType.RangedAttack, 12),
            new ActiveSkill("Bear Trap", SkillType.Traps, 4),
            new ActiveSkill("Blade Flurry", SkillType.MeleeAttack, 28),
            new ActiveSkill("Blade Vortex", SkillType.MeleeSpell, 12),
            new ActiveSkill("Bladefall", SkillType.RangedSpell, 28),
            new ActiveSkill("Blast Rain", SkillType.RangedAttack, 28),
            new ActiveSkill("Burning Arrow", SkillType.RangedAttack, 1),
            new ActiveSkill("Cyclone", SkillType.MeleeAttack, 28),
            new ActiveSkill("Detonate Dead", SkillType.RangedSpell, 4),
            new ActiveSkill("Double Strike", SkillType.MeleeAttack, 1),
            new ActiveSkill("Dual Strike", SkillType.MeleeAttack, 1),
            new ActiveSkill("Ethereal Knives", SkillType.RangedSpell, 1),
            new ActiveSkill("Explosive Arrow", SkillType.RangedAttack, 28),
            new ActiveSkill("Fire Trap", SkillType.Traps, 1),
            new ActiveSkill("Flicker Strike", SkillType.MeleeAttack, 10),
            new ActiveSkill("Freeze Mine", SkillType.Traps, 10),
            new ActiveSkill("Frenzy", SkillType.MeleeAttack, 16),
            new ActiveSkill("Frost Blades", SkillType.MeleeAttack, 1),
            new ActiveSkill("Ice Shot", SkillType.RangedAttack, 1),
            new ActiveSkill("Ice Trap", SkillType.Traps, 28),
            new ActiveSkill("Lacerate", SkillType.MeleeAttack, 12),
            new ActiveSkill("Lightning Arrow", SkillType.RangedAttack, 12),
            new ActiveSkill("Lightning Strike", SkillType.MeleeAttack, 12),
            new ActiveSkill("Blink Arrow", SkillType.Minions, 10),
            new ActiveSkill("Mirror Arrow", SkillType.Minions, 10),
            new ActiveSkill("Puncture", SkillType.RangedAttack, 4),
            new ActiveSkill("Rain of Arrows", SkillType.RangedAttack, 12),
            new ActiveSkill("Reave", SkillType.MeleeAttack, 12),
            new ActiveSkill("Shrapnel Shot", SkillType.RangedAttack, 1),
            new ActiveSkill("Siege Ballista", SkillType.Totems, 4),
            new ActiveSkill("Spectral Throw", SkillType.RangedAttack, 1),
            new ActiveSkill("Split Arrow", SkillType.RangedAttack, 1),
            new ActiveSkill("Tornado Shot", SkillType.RangedAttack, 28),
            new ActiveSkill("Viper Strike", SkillType.MeleeAttack, 1),
            new ActiveSkill("Wild Strike", SkillType.MeleeAttack, 28),
            new ActiveSkill("Arc", SkillType.RangedSpell, 12),
            new ActiveSkill("Arctic Breath", SkillType.RangedSpell, 28),
            new ActiveSkill("Ball Lightning", SkillType.RangedSpell, 28),
            new ActiveSkill("Blight", SkillType.MeleeSpell, 1),
            new ActiveSkill("Discharge", SkillType.MeleeSpell, 28),
            new ActiveSkill("Essence Drain", SkillType.RangedSpell, 12),
            new ActiveSkill("Fire Nova Mine", SkillType.Traps, 12),
            new ActiveSkill("Fireball", SkillType.RangedSpell, 1),
            new ActiveSkill("Firestorm", SkillType.RangedSpell, 12),
            new ActiveSkill("Flame Surge", SkillType.RangedSpell, 12),
            new ActiveSkill("Flameblast", SkillType.RangedSpell, 28),
            new ActiveSkill("Freezing Pulse", SkillType.RangedSpell, 1),
            new ActiveSkill("Frost Bomb", SkillType.RangedSpell, 4),
            new ActiveSkill("Frostbolt", SkillType.RangedSpell, 1),
            new ActiveSkill("Glacial Cascade", SkillType.RangedSpell, 28),
            new ActiveSkill("Ice Nova", SkillType.RangedSpell, 12),
            new ActiveSkill("Ice Spear", SkillType.RangedSpell, 12),
            new ActiveSkill("Incinerate", SkillType.RangedSpell, 12),
            new ActiveSkill("Kinetic Blast", SkillType.RangedSpell, 28),
            new ActiveSkill("Lightning Tendrils", SkillType.RangedSpell, 1),
            new ActiveSkill("Lightning Trap", SkillType.Traps, 12),
            new ActiveSkill("Magma Orb", SkillType.RangedSpell, 1),
            new ActiveSkill("Orb of Storms", SkillType.RangedSpell, 4),
            new ActiveSkill("Power Siphon", SkillType.RangedAttack, 12),
            new ActiveSkill("Raise Spectre", SkillType.Minions, 28),
            new ActiveSkill("Raise Zombie", SkillType.Minions, 1),
            new ActiveSkill("Righteous Fire", SkillType.MeleeSpell, 16),
            new ActiveSkill("Scorching Ray", SkillType.RangedSpell, 12),
            new ActiveSkill("Shock Nova", SkillType.RangedSpell, 28),
            new ActiveSkill("Spark", SkillType.RangedSpell, 1),
            new ActiveSkill("Storm Call", SkillType.RangedSpell, 12),
            new ActiveSkill("Summon Raging Spirit", SkillType.Minions, 4),
            new ActiveSkill("Summon Skeletons", SkillType.Minions, 10),
            new ActiveSkill("Vortex", SkillType.RangedSpell, 28),
            new ActiveSkill("Cast On Critical Strike Support", SkillType.RangedSpell, 38)
        };

        //List of non valid Cast on Crit skills
        public static List<string> notValidSkillList = new List<string>()
        {
            "Blade Flurry",
            "Blight",
            "Flameblast",
            "Incinerate",
            "Scorching Ray",
            "Wither",
            "Lightning Tendrils",
            "Righteous Fire",
            "Flame Surge",
            "Cast On Critical Strike Support"
        };

        //List of vaal skills
        public static List<string> vaalSkills = new List<string>()
        {
            "Glacial Hammer",
            "Ground Slam",
            "Burning Arrow",
            "Cyclone",
            "Detonate Dead",
            "Double Strike",
            "Lightning Strike",
            "Rain of Arrows",
            "Reave",
            "Spectral Throw",
            "Arc",
            "Cold Snap",
            "Fireball",
            "Flameblast",
            "Ice Nova",
            "Lightning Trap",
            "Power Siphon",
            "Righteous Fire",
            "Spark",
            "Storm Call",
            "Summon Skeletons"
        };

        //List of Keystones
        public static List<string> keyStones = new List<string>()
        {
            "Acrobatics",
            "Ancestral Bond",
            "Arrow Dancing",
            "Avatar of Fire",
            "Blood Magic",
            "Chaos Inoculation",
            "Conduit",
            "Eldritch Battery",
            "Elemental Equilibrium",
            "Elemental Overload",
            "Ghost Reaver",
            "Iron Grip",
            "Iron Reflexes",
            "Mind Over Matter",
            "Minion Instability",
            "Necromantic Aegis",
            "Pain Attunement",
            "Phase Acrobatics",
            "Point Blank",
            "Resolute Technique",
            "Unwavering Stance",
            "Vaal Pact",
            "Zealot's Oath"
        };
    }
}
