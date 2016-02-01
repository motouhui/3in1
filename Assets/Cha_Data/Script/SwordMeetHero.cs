using UnityEngine;
using System.Collections;

public class SwordMeetHero : MonoBehaviour
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

    /// <summary>
    /// 英雄的动作
    /// </summary>
    public CharaMotion HeroMotion;

    private const float DISTANCE = 1.3f;

    private bool isMeetSword = false;

    private float nowTime = -1f;

    public GameObject gantanhao;

    public GameObject Sword;

    public float showGanTanHaoTime = -1f;

    public float showSwordTime = -1f;

    public GameObject wenhao;

    public float showWenhaoTime = -1f;

    public bool isNeedPushKey = false;

    public bool youAlive = false;

    public bool isGameOver = false;

    public GameObject game_over_panel;

    public
        // Use this for initialization
    void Start()
    {
        game_over_panel.SetActive(false);
        gantanhao.SetActive(false);
        wenhao.SetActive(false);
        nowHeroLoc = hero.transform.localPosition;

        HeroMotion.PlayWalkLoop();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
        {
            return;
        }
        if (isNeedPushKey && Input.GetKeyDown(KeyCode.H))
        {
            Sword.SetActive(false);
            gantanhao.SetActive(false);
            youAlive = true;
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
                if (youAlive)
                {
                    showSwordTime = -1f;
                    wenhao.SetActive(true);
                    showWenhaoTime = Time.time * 1000;

                }
                else
                {
                    game_over_panel.SetActive(true);
                    Debug.Log("Game Over");
                    isGameOver = true;
                }
            }
        }

        if (showWenhaoTime - -1f > 0.000001f)
        {
            if (Time.time * 1000 - showWenhaoTime >= 1000f)
            {
                wenhao.SetActive(false);
                showGanTanHaoTime = -1f;

                HeroMotion.PlayWalkLoop();
            }
        }

        if (!isMeetSword && nowHeroLoc.x > 5.5f)
        {
            isMeetSword = true;
            HeroMotion.PlayWaitLoop();
            nowTime = Time.time * 1000f;
            gantanhao.SetActive(true);
            showGanTanHaoTime = nowTime;
            showSwordTime = nowTime;
            isNeedPushKey = true;
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
                if (nowHeroLoc.x <= 10f)
                {
                    nowHeroLoc.x += 0.03f;
                    hero.transform.localPosition = nowHeroLoc;
                }
                else
                {
                    nowHeroLoc.x = 0f;
                    Sword.SetActive(true);
                    isMeetSword = false;

                    nowTime = -1f;

                    showGanTanHaoTime = -1f;

                    showSwordTime = -1f;

                    showWenhaoTime = -1f;

                    isNeedPushKey = false;

                    youAlive = false;
                }
            }
        }
    }
}
