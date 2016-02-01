using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TestFGameServer;

public class HeroMeetSwordScene : MonoBehaviour {

    /// <summary>
    /// 英雄对象
    /// </summary>
    public GameObject hero;

    /// <summary>
    /// 魔王
    /// </summary>
    public GameObject boss;

    /// <summary>
    /// 场景镜头
    /// </summary>
    public Camera scenCam;

    private Vector3 nowHeroLoc;

    private Vector3 nowBossLoc;

    private Vector3 nowCamLoc;

    /// <summary>
    /// 英雄的动作
    /// </summary>
    public CharaMotion HeroMotion;

    /// <summary>
    /// 怪物的动作控制器
    /// </summary>
    public CharaMotion BossMotion;

    private const float DISTANCE = 1.3f;

    private bool isPlayAttack = false;

    private bool isPlayDemage = false;

    private bool isMeetSword = false;

    private float nowTime = -1f;

    public GameObject gantanhao;

    public GameObject Sword;

    public float showGanTanHaoTime = -1f;

    public float showSwordTime = -1f;

    public GameObject wenhao;

    public float showWenhaoTime = -1f;

    private bool beginBattle = false;

    private int bossHp = 100;

    private int heroHp = 100;

    private int bossAtk = 15;

    private int heroAtk = 5;

    public Image boss_hp_Max;

    public Image boss_hp_now;

    public Image hero_hp_Max;

    public Image hero_hp_now;

    public GameObject game_over_panel;
	// Use this for initialization
	void Start ()
    {
        game_over_panel.SetActive(false);
        gantanhao.SetActive(false);
        wenhao.SetActive(false);
        nowHeroLoc = hero.transform.localPosition;
        nowBossLoc = boss.transform.localPosition;
        nowBossLoc.x = 28;
        boss.transform.localPosition = nowBossLoc;

        nowCamLoc = scenCam.transform.localPosition;

        HeroMotion.animationPlayOverCallbackEvent += HeroAnimationCallback;
        BossMotion.animationPlayOverCallbackEvent += BossAnimationCallback;

        HeroMotion.PlayWalkLoop();
        BossMotion.PlayWaitLoop();


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

                    float nowWidth = hero_hp_Max.rectTransform.rect.width * heroHp / 100;
                    if (nowWidth - 0f >= 0.000001f && nowWidth < 8f)
                    {
                        nowWidth = 8f;
                    }

                    hero_hp_now.rectTransform.sizeDelta = new Vector2(nowWidth, hero_hp_Max.rectTransform.rect.height);

                    if (heroHp <= 0)
                    {
                        HeroMotion.PlayDead();

                        game_over_panel.SetActive(true);
                        return;
                    }
                    if (!beginBattle)
                    {
                        beginBattle = true;
                        HeroMotion.PlayWaitLoop();
                    }
                    else
                    {
                        HeroMotion.PlayAttack();

                        BossMotion.PlayWaitLoop();
                    }
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
                    BossMotion.PlayAttack();

                    HeroMotion.PlayWaitLoop();

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
	void Update () {
        //开始战斗
        if (beginBattle && Input.GetKeyDown(KeyCode.A))
        {
            beginBattle = false;
            HeroMotion.PlayAttack();
        }
        if (showGanTanHaoTime - -1f > 0.000001f)
        {
            if (Time.time * 1000 - showGanTanHaoTime >= 1000f)
            {
                gantanhao.SetActive(false);
                showGanTanHaoTime = -1f;
            }
        }

        if (showSwordTime - -1f > 0.000001f)
        {
            if (Time.time * 1000 - showSwordTime >= 2000f)
            {
                Sword.SetActive(false);
                showSwordTime = -1f;

                wenhao.SetActive(true);
                showWenhaoTime = Time.time * 1000;
            }
        }

        if (showWenhaoTime - -1f > 0.000001f)
        {
            if (Time.time * 1000 - showWenhaoTime >= 1000f)
            {
                wenhao.SetActive(false);
                showGanTanHaoTime = -1f;
            }
        }

        if (!isMeetSword && nowHeroLoc.x > 6f)
        {
            isMeetSword = true;
            HeroMotion.PlayWaitLoop();
            nowTime = Time.time * 1000f;
            gantanhao.SetActive(true);
            showGanTanHaoTime = nowTime;
            showSwordTime = nowTime;
        }
        else
        {
            if (isMeetSword && nowTime - -1f >= 0.00001f)
            {
                
                if (Time.time * 1000f - nowTime >= 3000f)
                {
                    nowTime = -1f;
                    HeroMotion.PlayWalkLoop();
                }
            }
            else
            {
                if (nowHeroLoc.x < nowBossLoc.x - DISTANCE)
                {
                    nowHeroLoc.x += 0.05f;
                    hero.transform.localPosition = nowHeroLoc;

                    if (nowCamLoc.x <= 28f)
                    {
                        nowCamLoc.x += 0.05f;
                        scenCam.transform.localPosition = nowCamLoc;
                    }
                    if (nowHeroLoc.x >= nowBossLoc.x - DISTANCE)
                    {
                        beginBattle = true;
                        HeroMotion.PlayWaitLoop();
                    }
                }
            }
        }
	}
}
