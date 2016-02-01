using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TestFGameServer
{
    public class Game
    {
        public int status;
        public string name;
        public int hero_status;
        public int sword_status;
        public int boss_status;
        public IDictionary<string, double> hero = new Dictionary<string, double>();
        public IDictionary<string, double> sword = new Dictionary<string, double>();
        public IDictionary<string, double> boss = new Dictionary<string, double>();

        public const string MP = "mp";

        public const string MAX_MP = "max_mp";

        public const string HP = "hp";

        public const string MAX_HP = "max_hp";



        public static Game Parse(Stream stream)
        {
            Game game = new Game();
            BinaryReader reader = new BinaryReader(stream);
            int magic = reader.ReadInt32();
            int id = reader.ReadInt32();
            int command = reader.ReadInt32();
            int length = reader.ReadInt32();

            game.status = reader.ReadInt32(); length -= 4;
            if (length <= 0) return game;
            int namelen = reader.ReadInt32(); length -= 4;
            if (length <= 0) return game;
            byte[] b = reader.ReadBytes(namelen);
            game.name = UTF8Encoding.UTF8.GetString(b); length -= b.Length;
            if (length <= 0) return game;

            // hero
            {
                int type = reader.ReadInt32(); length -= 4;
                if (length <= 0) return game;

                RitualBeattleScript.WriteFile("hero type:" + type);
                if (type != 0)
                {
                    double max_hp = reader.ReadDouble(); length -= 8;
                    game.hero.Add("max_hp", max_hp);
                    double max_mp = reader.ReadDouble(); length -= 8;
                    game.hero.Add("max_mp", max_mp);
                    double hp = reader.ReadDouble(); length -= 8;
                    game.hero.Add("hp", hp);
                    double mp = reader.ReadDouble(); length -= 8;
                    game.hero.Add("mp", mp);
                    double hp_rate = reader.ReadDouble(); length -= 8;
                    game.hero.Add("hp_recover_rate", hp_rate);
                    double mp_rate = reader.ReadDouble(); length -= 8;
                    game.hero.Add("mp_recover_rate", mp_rate);
                    game.hero_status = reader.ReadInt32(); length -= 4;
                    double mp_consumption = reader.ReadDouble(); length -= 8;
                    game.hero.Add("mp_consumption", mp_consumption);
                    double attack = reader.ReadDouble(); length -= 8;
                    game.hero.Add("attack", attack);
                    double critical_multiply = reader.ReadDouble(); length -= 8;
                    game.hero.Add("critical_multiply", critical_multiply);
                    double critical_rate = reader.ReadDouble(); length -= 8;
                    game.hero.Add("critical_rate", critical_rate);
                }
            }
            
            // sword
            {
                int type = reader.ReadInt32(); length -= 4;
                if (length <= 0) return game;
                RitualBeattleScript.WriteFile("sword type:" + type);
                if (type != 0)
                {
                    double max_hp = reader.ReadDouble(); length -= 8;
                    game.sword.Add("max_hp", max_hp);
                    double max_mp = reader.ReadDouble(); length -= 8;
                    game.sword.Add("max_mp", max_mp);
                    double hp = reader.ReadDouble(); length -= 8;
                    game.sword.Add("hp", hp);
                    double mp = reader.ReadDouble(); length -= 8;
                    game.sword.Add("mp", mp);
                    double hp_rate = reader.ReadDouble(); length -= 8;
                    game.sword.Add("hp_recover_rate", hp_rate);
                    double mp_rate = reader.ReadDouble(); length -= 8;
                    game.sword.Add("mp_recover_rate", mp_rate);
                    game.sword_status = reader.ReadInt32(); length -= 4;
                    double mp_consumption = reader.ReadDouble(); length -= 8;
                    game.sword.Add("mp_consumption", mp_consumption);
                    double attack = reader.ReadDouble(); length -= 8;
                    game.sword.Add("attack", attack);
                    double damage = reader.ReadDouble(); length -= 8;
                    game.sword.Add("damage", damage);
                }
            }

            // boss
            {
                int type = reader.ReadInt32(); length -= 4;
                if (length <= 0) return game;
                RitualBeattleScript.WriteFile("boss type:" + type);
                if (type != 0)
                {
                    double max_hp = reader.ReadDouble(); length -= 8;
                    game.boss.Add("max_hp", max_hp);
                    double max_mp = reader.ReadDouble(); length -= 8;
                    game.boss.Add("max_mp", max_mp);
                    double hp = reader.ReadDouble(); length -= 8;
                    game.boss.Add("hp", hp);
                    double mp = reader.ReadDouble(); length -= 8;
                    game.boss.Add("mp", mp);
                    double hp_rate = reader.ReadDouble(); length -= 8;
                    game.boss.Add("hp_recover_rate", hp_rate);
                    double mp_rate = reader.ReadDouble(); length -= 8;
                    game.boss.Add("mp_recover_rate", mp_rate);
                    game.boss_status = reader.ReadInt32(); length -= 4;
                    double mp_consumption = reader.ReadDouble(); length -= 8;
                    game.boss.Add("mp_consumption", mp_consumption);
                    double attack = reader.ReadDouble(); length -= 8;
                    game.boss.Add("attack", attack);
                    double critical_multiply = reader.ReadDouble(); length -= 8;
                    game.boss.Add("critical_multiply", critical_multiply);
                    double critical_rate = reader.ReadDouble(); length -= 8;
                    game.boss.Add("critical_rate", critical_rate);
                }
            }

            return game;
        }
    }
}
