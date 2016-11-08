using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CasterFlash;
using CasterUnitCore;

namespace CasterFlashGUI.Model
{
    public class DataItem
    {
        public CasterFlashUnitOp UnitOp { get; set; }

        public DataItem(CasterFlashUnitOp unit)
        {
            UnitOp = unit;
        }

        public string Title
        {
            get { return UnitOp.ComponentName; }
        }

        //#region Input Parameter

        //public CapeOptionParameter FlashOption { get { return UnitOp.Parameters["FlashOption"] as CapeOptionParameter; } }
        //public CapeRealParameter T { get { return UnitOp.Parameters["T"] as CapeRealParameter; } }
        //public CapeRealParameter P { get { return UnitOp.Parameters["P"] as CapeRealParameter; } }
        //public CapeRealParameter Duty { get { return UnitOp.Parameters["Heatduty"] as CapeRealParameter; } }
        //public CapeRealParameter VaporFraction { get { return UnitOp.Parameters["VaporFraction"] as CapeRealParameter; } }
        
        //#endregion

        //#region Output Parameters

        //public CapeRealParameter Tout { get { return UnitOp.Results["T"] as CapeRealParameter; } }

        //#endregion

        //#region Ports

        //#endregion
    }
}
