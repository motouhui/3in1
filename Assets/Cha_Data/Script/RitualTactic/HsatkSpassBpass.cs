﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFGameServer;
using UnityEngine.UI;

namespace Assets.Cha_Data.Script.RitualTactic
{
    class HsatkSpassBpass
    {
        private CharaMotion HeroMotion;
        private CharaMotion BossMotion;
        private Image HeroHpMax;
        private Image HeroHpNow;
        private Image HeroSpMax;
        private Image HeroSpNow;
        private Image BossHpMax;
        private Image BossHpNow;
        private Image BossSpMax;
        private Image BossSpNow;
        private IDictionary<string, double> heroInfo;
        private IDictionary<string, double> bossInfo;
        private IDictionary<string, double> swordInfo;

        public delegate void animationPlayOverCallback();
        public event animationPlayOverCallback animationPlayOverCallbackEvent;

        public void Process(
             CharaMotion HeroMotion,
             CharaMotion BossMotion,
             Image HeroHpMax,
             Image HeroHpNow,
             Image HeroSpMax,
             Image HeroSpNow,
             Image BossHpMax,
             Image BossHpNow,
             Image BossSpMax,
             Image BossSpNow,
             IDictionary<string, double> heroInfo,
            IDictionary<string, double> bossInfo,
            IDictionary<string, double> swordInfo
            )
        {
            this.HeroMotion = HeroMotion;
            this.BossMotion = BossMotion;
            this.HeroHpMax = HeroHpMax;
            this.HeroHpNow = HeroHpNow;
            this.HeroSpMax = HeroSpMax;
            this.HeroSpNow = HeroSpNow;
            this.BossHpMax = BossHpMax;
            this.BossHpNow = BossHpNow;
            this.BossSpMax = BossSpMax;
            this.BossSpNow = BossSpNow;
            this.heroInfo = heroInfo;
            this.bossInfo = bossInfo;
            this.swordInfo = swordInfo;

            HeroMotion.animationPlayOverCallbackEvent += HeroAnimationCallback;
            BossMotion.animationPlayOverCallbackEvent += BossAnimationCallback;
            HeroMotion.PlayAttack();
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
                        double nowHeroSpWidth = this.HeroSpMax.rectTransform.rect.width * heroInfo[Game.MP] / heroInfo[Game.MAX_MP];
                        this.HeroSpNow.rectTransform.sizeDelta =
                            new UnityEngine.Vector2((float)nowHeroSpWidth, this.HeroSpNow.rectTransform.rect.height);

                        BossMotion.PlayDamage();
                        break;
                    }
                case "Damage":
                    {
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
                        double nowBossHpWidth = this.BossHpMax.rectTransform.rect.width * bossInfo[Game.HP] / bossInfo[Game.MAX_HP];
                        this.BossHpNow.rectTransform.sizeDelta =
                            new UnityEngine.Vector2((float)nowBossHpWidth, this.BossHpNow.rectTransform.rect.height);

                        HeroMotion.animationPlayOverCallbackEvent -= HeroAnimationCallback;
                        BossMotion.animationPlayOverCallbackEvent -= BossAnimationCallback;

                        animationPlayOverCallbackEvent();

                        break;
                    }
                case "Attack":
                    {

                        break;
                    }
            }
        }
    }
}
