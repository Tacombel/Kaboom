﻿using KSP.Localization;

namespace Kaboom
{
    /// <summary>
    /// 
    /// </summary>
    public class ModuleKaboom : PartModule
    {
        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = true, guiName = "Kaboom delay",
            groupDisplayName = "<color=red><b>Kaboom Safety Cover</b></color>", groupName = "Kaboom", groupStartCollapsed = true,
            guiUnits = " Seconds"),
            UI_FloatRange(minValue = 0f, maxValue = 30f, stepIncrement = 1f)]

        public float delay = 0;

        [KSPField(isPersistant = true)]
        public bool timerActive = false;

        [KSPField(isPersistant = true)]
        public double kaboomTime;

        [KSPField(isPersistant = true)]
        public bool isGlued = false;

        [KSPField(isPersistant = true, guiName = "Superglue", guiActive = true, guiActiveEditor = true, groupName = "Kaboom")]
        public string gluedText = Localizer.Format("#autoLOC_6001071"); /*Disabled*/


        [KSPEvent(guiName = "Toggle Superglue", groupName = "Kaboom", 
            guiActive = true, guiActiveEditor = true, active = true, guiActiveUncommand = true)]
        public void GluedEvent()
        {
            isGlued = !isGlued;
            if (isGlued)
            {
                gluedText = Localizer.Format("#autoLOC_6001072")/*Enabled*/;
            }
            else
            {
                gluedText = Localizer.Format("#autoLOC_6001071")/*Disabled*/;
            }
        }

        [KSPEvent(guiName = "Kaboom!", groupName = "Kaboom", 
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, active = true, guiActiveUncommand = true)]
        public void KaboomEvent()
        {
            KaboomIt();
        }

        [KSPEvent(guiName = "Cancel Kaboom!", groupName = "Kaboom", 
            guiActive = true, guiActiveUnfocused = true, unfocusedRange = 5f, active = false, guiActiveUncommand = true)]
        public void CancelKaboomEvent()
        {
            CancelKaboomIt();
        }

        [KSPAction("Kaboom!")]
        public void KaboomAction(KSPActionParam _) => KaboomIt();

        private void KaboomIt()
        {
            Events["CancelKaboomEvent"].active = true;
            Events["KaboomEvent"].active = false;
            part.force_activate();

            if (delay == 0)
            {
                bool success = Proceed();
                if (!success)
                    CancelKaboomIt();
            }
            else
            {
                ScreenMessages.PostScreenMessage("Kaboom set for " + delay + " seconds.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                kaboomTime = Planetarium.GetUniversalTime() + delay;
                timerActive = true;
            }
        }

        private bool Proceed()
        {
            if (isGlued)
            {
                var k = new Welding(vessel, part);
                bool success = k.MergeParts(true);
                return success;
            }
            else
            {
                part.explode();
                return true;
            }
        }

        private void CancelKaboomIt()
        {
            Events["CancelKaboomEvent"].active = false;
            Events["KaboomEvent"].active = true;
            ScreenMessages.PostScreenMessage("Kaboom cancelled.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            timerActive = false;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (timerActive)
            {
                if (Planetarium.GetUniversalTime() >= kaboomTime)
                {
                    timerActive = false;
                    bool success = Proceed();
                    if (!success)
                        CancelKaboomIt();
                }
            }
            //base.OnUpdate();
        }
    }
}
