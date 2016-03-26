using LeagueSharp.Common;
using SharpDX;

namespace HastaKalistaBaby
{
    public static class MenuManager
    {
        private static Menu root = Program.root;
        private static Menu draw = Program.draw;
        public static void Create()
        {

            var q = new Menu("Q Settings", "spell.q");
            {
                q.AddItem(new MenuItem("Qsetting", "Q Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.Red);
                q.AddItem(new MenuItem("AutoQ", "Enable Q").SetValue(true));
                q.AddItem(new MenuItem("AutoQH", "Auto Q Harass").SetValue(true));
                q.AddItem(new MenuItem("AutoQM", "Auto Q Across Minions").SetValue(true));
                root.AddSubMenu(q);
            }

            var w = new Menu("W Settings", "spell.w");
            {
                w.AddItem(new MenuItem("Wsetting", "W Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.RosyBrown);
                w.AddItem(new MenuItem("AutoW", "Auto W").SetValue(true));
                w.AddItem(new MenuItem("WAll", "Cast W on Nearest Monster").SetValue(new KeyBind('A',KeyBindType.Press)));
                w.AddItem(new MenuItem("WBaron", "Cast W on Baron").SetValue(new KeyBind('T', KeyBindType.Press)));
                w.AddItem(new MenuItem("WDrake", "Cast W on Drake").SetValue(new KeyBind('Y', KeyBindType.Press)));
                root.AddSubMenu(w);
            }

            var e = new Menu("E Settings", "spell.e");
            {
                e.AddItem(new MenuItem("Esetting", "E Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.Blue);
                e.AddItem(new MenuItem("AutoEChamp", "Auto E On Champions").SetValue(true));
                e.AddItem(new MenuItem("AutoEDead", "Auto E Before Death").SetValue(true));
                e.AddItem(new MenuItem("AutoEDeadS", "Auto E Before Death if Health % <=").SetValue(new Slider(5, 1, 20)));
                e.AddItem(new MenuItem("jEsetting", "Jungle Settings"));
                e.AddItem(new MenuItem("BlueM", "Auto E Blue").SetValue(true));
                e.AddItem(new MenuItem("RedM", "Auto E Red").SetValue(true));
                e.AddItem(new MenuItem("BaronM", "Auto E Baron").SetValue(true));
                e.AddItem(new MenuItem("DrakeM", "Auto E Dragon").SetValue(true));
                e.AddItem(new MenuItem("SmallM", "Auto E Smalls").SetValue(false));
                e.AddItem(new MenuItem("OtherM", "Auto E Gromp/Wolf/Krug/Raptor").SetValue(true));
                e.AddItem(new MenuItem("MidM", "Auto E Crab").SetValue(true));
                e.AddItem(new MenuItem("lEsetting", "LaneClear Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.ForestGreen);
                e.AddItem(new MenuItem("AutoEMinions", "Auto E Minions").SetValue(true));
                e.AddItem(new MenuItem("minAutoEMinions", "Min minions").SetValue(new Slider(2,1,5)));
                e.AddItem(new MenuItem("BigMinionFinisher", "Auto E Big Minions").SetValue(true));
                e.AddItem(new MenuItem("AutoEMinionsTower", "Auto E Big Minions Under Tower").SetValue(true));
                root.AddSubMenu(e);
            }

            var r = new Menu("R Settings", "spell.r");
            {
                r.AddItem(new MenuItem("Rsetting", "R Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.BlueViolet);
                r.AddItem(new MenuItem("AutoR", "Auto R Saver").SetValue(true));
                r.AddItem(new MenuItem("KBS", "Auto R BlitzCrank/Skarner/Kench").SetValue(true));
                root.AddSubMenu(r);
            }

            {
                draw.AddItem(new MenuItem("Dsetting", "Drawing Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.GreenYellow);
                draw.AddItem(new MenuItem("Qrange", "Draw Q Range").SetValue(false));
                draw.AddItem(new MenuItem("Wrange", "Draw W Range").SetValue(false));
                draw.AddItem(new MenuItem("Erange", "Draw E Range").SetValue(true));
                draw.AddItem(new MenuItem("Rrange", "Draw R Range").SetValue(true));
                draw.AddItem(new MenuItem("healthp", "Show Health Percent").SetValue(true));
                draw.AddItem(new MenuItem("healthp1", "Show Damage HealthBar").SetValue(new Circle(true, System.Drawing.Color.Purple)));
                draw.AddItem(new MenuItem("TargetA", "Draw Target Attack Range").SetValue(true));
                root.AddSubMenu(draw);
            }

            var lvl = new Menu("Level Settigns", "lvl");
            {
                lvl.AddItem(new MenuItem("Dsetting", "Level Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.Snow);
                lvl.AddItem(new MenuItem("Lvlon", "Enable Level Up").SetValue(true));
                lvl.AddItem(new MenuItem("1", "1").SetValue(new StringList(new[] { "Q", "W", "E", "R"},3)));
                lvl.AddItem(new MenuItem("2", "2").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
                lvl.AddItem(new MenuItem("3", "3").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
                lvl.AddItem(new MenuItem("4", "4").SetValue(new StringList(new[] { "Q", "W", "E", "R" }, 1)));
                lvl.AddItem(new MenuItem("s", "Start at level").SetValue(new Slider(2, 1, 5)));
                root.AddSubMenu(lvl);
            }

            root.AddToMainMenu();
        }
    }
}
