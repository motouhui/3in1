using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Cha_Data.Script.RitualTactic
{
    class HpassSpassBdef
    {
        public delegate void animationPlayOverCallback();
        public event animationPlayOverCallback animationPlayOverCallbackEvent;
        public void Process()
        {
            animationPlayOverCallbackEvent();
        }
    }
}
