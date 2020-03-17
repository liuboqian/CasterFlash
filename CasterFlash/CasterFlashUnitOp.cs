/*Copyright 2016 Caster

* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*     http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.*/

using System.Linq;
using CasterUnitCore;
using System.Runtime.InteropServices;
using CAPEOPEN;

namespace CasterFlash
{
    [ComVisible(true)]
    [Guid("8FFE8B95-FFAB-4032-B652-181AB5206E36")]
    [CapeName("CasterFlash")]
    [CapeDescription("Flash block using CasterUnitCore")]
    [CapeHelpURL("liuboqian2015@outlook.com")]
    public class CasterFlashUnitOp : CasterUnitOperationBase
    {
        public CasterFlashUnitOp()
            : base(new FlashCalculator(), "CasterFlash", "Flash block using CasterUnitBase")
        {
            OnGetPorts += CasterFlash_OnGetPorts;
        }

        public override void InitParameters()
        {
            Parameters.Clear();
            Parameters.Add(new CapeOptionParameter("FlashOption",
                typeof(FlashOptions), CapeParamMode.CAPE_INPUT));
            Parameters.Add(new CapeRealParameter("T", UnitCategoryEnum.Temperature, CapeParamMode.CAPE_INPUT));
            Parameters.Add(new CapeRealParameter("P", UnitCategoryEnum.Pressure, CapeParamMode.CAPE_INPUT, minVal: 0));
            Parameters.Add(new CapeRealParameter("Heatduty", UnitCategoryEnum.Power, CapeParamMode.CAPE_INPUT, minVal: 0));
            Parameters.Add(new CapeRealParameter("VaporFraction", UnitCategoryEnum.Dimensionless, CapeParamMode.CAPE_INPUT, minVal: 0));
        }

        public override void InitResults()
        {
            Results.Clear();
            Results.Add(new CapeRealParameter("TOut", UnitCategoryEnum.Temperature, CapeParamMode.CAPE_INPUT_OUTPUT));
            Results.Add(new CapeRealParameter("POut", UnitCategoryEnum.Pressure, CapeParamMode.CAPE_OUTPUT, minVal: 0));
            Results.Add(new CapeRealParameter("HeatdutyOut", UnitCategoryEnum.Power, CapeParamMode.CAPE_OUTPUT, minVal: 0));
            Results.Add(new CapeRealParameter("VaporFractionOut", UnitCategoryEnum.Dimensionless, CapeParamMode.CAPE_OUTPUT, minVal: 0));
        }

        public override void InitPorts()
        {
            Ports.Clear();
            Ports.Add(new CapeMaterialPort("feed", CapePortDirection.CAPE_INLET, "feed for Flash"));
            Ports.Add(new CapeMaterialPort("vapor", CapePortDirection.CAPE_OUTLET, "vapor for Flash"));
            Ports.Add(new CapeMaterialPort("liquid", CapePortDirection.CAPE_OUTLET, "liquid for Flash"));
            Ports.Add(new CapeEnergyPort("energy", CapePortDirection.CAPE_INLET));
        }

        void CasterFlash_OnGetPorts(CapeCollection ports)
        {
            int emptyPort = 0;
            foreach (var pair in ports)
            {
                if (!pair.Value.ComponentName.StartsWith("feed")) continue;
                CapeMaterialPort port = (CapeMaterialPort)pair.Value;
                if (!port.IsConnected())
                    emptyPort++;
                //if (emptyPort > 1)    //This will cause load fail in Aspen
                //{
                //    Ports.Remove(pair);
                //    emptyPort--;
                //}
            }
            if (emptyPort == 0)
            {
                int i = 0;
                while (ports.Contains("feed-" + i)) i++;
                ports.Add(new CapeMaterialPort("feed-" + i, CapePortDirection.CAPE_INLET));
            }
        }

        protected override bool PortsValidate(out string message)
        {
            bool hasInlet = Ports.Any(
                x => x.Value is CapeMaterialPort
                    && ((CapeMaterialPort)x.Value).direction == CapePortDirection.CAPE_INLET
                    && ((CapeMaterialPort)x.Value).IsConnected());
            bool hasOutlet = Ports.Any(
                x => x.Value is CapeMaterialPort
                    && ((CapeMaterialPort)x.Value).direction == CapePortDirection.CAPE_OUTLET
                    && ((CapeMaterialPort)x.Value).IsConnected());

            message = "";
            if (!hasInlet) 
                message = "Inlet material missing. ";
            if (!hasOutlet)
                message += "Outlet material missing.";
            return hasInlet && hasOutlet;
        }

        protected override bool ParamtersValidate(out string message)
        {
            //Type cast may be annoying and easy to go wrong,
            //but COM dont support generic type so there is nothing I can do
            message = "";
            if (((CapeOptionParameter)Parameters["FlashOption"]) == FlashOptions.TP
                && ((CapeRealParameter)Parameters["T"]).Validate()
                && ((CapeRealParameter)Parameters["P"]).Validate())
                return true;
            if (((CapeOptionParameter)Parameters["FlashOption"]) == FlashOptions.PH
                && ((CapeRealParameter)Parameters["Heatduty"]).Validate()
                && ((CapeRealParameter)Parameters["P"]).Validate())
                return true;
            if (((CapeOptionParameter)Parameters["FlashOption"]) == FlashOptions.TVf
                && ((CapeRealParameter)Parameters["T"]).Validate()
                && ((CapeRealParameter)Parameters["VaporFraction"]).Validate())
                return true;
            if (((CapeOptionParameter)Parameters["FlashOption"]) == FlashOptions.PVf
                && ((CapeRealParameter)Parameters["VaporFraction"]).Validate()
                && ((CapeRealParameter)Parameters["P"]).Validate())
                return true;

            message = "Parameters incomplete.";
            return false;
        }

   //     protected override void OpenEditWindow()
   //     {
			//casterflashg

   //     }
    }
}
