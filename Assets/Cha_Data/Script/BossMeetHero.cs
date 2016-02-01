using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BossMeetHero : MonoBehaviour {

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

    private const float DISTANCE = 1.5f;

    private bool isPlayAttack = false;

    private bool isPlayDemage = false;

    private float nowTime = -1f;

    private bool isFinish = false;

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
    void Start()
    {
        game_over_panel.SetActive(false);
        nowHeroLoc = hero.transform.localPosition;
        nowBossLoc = boss.transform.localPosition;
        nowBossLoc.x = 27;
        boss.transform.localPosition = nowBossLoc;


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
        //开始战斗
        if (beginBattle && Input.GetKeyDown(KeyCode.A))
        {
            beginBattle = false;
            BossMotion.PlayAttack();
        }

        if (nowHeroLoc.x < nowBossLoc.x - DISTANCE)
        {
            nowHeroLoc.x += 0.05f;
            hero.transform.localPosition = nowHeroLoc;

            if (nowHeroLoc.x >= nowBossLoc.x - DISTANCE)
            {
                beginBattle = true;
                HeroMotion.PlayWaitLoop();

            }
        }
    }
}
