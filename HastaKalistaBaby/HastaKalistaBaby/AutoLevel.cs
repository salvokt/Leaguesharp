using LeagueSharp;
using LeagueSharp.Common;
using System;

namespace HastaKalistaBaby
{
     internal class AutoLevel
    {
        private static Menu root = Program.root;
        static int  Q, W, E, R;

        public static void Init()
        {
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnLevelUp += Obj_AI_Base_OnLevelUp;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            Q = root.Item("1").GetValue<StringList>().SelectedIndex;
            W = root.Item("2").GetValue<StringList>().SelectedIndex;
            E = root.Item("3").GetValue<StringList>().SelectedIndex;
            R = root.Item("4").GetValue<StringList>().SelectedIndex;
            if(SameValues())
            {
                return;
            }
        }

        private static void Obj_AI_Base_OnLevelUp(Obj_AI_Base sender, EventArgs args)
        {
            if (!sender.IsMe || !root.Item("Lvlon").GetValue<bool>() || Program.Player.Level < root.Item("s").GetValue<Slider>().Value || SameValues())
            {
                return;
            }
            lvl(Q);
            lvl(W);
            lvl(E);
            lvl(R);
        }

        private static void OnDraw(EventArgs args)
        {
            if(SameValues())
            {
                Drawing.DrawText(Drawing.WorldToScreen(Program.Player.Position).X,Drawing.WorldToScreen(Program.Player.Position).Y - 10,System.Drawing.Color.OrangeRed,"Wrong Ability Sequence");
            }
        }

        private static void lvl(int i)//Inspired By seb oktw
        {
            if (Program.Player.Level < 4)
            {
                if (i == 0 && Program.Player.Spellbook.GetSpell(SpellSlot.Q).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1 && Program.Player.Spellbook.GetSpell(SpellSlot.W).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2 && Program.Player.Spellbook.GetSpell(SpellSlot.E).Level == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
            }
            else
            {
                if (i == 0)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (i == 1)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (i == 2)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (i == 3)
                    Program.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static bool SameValues()
        {
            if ((Q == W || Q == E || Q == R || W == E || W == R || E == R) && root.Item("Lvlon").GetValue<bool>())
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
