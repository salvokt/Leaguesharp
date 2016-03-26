using LeagueSharp;
using LeagueSharp.Common;

namespace HastaKalistaBaby
{
    class Damage
    {
       public static float GetQDmg(Obj_AI_Base target)
        {
            var dmg = new double[] { 10, 70, 130, 190, 250 }[Program.Q.Level]
                + Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod;
            return (float)ObjectManager.Player.CalcDamage(target,LeagueSharp.Common.Damage.DamageType.Physical,dmg);
        }

        public static float GetWDmg(Obj_AI_Base target)
        {
            var dmg = (new double[] { 5, 7.5, 10, 12.5, 15 }[Program.W.Level] / 100)
            * target.MaxHealth;
            return (float)Program.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Magical, dmg);

        }

        public static float GetEdamage(Obj_AI_Base target)
        {
            if (target.GetBuffCount("kalistaexpungemarker") > 0)
            {
                var dmg =
                    (float)
                        ((new double[] { 20, 30, 40, 50, 60 }[Program.E.Level - 1] +
                          0.6 * (Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod)) +
                         ((target.GetBuffCount("kalistaexpungemarker") - 1) *
                          (new double[] { 10, 14, 19, 25, 32 }[Program.E.Level - 1] +
                           new double[] { 0.2, 0.225, 0.25, 0.275, 0.3 }[Program.E.Level - 1] *
                           (Program.Player.BaseAttackDamage + Program.Player.FlatPhysicalDamageMod))));


                if (Program.Player.HasBuff("summonerexhaust"))
                {
                    dmg *= 0.6f;
                }
                if (Program.Player.HasBuff("urgotcorrosivedebuff"))
                {
                    dmg *= 0.85f;
                }
                if (target.HasBuff("FerociousHowl"))
                {
                    dmg *= 0.3f;
                }   
                if (target.HasBuff("vladimirhemoplaguedebuff"))
                {
                    dmg *= 1.15f;
                }
                if (target.Name.Contains("Baron") && Program.Player.HasBuff("barontarget"))
                {
                    dmg *= 0.5f;
                }
                if (target.Name.Contains("Dragon") && Program.Player.HasBuff("s5test_dragonslayerbuff"))
                {
                    dmg *= (1f - (0.07f * Program.Player.GetBuffCount("s5test_dragonslayerbuff")));
                }


                return
                (float)
                    Program.Player.CalcDamage(target, LeagueSharp.Common.Damage.DamageType.Physical, dmg + target.FlatHPRegenMod);
            }
            return 0;

        }
    }
}
