using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HastaKalistaBaby
{

    internal partial class EarlyList
    {
        public string ChampionName { get; set; }
        public string SpellName { get; set; }
        public int Width { get; set; }
        public float Range { get; set; }
        public System.Drawing.Color Color { get; set; }
        public SharpDX.Color c { get; set; }
    }
    internal class EarlyEvade
    {
        public readonly List<EarlyList> EarlyList = new List<EarlyList>();
        private static Menu root = Program.root;
        private static Menu draw = Program.draw;
        public EarlyEvade()
        {
            this.Load();

            Menu EE = new Menu("Early Evade", "EarlyEvade").SetFontStyle(System.Drawing.FontStyle.Bold, Color.Red);
            EE.AddItem(new MenuItem("EESettings", "Early Evade Settings")).SetFontStyle(System.Drawing.FontStyle.Bold, Color.Yellow);
            EE.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));
            EE.AddItem(new MenuItem("drawline", "Draw Line").SetValue(true));
            EE.AddItem(new MenuItem("drawtext", "Draw Text").SetValue(true));

            foreach (var e in HeroManager.Enemies)
            {
                foreach (var eList in this.EarlyList)
                {
                    if (eList.ChampionName == e.ChampionName)
                    {
                        EE.AddItem(new MenuItem(eList.ChampionName + eList.SpellName, eList.ChampionName + eList.SpellName).SetValue(true)).SetFontStyle(System.Drawing.FontStyle.Regular,eList.c);
                    }

                    if (e.ChampionName == "Vayne")
                    {
                        EE.AddItem(new MenuItem("VayneE", "Vayne E").SetValue(true)).SetFontStyle(System.Drawing.FontStyle.Regular,Color.Silver);
                    }
                }
            }

            draw.AddSubMenu(EE);

            Drawing.OnDraw += Drawing_OnDraw;

        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!root.Item("Enabled").GetValue<bool>())
            {
                return;
            }

            if (root.Item("VayneE") != null)
            {
                foreach (var e in HeroManager.Enemies.Where(e => e.ChampionName.ToLower() == "vayne" && e.Distance(Program.Player.Position) < 900))
                {
                    for (var i = 1; i < 8; i++)
                    {
                        var championBehind = ObjectManager.Player.Position
                                             + Vector3.Normalize(e.ServerPosition - ObjectManager.Player.Position)
                                             * (-i * 50);
                        if (draw.Item("drawline").GetValue<bool>())
                        {
                            Drawing.DrawCircle(championBehind, 35f, championBehind.IsWall() ? System.Drawing.Color.Red : System.Drawing.Color.Gray);
                        }
                    }
                }
            }

            foreach (var e in HeroManager.Enemies.Where(e => e.IsValidTarget(2000)))
            {
                foreach (var eList in this.EarlyList)
                {
                    if (eList.ChampionName == e.ChampionName)
                    {
                        if (draw.Item(eList.ChampionName + eList.SpellName).GetValue<bool>())
                        {
                            var xminions = 0;
                            if (e.IsValidTarget(eList.Range))
                            {
                                for (var i = 1;
                                     i < e.Position.Distance(ObjectManager.Player.Position) / eList.Width;
                                     i++)
                                {
                                    var championBehind = ObjectManager.Player.Position
                                                         + Vector3.Normalize(
                                                             e.ServerPosition - ObjectManager.Player.Position)
                                                         * (i * eList.Width);

                                    var list = eList;
                                    var allies = HeroManager.Allies.Where(a => a.Distance(ObjectManager.Player.Position) < list.Range);
                                    var minions = MinionManager.GetMinions(ObjectManager.Player.Position, eList.Range,MinionTypes.All, MinionTeam.Ally);
                                    var mobs = MinionManager.GetMinions(ObjectManager.Player.Position,eList.Range,MinionTypes.All,MinionTeam.Neutral);

                                    xminions += minions.Count(m => m.Distance(championBehind) < eList.Width)
                                                + allies.Count(a => a.Distance(championBehind) < eList.Width)
                                                + mobs.Count(m => m.Distance(championBehind) < eList.Width);
                                }

                                if (xminions == 0)
                                {
                                    if (draw.Item("drawline").GetValue<bool>())
                                    {
                                        var rec = new Geometry.Polygon.Rectangle(ObjectManager.Player.Position, e.Position, eList.Width - 10);
                                        rec.Draw(eList.Color, 2);
                                    }

                                    if (draw.Item("drawtext").GetValue<bool>())
                                    {
                                        Vector3[] x = new[] { ObjectManager.Player.Position, e.Position };
                                        var aX =
                                            Drawing.WorldToScreen(
                                                new Vector3(
                                                    Helper.CenterOfVectors(x).X,
                                                    Helper.CenterOfVectors(x).Y,
                                                    Helper.CenterOfVectors(x).Z));

                                        Drawing.DrawText(aX.X - 15, aX.Y - 15, eList.Color, eList.ChampionName + eList.SpellName);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Load()
        {
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "LeBlanc",
                    SpellName = "E",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkViolet,
                    c = Color.DarkViolet
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Morgana",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.Violet,
                    c = Color.Violet
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Blitzcrank",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightGoldenrodYellow,
                    c = Color.LightGoldenrodYellow
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Amumu",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LimeGreen,
                    c = Color.LimeGreen
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Braum",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightCyan,
                    c = Color.LightCyan
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Ezreal",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.LightYellow,
                    c = Color.LightYellow
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Brand",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkGoldenrod,
                    c = Color.DarkGoldenrod
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Corki",
                    SpellName = "R",
                    Width = 75,
                    Range = 1500,
                    Color = System.Drawing.Color.WhiteSmoke,
                    c = Color.WhiteSmoke
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Karma",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.BlanchedAlmond,
                    c = Color.BlanchedAlmond
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "LeeSin",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.LightSkyBlue,
                    c = Color.LightSkyBlue
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Lux",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.LightSlateGray,
                    c = Color.LightSlateGray
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Nautilius",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1000,
                    Color = System.Drawing.Color.Black,
                    c = Color.Black
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Nidalee",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1500,
                    Color = System.Drawing.Color.Beige,
                    c = Color.Beige
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Quinn",
                    SpellName = "Q",
                    Width = 75,
                    Range = 850,
                    Color = System.Drawing.Color.DarkBlue,
                    c = Color.DarkBlue
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "TahmKench",
                    SpellName = "Q",
                    Width = 75,
                    Range = 850,
                    Color = System.Drawing.Color.HotPink,
                    c = Color.HotPink
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Thresh",
                    SpellName = "Q",
                    Width = 75,
                    Range = 1200,
                    Color = System.Drawing.Color.DarkSeaGreen,
                    c = Color.DarkSeaGreen
                });
            this.EarlyList.Add(
                new EarlyList
                {
                    ChampionName = "Zyra",
                    SpellName = "E",
                    Width = 75,
                    Range = 900,
                    Color = System.Drawing.Color.AliceBlue,
                    c = Color.AliceBlue
                });
        }

    }
}
