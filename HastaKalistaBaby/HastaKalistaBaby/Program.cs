using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using SharpDX.Direct3D9;

using Color = System.Drawing.Color;

namespace HastaKalistaBaby
{
    internal class Program
    {
        public static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu root, draw;
        public static Spell Q, W, E, R;
        static Items.Item botrk = new Items.Item(3153, 550);
        static Items.Item yom = new Items.Item(3142, 750);
        static Items.Item bilgwat = new Items.Item(3144, 550);
        static Items.Item healthpotion = new Items.Item(2003, 0);
        static Items.Item manapotion = new Items.Item(2004, 0);
        static Items.Item flask = new Items.Item(2041, 0);
        public static EarlyEvade ee;
        public static Orbwalking.Orbwalker Orbwalker;
        public static int wcount = 0;

        public static Font Text;
        public static float grabT = Game.Time, lastecast = 0f;
        public static double i;
        public static float time, wtime;
        public static Obj_AI_Hero soulmate = null;


        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnGameLoad;
        }

        private static void OnGameLoad(EventArgs e)
        {
            if (Player.ChampionName != "Kalista")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 1130);
            W = new Spell(SpellSlot.W, 5200);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1400f);

            Q.SetSkillshot(0.25f, 30f, 1700f, true, SkillshotType.SkillshotLine);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Arial", Height = 35, Width = 12, Weight = FontWeight.Bold, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });
            root = new Menu("HastaKalistaBaby", "hkalista", true);
            draw = new Menu("Drawings Settings", "drawing");
            Orbwalker = new Orbwalking.Orbwalker(root.SubMenu("Orbwalker Settings"));

            MenuManager.Create();
            DamageIndicator.Init(Damage.GetEdamage);
            ee = new EarlyEvade();
            AutoLevel.Init();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Helper.OnProcessSpellCast;
            Spellbook.OnCastSpell += Helper.OnCastSpell;

        }

        public static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Items();
                    Qlogic();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    if (root.Item("AutoQH").GetValue<bool>())
                    {
                        Qlogic();
                    }

                    break;
            }
            WLogic();
            RLogic();
            ELogic();
            LaneClear();
            JungleClear();
            Pots();
        }



        private static void Qlogic()
        {
            if (!Q.IsReady() || !root.Item("AutoQ").GetValue<bool>() || Helper.GetMana(Q) < 80)
            {
                return;
            }

            var target = TargetSelector.GetTarget(E.Range * 1.2f, TargetSelector.DamageType.Physical);
            if (target.IsValidTarget())
            {
                var predout = Q.GetPrediction(target);
                var coll = predout.CollisionObjects;

                if (coll.Count < 1)
                {
                    Q.CastIfHitchanceEquals(target, HitChance.High);
                }
                if (coll.Count == 1 && root.Item("AutoQM").GetValue<bool>())
                {
                    foreach (var c in coll)
                    {
                        if (Damage.GetEdamage(c) > c.Health)
                        {
                            Q.Cast(predout.CastPosition);
                        }
                    }
                }


            }
        }

        private static void WLogic()
        {
            if (!W.IsReady() && Helper.GetMana(W) < 80 && Game.Time - wtime < 2)
            {
                return;
            }

            if (Helper.CountEnemy(Player.Position, 1300) > 0)
            {
                wtime = Game.Time;
            }

            if(root.Item("WBaron").GetValue<KeyBind>().Active)
            {
                Vector3 baronPos;
                baronPos.X = 5232;
                baronPos.Y = 10788;
                baronPos.Z = 0;
                if (Player.Distance(baronPos) < 5000)
                    W.Cast(baronPos);
            }

            if(root.Item("WDrake").GetValue<KeyBind>().Active)
            {
                Vector3 dragonPos;
                dragonPos.X = 9919f;
                dragonPos.Y = 4475f;
                dragonPos.Z = 0f;
                if (Player.Distance(dragonPos) < 5000)
                    W.Cast(dragonPos);
            }

            if ((root.Item("AutoW").GetValue<bool>() || root.Item("WAll").GetValue<KeyBind>().Active) && Helper.CountEnemy(Player.Position, 1300) == 0)
            {
                if (wcount > 0)
                {
                    Vector3 baronPos;
                    baronPos.X = 5232;
                    baronPos.Y = 10788;
                    baronPos.Z = 0;
                    if (Player.Distance(baronPos) < 5000)
                        W.Cast(baronPos);
                }
                if (wcount == 0)
                {
                    Vector3 dragonPos;
                    dragonPos.X = 9919f;
                    dragonPos.Y = 4475f;
                    dragonPos.Z = 0f;
                    if (Player.Distance(dragonPos) < 5000)
                        W.Cast(dragonPos);
                    else
                        wcount++;
                    return;
                }

                if (wcount == 1)
                {
                    Vector3 redPos;
                    redPos.X = 8022;
                    redPos.Y = 4156;
                    redPos.Z = 0;
                    if (Player.Distance(redPos) < 5000)
                        W.Cast(redPos);
                    else
                        wcount++;
                    return;
                }
                if (wcount == 2)
                {
                    Vector3 bluePos;
                    bluePos.X = 11396;
                    bluePos.Y = 7076;
                    bluePos.Z = 0;
                    if (Player.Distance(bluePos) < 5000)
                        W.Cast(bluePos);
                    else
                        wcount++;
                    return;
                }
                if (wcount > 2)
                {
                    wcount = 0;
                }
            }
        }

        private static void ELogic()
        {
            if (!E.IsReady())
            {
                return;
            }

            if (root.Item("AutoEDead").GetValue<bool>() && Player.HealthPercent < root.Item("AutoEDeadS").GetValue<Slider>().Value && HeroManager.Enemies.Any(o => o.IsValidTarget() && Helper.hasE(o) && E.IsInRange(o)))
            {
                E.Cast();
            }

            foreach (var enemy in ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsEnemy && Helper.hasE(x) && !Helper.Unkillable(x) && x.Distance(Player) < 900 && !x.IsDead))
            {
                if (root.Item("AutoEChamp").GetValue<bool>())
                {
                    if (Damage.GetEdamage(enemy) > Helper.GetHealth(enemy))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void RLogic()
        {
            if (Player.IsRecalling() || Player.InFountain() || !R.IsReady() || Helper.GetMana(R) < 80 || !root.Item("AutoR").GetValue<bool>())
            {
                return;
            }

            if (soulmate == null)
            {
                foreach (var ally in HeroManager.Allies.Where(x => !x.IsDead && !x.IsMe && x.HasBuff("kalistacoopstrikeally")))
                {
                    soulmate = ally;
                    break;
                }
            }
            else if (soulmate.IsVisible && soulmate.Distance(Player) < R.Range)
            {
                if (soulmate.Health < Helper.CountEnemy(soulmate.Position, 600) * soulmate.Level * 30)
                {
                    R.Cast();
                }
                if (soulmate.ChampionName == "Blitzcrank" && Player.Distance(soulmate.Position) > 300)
                {
                    if (Game.Time - grabT < 0.7)
                    {
                        return;
                    }
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("rocketgrab2")))
                    {
                        R.Cast();
                    }
                }
                if (soulmate.ChampionName == "TahmKench" && Player.Distance(soulmate.Position) > 300)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("tahmkenchwdevoured")))
                    {
                        R.Cast();
                    }
                }
                if (soulmate.ChampionName == "Skarner" && Player.Distance(soulmate.Position) > 300)
                {
                    foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && !x.IsZombie && x.HasBuff("skarnerimpale")))
                    {
                        R.Cast();
                    }
                }
            }
        }

        private static void LaneClear()
        {
            if (!E.IsReady() || Helper.GetMana(E) < 80)
            {
                return;
            }

            var minions = MinionManager.GetMinions(Orbwalking.GetRealAutoAttackRange(Player), MinionTypes.All, MinionTeam.Enemy).ToList();

            if (minions.Count == 0)
            {
                return;
            }
            if (root.Item("AutoEMinions").GetValue<bool>() || root.Item("BigMinionFinisher").GetValue<bool>() || root.Item("AutoEMinionsTower").GetValue<bool>())
            {
                int killable = 0;
                foreach (var m in minions)
                {
                    if (Damage.GetEdamage(m) > m.Health && HealthPrediction.GetHealthPrediction(m, 500, 250) > Player.GetAutoAttackDamage(m) && m.GetBuff("kalistaexpungemarker").EndTime > 0.5)
                    {
                        killable++;
                        if (killable >= root.Item("minAutoEMinions").GetValue<Slider>().Value && root.Item("AutoEMinions").GetValue<bool>() || (m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && root.Item("BigMinionFinisher").GetValue<bool>())
                        {
                            CastE();
                            break;
                        }
                        if ((m.CharData.BaseSkinName.ToLower().Contains("siege") || m.CharData.BaseSkinName.ToLower().Contains("super")) && root.Item("AutoEMinionsTower").GetValue<bool>() && killable > 0 && m.UnderTurret())
                        {
                            CastE();
                            break;
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {

            foreach (var jungle in MinionManager.GetMinions(E.Range, MinionTypes.All, MinionTeam.Neutral))
            {
                if (Damage.GetEdamage(jungle) > jungle.Health)
                {
                    if (jungle.Name.Contains("Red") && root.Item("RedM").GetValue<bool>() && !jungle.Name.Contains("RedMini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Blue") && root.Item("BlueM").GetValue<bool>() && !jungle.Name.Contains("BlueMini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Baron") && root.Item("BaronM").GetValue<bool>())
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Dragon") && root.Item("DrakeM").GetValue<bool>())
                    {
                        CastE();
                    }
                    if ((jungle.Name.Contains("Krug") || jungle.Name.Contains("Razor") || jungle.Name.Contains("wolf") || jungle.Name.Contains("Gromp")) && root.Item("OtherM").GetValue<bool>() && !jungle.Name.Contains("Mini"))
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Crab") && root.Item("MidM").GetValue<bool>())
                    {
                        CastE();
                    }
                    if (jungle.Name.Contains("Mini") && root.Item("SmallM").GetValue<bool>() && !jungle.Name.Contains("Crab"))
                    {
                        CastE();
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (root.Item("Qrange").GetValue<bool>())
            {
                if (Q.IsReady())
                {
                    if (root.Item("fps").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Violet);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, Q.Range, Color.Violet);
                    }
                }
            }

            if (root.Item("Wrange").GetValue<bool>())
            {
                if (W.IsReady())
                {
                    if (root.Item("fps").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(Player.Position, W.Range, Color.Cyan);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, W.Range, Color.Cyan);
                    }
                }
            }

            if (root.Item("Erange").GetValue<bool>())
            {
                if (E.IsReady())
                {
                    if (root.Item("fps").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(Player.Position, E.Range, Color.Orange);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, E.Range, Color.Orange);
                    }
                }
            }

            if (root.Item("Rrange").GetValue<bool>())
            {
                if (R.IsReady())
                {
                    if (root.Item("fps").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gray);
                    }
                    else
                    {
                        Drawing.DrawCircle(Player.Position, R.Range, Color.Gray);
                    }
                }
            }

            if (root.Item("Minionh").GetValue<bool>())
            {
                foreach (var e in ObjectManager.Get<Obj_AI_Base>().Where(x => x.IsMinion && Helper.hasE(x) && x.IsValidTarget(E.Range + 250)))
                {
                    if (Damage.GetEdamage(e) > e.Health)
                    {
                        Vector3 p = new Vector3((int)e.Position.X, (int)e.Position.Y, (int)e.Position.Z);
                        var points = Helper.CalculateVertices(4, e.ScaleSkinCoef * 30, 70, p);

                        PolygonDraw(p, points, 2, Color.LightGreen);
                    }
                }
            }

            if (root.Item("healthp").GetValue<bool>())
            {
                foreach (var enemy in ObjectManager.Get<Obj_AI_Base>().Where(x => (x.IsValidTarget(E.Range) && Helper.hasE(x) && !x.IsMinion && !x.Name.Contains("Mini")) && (x.IsEnemy || x.Name.Contains("Krug") || x.Name.Contains("Razor") || x.Name.Contains("wolf") || x.Name.Contains("Gromp") || x.Name.Contains("Crab") || x.Name.Contains("Blue") || x.Name.Contains("Red"))))
                {
                    float hp = Helper.GetHealth(enemy) - Damage.GetEdamage(enemy);
                    var dmg = ((int)((Damage.GetEdamage(enemy) / Helper.GetHealth(enemy)) * 100));

                    if (dmg <= 9)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 125, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, SharpDX.Color.WhiteSmoke);
                    }
                    if (dmg >= 10)
                    {
                        Text.DrawText(null, dmg.ToString(), (int)enemy.HPBarPosition.X + 108, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, "%", (int)enemy.HPBarPosition.X + 138, (int)enemy.HPBarPosition.Y + 41, SharpDX.Color.Black);
                        Text.DrawText(null, dmg.ToString() + "%", (int)enemy.HPBarPosition.X + 110, (int)enemy.HPBarPosition.Y + 40, SharpDX.Color.WhiteSmoke);
                    }
                }
            }

            if (root.Item("Target").GetValue<bool>())
            {
                if (!Player.IsDead)
                {
                    var t = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);

                    if (root.Item("fps").GetValue<bool>())
                    {
                        Render.Circle.DrawCircle(t.Position, t.ScaleSkinCoef * 65, Color.Gold);
                    }
                    else
                    {
                        if (Game.Time - time < 1)
                        {
                            i = i + 0.5;
                        }
                        if (i > 360)
                        {
                            i = 0;
                        }
                        Vector3 p = new Vector3((int)t.Position.X, (int)t.Position.Y, (int)t.Position.Z);
                        var points = Helper.CalculateVertices(3, t.ScaleSkinCoef * 65, (int)i, p);
                        var points1 = Helper.CalculateVertices(11, t.ScaleSkinCoef * 73, (int)-i, p);
                        PolygonDraw(p, points1, (float)3, Color.Gold);
                        PolygonDraw(p, points, (float)2.5, Color.LightYellow);
                        time = Game.Time;
                    }

                    if (root.Item("TargetA").GetValue<bool>())
                    {
                        if (root.Item("fps").GetValue<bool>())
                        {
                            if (Player.Distance(t) > Helper.GetAttackRange(t))
                            {
                                Render.Circle.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.ForestGreen);
                            }
                            else
                            {
                                Render.Circle.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.OrangeRed);
                            }
                        }
                        else
                        {
                            if (Player.Distance(t) > Helper.GetAttackRange(t))
                            {
                                Drawing.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.ForestGreen);
                            }
                            else
                            {
                                Drawing.DrawCircle(t.Position, Helper.GetAttackRange(t), Color.OrangeRed);
                            }
                        }
                    }
                }
            }
        }

        private static void CastE()
        {
            if (Game.Time - lastecast < 0.700)
            {
                return;
            }

            E.Cast();
        }

        private static void PolygonDraw(Vector3 Center, Vector3[] Points, float thickness, Color color)
        {
            for (int i = 0; i < Points.Count() - 1; i++)
            {
                var q = Drawing.WorldToScreen(new Vector3(new Vector2(Points[i].X, Points[i].Y),
                    Points[i].Z));

                var z = Drawing.WorldToScreen(new Vector3(new Vector2(Points[i + 1].X, Points[i + 1].Y),
                    Points[i + 1].Z));

                Drawing.DrawLine(q, z, thickness, color);
            }

            Drawing.DrawLine(Drawing.WorldToScreen(new Vector3(new Vector2(Points[Points.Length - 1].X, Points[Points.Length - 1].Y),
                                Player.Position.Z)), Drawing.WorldToScreen(new Vector3(new Vector2(Points[0].X, Points[0].Y),
                                 Player.Position.Z)), thickness, color);
        }

        //  ITEMS
        private static void Items()
        {
            //Bilgewater's Cutlass
            if (root.Item("bilg").GetValue<bool>())
            {
                if (bilgwat.IsOwned() && bilgwat.IsReady())
                {
                    var target = TargetSelector.GetTarget(bilgwat.Range, TargetSelector.DamageType.Physical);
                    if (target != null && !target.IsZombie)
                    {
                        if (target.HealthPercent <= root.Item("enemyBotkr").GetValue<Slider>().Value)
                        {
                            bilgwat.Cast(target);
                        }
                        if (Player.HealthPercent <= root.Item("selfBotkr").GetValue<Slider>().Value)
                        {
                            bilgwat.Cast(target);
                        }
                    }
                }
            }

            //Botrk
            if (root.Item("Botkr").GetValue<bool>())
            {
                if (botrk.IsOwned() && botrk.IsReady())
                {
                    var target = TargetSelector.GetTarget(botrk.Range, TargetSelector.DamageType.Physical);
                    if (target != null && !target.IsZombie)
                    {
                        if (target.HealthPercent <= root.Item("enemyBotkr").GetValue<Slider>().Value)
                        {
                            botrk.Cast(target);
                        }
                        if (Player.HealthPercent <= root.Item("selfBotkr").GetValue<Slider>().Value)
                        {
                            botrk.Cast(target);
                        }
                    }
                }
            }

            //Youumu
            if (root.Item("youm").GetValue<bool>())
            {
                if (yom.IsOwned() && yom.IsReady())
                {
                    var target = TargetSelector.GetTarget(botrk.Range, TargetSelector.DamageType.Physical);
                    {
                        if (target != null && !target.IsZombie)
                        {
                            if (target.HealthPercent <= root.Item("enemyYoumuus").GetValue<Slider>().Value)
                            {
                                yom.Cast();
                            }
                            if (Player.HealthPercent <= root.Item("selfYoumuus").GetValue<Slider>().Value)
                            {
                                yom.Cast();
                            }
                        }
                    }
                }
            }
        }

        private static void Pots()
        {
            if (!Player.InFountain() && !Player.HasBuff("Recall"))
            {
                if (manapotion.IsReady() && !Player.HasBuff("FlaskOfCrystalWater") && root.Item("mp1").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(1200) > 0 && Player.Mana < 200)
                        manapotion.Cast();
                }

                if (Player.HasBuff("RegenerationPotion") || Player.HasBuff("ItemMiniRegenPotion") || Player.HasBuff("ItemCrystalFlask"))
                    return;

                if (flask.IsReady() && root.Item("flask").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(700) > 0 && Player.Health + 200 < Player.MaxHealth)
                        flask.Cast();
                    else if (Player.Health < Player.MaxHealth * 0.6)
                        flask.Cast();
                    else if (Player.CountEnemiesInRange(1200) > 0 && Player.Mana < 200 && !Player.HasBuff("FlaskOfCrystalWater"))
                        flask.Cast();
                    return;
                }

                if (healthpotion.IsReady() && root.Item("hp1").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(700) > 0 && Player.Health + 200 < Player.MaxHealth)
                        healthpotion.Cast();
                    else if (Player.Health < Player.MaxHealth * 0.6)
                        healthpotion.Cast();
                    return;
                }
            }
        }
    }
}
