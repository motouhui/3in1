using Assets.Cha_Data.Script.RitualTactic;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TestFGameServer;
using UnityEngine;
using UnityEngine.UI;

public class RitualBeattleScript : MonoBehaviour
{

    /// <summary>
    /// 英雄对象
    /// </summary>
    public GameObject hero;

    /// <summary>
    /// 魔王
    /// </summary>
    public GameObject boss;

    private Vector3 nowHeroLoc;

    private Vector3 nowBossLoc;

    /// <summary>
    /// 英雄的动作
    /// </summary>
    public CharaMotion HeroMotion;

    /// <summary>
    /// 怪物的动作控制器
    /// </summary>
    public CharaMotion BossMotion;

    private const float DISTANCE = 1.3f;

    private bool beginBattle = false;

    private int bossHp = 100;

    private int heroHp = 100;

    private int bossAtk = 15;

    private int heroAtk = 5;

    public Image boss_hp_Max;

    public Image boss_hp_now;

    public Image hero_hp_Max;

    public Image hero_hp_now;

    public Image boss_sp_now;

    public Image boss_sp_max;

    public Image hero_sp_now;

    public Image hero_sp_max;

    private IDictionary<string, double> heroInfo;

    private IDictionary<string, double> bossInfo;

    private IDictionary<string, double> swordInfo;

    public GameObject game_over_panel;

    private NewServer server;

    private PlayerRole role = PlayerRole.ROLE_NONE;

    private bool isGameStarted = false;

    public GameObject SwordStatusBar;

    public bool isGetKey = false;

    public Text tips;

    public GameObject shied;

    public static void WriteFile(string message)
    {

        var filename = Process.GetCurrentProcess().Id.ToString() + ".txt";
        var stream = new FileStream(filename, FileMode.Append);
        var writer = new StreamWriter(stream);
        writer.WriteLine(message);
        writer.Dispose();
        stream.Dispose();
    }

    // Use this for initialization
    void Start()
    {
        shied.SetActive(false);
        isGetKey = false;
        SwordStatusBar.SetActive(false);
        HeroMotion.PlayWaitLoop();
        BossMotion.PlayWaitLoop();
        server = new NewServer("121.43.109.77", 12345);
        server.AddGameServerAction((Game game) =>
        {
            WriteFile( "game status :" + game.status + " sword Status:" + game.sword_status.ToString() + " Hero Status:" + game.hero_status.ToString() + " Boss Status:" + game.boss_status.ToString());

            double txtheroHp = game.hero.Count > 0 ? game.hero["hp"] : -1f;
            double txtBossHp = game.boss.Count > 0 ? game.boss["hp"] : -1f;
            double txtSwordHp = game.sword.Count > 0 ? game.sword["hp"] : -1f;

            WriteFile("H_B_S:" + ((int)txtheroHp).ToString() + "_" + ((int)txtBossHp).ToString() + "_" + ((int)txtSwordHp).ToString());

            if (role == PlayerRole.ROLE_NONE)
            {
                if (game != null && game.sword.Count != 0)
                {
                    role = PlayerRole.ROLE_SWORD;
                    //SwordStatusBar.SetActive(true);
                }
                else if (game != null && game.boss.Count != 0)
                {
                    role = PlayerRole.ROLE_BOSS;
                }
                else if (game != null && game.hero.Count != 0)
                {
                    role = PlayerRole.ROLE_HERO;
                }

                //_initSceneInfo();
            }

           // if (game.sword.Count != 0 && game.boss.Count != 0 && game.hero.Count != 0)
            //{
           //     PlayAnimation(game);
            //}
        });
        server.AddGameServerConnectedAction((bool connected) =>
        {
            server.Send(NewServer.Command.JOININ, NewServer.Status.IDLE);
        });
        server.Start();

        

        //Game testgame =new Game();
        //testgame.sword_status = (int)NewServer.Status.PASS;
        //testgame.hero_status = (int)NewServer.Status.PASS;
        //testgame.boss_status = (int)NewServer.Status.DEFEND;
        //PlayAnimation(testgame);

        game_over_panel.SetActive(false);
        nowHeroLoc = hero.transform.localPosition;
        nowBossLoc = boss.transform.localPosition;
        nowBossLoc.x = 26.46f;
        boss.transform.localPosition = nowBossLoc;

    }

    private void aniCallback()
    {
        HeroMotion.PlayWaitLoop();
        BossMotion.PlayWaitLoop();
        shied.SetActive(false);
        UnityEngine.Debug.Log("callback");
    }

    private void PlayAnimation(Game game)
    {
        //     if (!isGameStarted)
        //         isGameStarted = true;
        bossInfo = game.boss;
        bossInfo["hp"] = 80;
        bossInfo["max_hp"] = 100;
        bossInfo["mp"] = 80;
        bossInfo["max_mp"] = 100;
        heroInfo = game.hero;
        heroInfo["hp"] = 80;
        heroInfo["max_hp"] = 100;
        heroInfo["mp"] = 80;
        heroInfo["max_mp"] = 100;
        swordInfo = game.sword;
        swordInfo["hp"] = 80;
        swordInfo["max_hp"] = 100;
        swordInfo["mp"] = 80;
        swordInfo["max_mp"] = 100;

        //当三人操作完毕之后才开始渲染
        //剑防御 人攻击 魔攻击
        if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
            (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            HatkSdogeBatk hatkSdogeBatk = new HatkSdogeBatk();
            hatkSdogeBatk.animationPlayOverCallbackEvent += aniCallback;
            hatkSdogeBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人攻击 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
       (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
       (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HatkSdogBpass hatkSdogBpass = new HatkSdogBpass();
            hatkSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hatkSdogBpass.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人攻击 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
   (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
   (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HatkSdogeBdef hatkSdogeBdef = new HatkSdogeBdef();
            hatkSdogeBdef.animationPlayOverCallbackEvent += aniCallback;
            hatkSdogeBdef.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人暴击 魔攻击
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
            (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            HsatkSdogeBatk hsatkSdogeBatk = new HsatkSdogeBatk();
            hsatkSdogeBatk.animationPlayOverCallbackEvent += aniCallback;
            hsatkSdogeBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人暴击 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
            (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HsatkSdogBpass hsatkSdogBpass = new HsatkSdogBpass();
            hsatkSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hsatkSdogBpass.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人暴击 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
        (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
        (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HsatkSdogeBdef hsatkSdogeBdef = new HsatkSdogeBdef();
            hsatkSdogeBdef.animationPlayOverCallbackEvent += aniCallback;
            hsatkSdogeBdef.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人放弃 魔功击
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
            (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            HpassSpassBatk hpassSpassBatk = new HpassSpassBatk();
            hpassSpassBatk.animationPlayOverCallbackEvent += aniCallback;
            hpassSpassBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑防御 人放弃 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
           (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
           (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HpassSdogBpass hpassSdogBpass = new HpassSdogBpass();
            hpassSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hpassSdogBpass.Process(
                );
        }
        //剑防御 人放弃 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.DODGE &&
        (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
        (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HpassSdogBpass hpassSdogBpass = new HpassSdogBpass();
            hpassSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hpassSdogBpass.Process();
        }
        //剑放弃 人放弃 魔功击
        //剑放弃 人攻击 魔攻击
        if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
            (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            bossInfo = game.boss;
            heroInfo = game.hero;
            swordInfo = game.sword;

            HatkSpassBatk hatkSpassBatk = new HatkSpassBatk();

            hatkSpassBatk.animationPlayOverCallbackEvent += aniCallback;
            hatkSpassBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人攻击 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
       (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
       (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HatkSpassBpass hatkSpassBpass = new HatkSpassBpass();
            hatkSpassBpass.animationPlayOverCallbackEvent += aniCallback;
            hatkSpassBpass.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人攻击 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
   (NewServer.Status)game.hero_status == NewServer.Status.ATTACK &&
   (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HatkSpassBdef hatkSpassBdef = new HatkSpassBdef();
            hatkSpassBdef.animationPlayOverCallbackEvent += aniCallback;
            hatkSpassBdef.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人暴击 魔攻击
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
            (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            HsatkSpassBatk hsatkSpassBatk = new HsatkSpassBatk();
            hsatkSpassBatk.animationPlayOverCallbackEvent += aniCallback;
            hsatkSpassBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人暴击 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
            (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
            (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HsatkSpassBpass hsatkSpassBpass = new HsatkSpassBpass();
            hsatkSpassBpass.animationPlayOverCallbackEvent += aniCallback;
            hsatkSpassBpass.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人暴击 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
        (NewServer.Status)game.hero_status == NewServer.Status.CRITICAL_ATTACK &&
        (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HsatkSpassBdef hsatkSpassBdef = new HsatkSpassBdef();
            hsatkSpassBdef.animationPlayOverCallbackEvent += aniCallback;
            hsatkSpassBdef.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人放弃 魔功击
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
            (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
            (NewServer.Status)game.boss_status == NewServer.Status.ATTACK)
        {
            HpassSpassBatk hpassSpassBatk = new HpassSpassBatk();
            hpassSpassBatk.animationPlayOverCallbackEvent += aniCallback;
            hpassSpassBatk.Process(
                HeroMotion,
                BossMotion,
                hero_hp_Max,
                hero_hp_now,
                hero_sp_max,
                hero_sp_now,
                boss_hp_Max,
                boss_hp_now,
                boss_sp_max,
                boss_sp_now,
                heroInfo,
                bossInfo,
                swordInfo);
        }
        //剑放弃 人放弃 魔放弃
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
           (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
           (NewServer.Status)game.boss_status == NewServer.Status.PASS)
        {
            HpassSdogBpass hpassSdogBpass = new HpassSdogBpass();
            hpassSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hpassSdogBpass.Process(
                );
        }
        //剑放弃 人放弃 魔防御
        else if ((NewServer.Status)game.sword_status == NewServer.Status.PASS &&
        (NewServer.Status)game.hero_status == NewServer.Status.PASS &&
        (NewServer.Status)game.boss_status == NewServer.Status.DEFEND)
        {
            shied.SetActive(true);
            HpassSdogBpass hpassSdogBpass = new HpassSdogBpass();
            hpassSdogBpass.animationPlayOverCallbackEvent += aniCallback;
            hpassSdogBpass.Process();
        }
    }

    void OnDestroy()
    {
        server.Close();
        server = null;
        //server.DisConnect();
    }

    private void _initSceneInfo()
    {
        switch (role)
        {
            case PlayerRole.ROLE_HERO:
                {
                    break;
                }
            case PlayerRole.ROLE_BOSS:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// 动作播放回调
    /// </summary>
    /// <param name="aniName"></param>
    private void HeroAnimationCallback(string aniName)
    {
        switch (aniName)
        {
            case "Attack":
                {
                    HeroMotion.PlayWaitLoop();

                    BossMotion.PlayDamage();
                    break;
                }
            case "Damage":
                {
                    heroHp -= bossAtk;

                    heroHp = heroHp < 0 ? 0 : heroHp;

                    hero_hp_now.rectTransform.sizeDelta = new Vector2(hero_hp_Max.rectTransform.rect.width * heroHp / 100, hero_hp_Max.rectTransform.rect.height);

                    if (heroHp <= 0)
                    {
                        game_over_panel.SetActive(true);
                        HeroMotion.PlayDead();
                        return;
                    }

                    HeroMotion.PlayAttack();

                    BossMotion.PlayWaitLoop();
                    break;
                }
        }
    }

    /// <summary>
    /// 动作播放回调
    /// </summary>
    /// <param name="aniName"></param>
    private void BossAnimationCallback(string aniName)
    {
        switch (aniName)
        {
            case "Damage":
                {
                    bossHp -= heroAtk;

                    boss_hp_now.rectTransform.sizeDelta = new Vector2(boss_hp_Max.rectTransform.rect.width * bossHp / 100, boss_hp_Max.rectTransform.rect.height);
                    if (!beginBattle)
                    {
                        beginBattle = true;
                        BossMotion.PlayWaitLoop();
                    }
                    else
                    {
                        BossMotion.PlayAttack();

                        HeroMotion.PlayWaitLoop();
                    }

                    break;
                }
            case "Attack":
                {
                    HeroMotion.PlayDamage();
                    BossMotion.PlayWaitLoop();
                    break;
                }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (role != PlayerRole.ROLE_NONE && !isGetKey)
        {
            switch (role)
            {
                case PlayerRole.ROLE_HERO:
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            isGetKey = true;

                            server.Send(NewServer.Command.DOACTION, NewServer.Status.ATTACK);
                        }
                        else if (Input.GetKeyDown(KeyCode.C))
                        {
                            isGetKey = true;
                            server.Send(NewServer.Command.DOACTION, NewServer.Status.CRITICAL_ATTACK);
                        }
                        break;
                    }
                case PlayerRole.ROLE_BOSS:
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            isGetKey = true;
                            server.Send(NewServer.Command.DOACTION, NewServer.Status.ATTACK);
                        }
                        else if (Input.GetKeyDown(KeyCode.D))
                        {
                            isGetKey = true;
                            server.Send(NewServer.Command.DOACTION, NewServer.Status.DEFEND);
                        }
                        break;
                    }
                case PlayerRole.ROLE_SWORD:
                    {
                        if (Input.GetKeyDown(KeyCode.T))
                        {
                            isGetKey = true;
                            server.Send(NewServer.Command.DOACTION, NewServer.Status.DODGE);
                        }
                        break;
                    }

            }
        }
    }
}
