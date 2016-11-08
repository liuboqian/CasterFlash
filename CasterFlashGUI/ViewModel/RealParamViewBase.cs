using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using CasterUnitCore;
using CAPEOPEN;

namespace CasterFlashGUI.ViewModel
{
    public class RealParamViewBase : ViewModelBase
    {
        private CapeRealParameter param;

        public RealParamViewBase(string uiName, CapeRealParameter realParam)
        {
            UIName = uiName;
            param = realParam;
        }

        public string UIName { get; private set; }

        /// <summary>
        /// The <see cref="Value" /> property's name.
        /// </summary>
        public const string ValuePropertyName = "Value";

        /// <summary>
        /// Sets and gets the Value property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Value
        {
            get
            {
                if (param.Mode == CapeParamMode.CAPE_INPUT || param.Mode == CapeParamMode.CAPE_INPUT_OUTPUT)
                {
                    if (double.IsNaN(param.value))
                        return "";
                }
                else //if (param.Mode == CapeParamMode.CAPE_OUTPUT)
                {
                    if (double.IsNaN(param.value))
                        return "--";
                }
                return Units.ConvertFromSI(CurUnit, param.value, param.CurrentUnitCategory).ToString();
            }
            set
            {
                if (param.Mode == CapeParamMode.CAPE_INPUT || param.Mode == CapeParamMode.CAPE_INPUT_OUTPUT)
                {
                    double sivalue;
                    try
                    {
                        sivalue = Units.ConvertToSI(Convert.ToDouble(value), CurUnit, param.CurrentUnitCategory);
                    }
                    catch
                    {
                        sivalue = double.NaN;
                    }
                    if (param.value == sivalue)
                    {
                        return;
                    }

                    param.value = sivalue;
                }
                else //if (param.Mode == CapeParamMode.CAPE_OUTPUT)
                {
                    ;
                }
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurUnit" /> property's name.
        /// </summary>
        public const string CurUnitPropertyName = "CurUnit";

        /// <summary>
        /// Sets and gets the TOutUnit property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurUnit
        {
            get
            {
                return param.CurrentUnit;
            }

            set
            {
                if (param.CurrentUnit == value)
                {
                    return;
                }

                if (param.Mode == CapeParamMode.CAPE_INPUT)
                {
                    string oldValue = Value;
                    //if(_item.T.Mode==CAPEOPEN.CapeParamMode.CAPE_INPUT)
                    param.CurrentUnit = value;
                    //Param in this view model need to change its si value with unit
                    Value = oldValue;     //the display value don't change
                }
                else
                    param.CurrentUnit = value;
                RaisePropertyChanged(CurUnitPropertyName);
                RaisePropertyChanged(ValuePropertyName);
            }
        }

        public IEnumerable<string> UnitList { get { return Units.GetUnitList(param.CurrentUnitCategory); } }

    }
}
