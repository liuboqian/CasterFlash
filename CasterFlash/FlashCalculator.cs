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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CasterUnitCore;

namespace CasterFlash
{
    public class FlashCalculator : Calculator, IFlash
    {
        #region fields

        public FlashOptions flashOption;
        public double T;
        public double P;
        public double heatduty;
        public double vaporFraction;
        public List<MaterialObject> feeds;
        public MaterialObject product;
        public CapeEnergyPort energyPort;

        #endregion

        /// <summary>
        /// Get parameters and materials
        /// </summary>
        public override void BeforeCalculate()
        {
            //get port
            feeds = new List<MaterialObject>();
            foreach (var port in UnitOp.Ports)
            {
                if (port.Value.ComponentName.StartsWith("feed") && ((CapeMaterialPort)port.Value).IsConnected())
                    feeds.Add(((CapeMaterialPort)port.Value).Material.Duplicate());
            }
            energyPort = (CapeEnergyPort)UnitOp.Ports["energy"];
            //get parameters
            flashOption = (FlashOptions)(CapeOptionParameter)UnitOp.Parameters["FlashOption"];
            T = (CapeRealParameter)UnitOp.Parameters["T"];
            P = (CapeRealParameter)UnitOp.Parameters["P"];
            heatduty = (CapeRealParameter)UnitOp.Parameters["Heatduty"];
            if (energyPort.IsConnected()) heatduty += energyPort.Work;
            vaporFraction = (CapeRealParameter)UnitOp.Parameters["VaporFraction"];
        }
        /// <summary>
        /// Calculation
        /// </summary>
        public override void Calculate()
        {
            if (flashOption == FlashOptions.TP)
                TPFlash(T, P, feeds, out product);
            else if (flashOption == FlashOptions.PH)
                PHFlash(P, heatduty, feeds, out product);
            else if (flashOption == FlashOptions.TVf)
                TVFFlash(T, vaporFraction, feeds, out product);
            else if (flashOption == FlashOptions.PVf)
                PVFFlash(P, vaporFraction, feeds, out product);
        }
        /// <summary>
        /// result is get from product
        /// </summary>
        public override void OutputResult()
        {
            ((CapeRealParameter)UnitOp.Results["T"]).value = product.T;
            ((CapeRealParameter)UnitOp.Results["P"]).value = product.P;
            ((CapeRealParameter)UnitOp.Results["Heatduty"]).value = product.Enthalpy;
            foreach (var m in feeds)
                ((CapeRealParameter)UnitOp.Results["Heatduty"]).value -= m.Enthalpy;
            ((CapeRealParameter)UnitOp.Results["VaporFraction"]).value = product.VaporFraction;

            ((CapeMaterialPort)UnitOp.Ports["product"]).Material
                .SetOverallTPFlowCompositionAndFlash(
                product.T, product.P, product.TotalFlow, product.Composition);
        }

        #region IFlash
        /// <summary>
        /// Do TP Flash
        /// I give this method some parameters, 
        /// because I wish other block could use this method.
        /// May be unnecessary, you can do it your way.
        /// </summary>
        public bool TPFlash(double T, double P, List<MaterialObject> feeds, out MaterialObject product)
        {
            if (feeds.Count == 0) { product = null; return false; }
            product = feeds[0].Duplicate();
            product.T = T;
            product.P = P;
            double totalFlow;
            Dictionary<string, double> composition;
            MergeInputMaterial(feeds, out totalFlow, out composition);
            product.TotalFlow = totalFlow;
            product.Composition = composition;
            product.DoTPFlash();
            return true;
        }

        public bool PHFlash(double P, double heatDuty, List<MaterialObject> feeds, out MaterialObject product)
        {
            if (feeds.Count == 0) { product = null; return false; }
            product = feeds[0].Duplicate();
            product.P = P;
            double totalFlow;
            Dictionary<string, double> composition;
            MergeInputMaterial(feeds, out totalFlow, out composition);
            product.TotalFlow = totalFlow;
            product.Composition = composition;
            product.Enthalpy = feeds.Sum(m => m.Enthalpy) + heatDuty;
            product.DoPHFlash();
            return true;
        }

        public bool TVFFlash(double T, double vaporFraction, List<MaterialObject> feeds, out MaterialObject product)
        {
            if (feeds.Count == 0) { product = null; return false; }
            product = feeds[0].Duplicate();
            product.T = T;
            double totalFlow;
            Dictionary<string, double> composition;
            MergeInputMaterial(feeds, out totalFlow, out composition);
            product.TotalFlow = totalFlow;
            product.Composition = composition;
            product.VaporFraction = vaporFraction;
            product.DoTVFFlash();
            return true;
        }

        public bool PVFFlash(double P, double vaporFraction, List<MaterialObject> feeds, out MaterialObject product)
        {
            if (feeds.Count == 0) { product = null; return false; }
            product = feeds[0].Duplicate();
            product.P = P;
            double totalFlow;
            Dictionary<string, double> composition;
            MergeInputMaterial(feeds, out totalFlow, out composition);
            product.TotalFlow = totalFlow;
            product.Composition = composition;
            product.VaporFraction = vaporFraction;
            product.DoPVFlash();
            return true;
        }

        private void MergeInputMaterial(List<MaterialObject> feeds, out double totalFlow, out Dictionary<string, double> composition)
        {
            totalFlow = 0;
            foreach (var material in feeds)
            {
                totalFlow += material.TotalFlow;
            }
            composition = new Dictionary<string, double>();
            foreach (var c in feeds[0].Compounds)
            {
                foreach (var material in feeds)
                {
                    if (!composition.ContainsKey(c))
                        composition[c] = material.CompositionFlow[c];
                    else
                        composition[c] += material.CompositionFlow[c];
                }
                composition[c] /= totalFlow;
            }
        }
        #endregion
    }
}
