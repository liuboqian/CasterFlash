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
using CasterFlashGUI.Model;
using CasterUnitCore;
using GalaSoft.MvvmLight;

namespace CasterFlashGUI.ViewModel
{
    public class ResultViewModel:ViewModelBase
    {
        private readonly IDataService _dataService;

        public ResultViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.GetData(
                (item, error) =>
                {
                    if (error != null)
                    {
                        // Report error here
                        return;
                    }

                    T = new RealParamViewBase("Outlet Temperature:", item.UnitOp.Results["TOut"] as CapeRealParameter);
                    P = new RealParamViewBase("Outlet Pressure:", item.UnitOp.Results["POut"] as CapeRealParameter);
                    Duty = new RealParamViewBase("Duty:", item.UnitOp.Results["HeatdutyOut"] as CapeRealParameter);
                    VaporFraction = new RealParamViewBase("VaporFraction:", item.UnitOp.Results["VaporFractionOut"] as CapeRealParameter);

                });
        }

        public RealParamViewBase T { get; set; }
        public RealParamViewBase P { get; set; }
        public RealParamViewBase Duty { get; set; }
        public RealParamViewBase VaporFraction { get; set; }
    }
}
