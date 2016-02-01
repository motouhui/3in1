using UnityEngine;
using System.Collections;

public class CharaMotion : MonoBehaviour
{

    private float roteSpeed = 0.0f;

    private float animationSpeed = 1.0f;
    private int animationCount;
    private Animation charAnimation;

    private string nowPlayAnimation = "";

    private bool isPlayPause = false;

    private float playPauseAniTime = -1f;

    /// <summary>
    /// 动画播放完毕的托管回调
    /// </summary>
    /// <returns></returns>
    public delegate void animationPlayOverCallback(string aniName);
    public event animationPlayOverCallback animationPlayOverCallbackEvent;

    private bool isPlayed = false;


    // Use this for initialization
    void Start()
    {
        animationCount = GetComponent<Animation>().GetClipCount();
        charAnimation = gameObject.GetComponent<Animation>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayPause)
        {
            if (Time.time * 1000 - playPauseAniTime >= 300f)
            {
                charAnimation.CrossFade("Damage", 0.1f);
                isPlayPause = false;
                playPauseAniTime = -1f;
                nowPlayAnimation = "Damage";
            }
        }

        if (!string.IsNullOrEmpty(nowPlayAnimation))
        {
            if (!AnimationStillPlaying(charAnimation, nowPlayAnimation))
            {
                string callbackAniName = nowPlayAnimation;
                nowPlayAnimation = "";
                animationPlayOverCallbackEvent(callbackAniName);
            }
        }
    }

    /// <summary>
    /// 判断动作是否播放完毕
    /// </summary>
    /// <param name="animationName"></param>
    /// <returns></returns>
    private bool AnimationStillPlaying(Animation animation, string animationName)
    {
        return animation.IsPlaying(animationName) && animation[animationName].normalizedTime < 1.0f;
    }

    /// <summary>
    /// 播放攻击动作
    /// </summary>
    public void PlayAttack()
    {
        charAnimation.Play("Attack");
        charAnimation.wrapMode = WrapMode.Once;
        nowPlayAnimation = "Attack";
    }

    /// <summary>
    /// 播放待机动作
    /// </summary>
    public void PlayWaitLoop()
    {
        charAnimation.Play("Wait");
        charAnimation.wrapMode = WrapMode.Loop;
    }

    public void PlayWalkLoop()
    {
        charAnimation.Play("Walk");
        charAnimation.wrapMode = WrapMode.Loop;
    }

    /// <summary>
    /// 播放受到伤害动画
    /// </summary>
    public void PlayDamage()
    {
        charAnimation.Play("Damage");
        charAnimation.wrapMode = WrapMode.Once;
        nowPlayAnimation = "Damage";
    }

    public void PlayDead()
    {
        charAnimation.Play("Dead");
        charAnimation.wrapMode = WrapMode.Once;
        nowPlayAnimation = "Dead";
    }

    public void PlayAttackPause()
    {
        charAnimation.Play("Attack");
        charAnimation.wrapMode = WrapMode.Once;
        isPlayPause = true;
        playPauseAniTime = Time.time * 1000;

    }

}
