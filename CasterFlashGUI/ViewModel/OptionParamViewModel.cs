using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using CAPEOPEN;
using CasterFlash;
using CasterUnitCore;

namespace CasterFlashGUI.ViewModel
{
    public class OptionParamViewModel:ViewModelBase
    {

        public OptionParamViewModel(string uiName,CapeOptionParameter optionParam)
        {
            param = optionParam;
            UIName = uiName;

            #region get Op1 and Op2 from _specOption
            if (param == CasterFlash.FlashOptions.TP)
            {
                Op1 = "Temperature";
                Op2 = "Pressure";
            }
            else if (param == CasterFlash.FlashOptions.PH)
            {
                Op1 = "Pressure";
                Op2 = "Duty";
            }
            else if (param == CasterFlash.FlashOptions.TVf)
            {
                Op1 = "Temperature";
                Op2 = "VaporFraction";
            }
            else if (param == CasterFlash.FlashOptions.PVf)
            {
                Op1 = "Pressure";
                Op2 = "VaporFraction";
            }
            #endregion
        }

        public string UIName { get; private set; }

        public CapeOptionParameter param;

        /// <summary>
        /// The <see cref="Op1" /> property's name.
        /// </summary>
        public const string Op1PropertyName = "Op1";

        private string _Op1Property;

        /// <summary>
        /// Sets and gets the Op1 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Op1
        {
            get
            {
                return _Op1Property;
            }

            set
            {
                if (_Op1Property == value)
                {
                    return;
                }

                _Op1Property = value;

                UpdateFlashOptionParam();
                RaisePropertyChanged(Op1ListPropertyName);
                RaisePropertyChanged(Op2ListPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Op2" /> property's name.
        /// </summary>
        public const string Op2PropertyName = "Op2";

        private string _Op2Property;

        /// <summary>
        /// Sets and gets the Op2 property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string Op2
        {
            get
            {
                return _Op2Property;
            }

            set
            {
                if (_Op2Property == value)
                {
                    return;
                }

                _Op2Property = value;

                UpdateFlashOptionParam();
                RaisePropertyChanged(Op1ListPropertyName);
                RaisePropertyChanged(Op2ListPropertyName);
            }
        }

        /// <summary>
        /// Update the real option parameter param by Op1 and Op2
        /// </summary>
        private void UpdateFlashOptionParam()
        {
            string temp = Op1 + Op2;
            if (temp.Contains("Temperature") && temp.Contains("Pressure"))
                param.value = CasterFlash.FlashOptions.TP;
            else if (temp.Contains("Pressure") && temp.Contains("Duty"))
                param.value = CasterFlash.FlashOptions.PH;
            else if (temp.Contains("Temperature") && temp.Contains("VaporFraction"))
                param.value = CasterFlash.FlashOptions.TVf;
            else if (temp.Contains("Pressure") && temp.Contains("VaporFraction"))
                param.value = CasterFlash.FlashOptions.PVf;
        }

        public IEnumerable<string> AvailiableOptions
        { get { return new[] { "Temperature", "Pressure", "Duty", "VaporFraction" }; } }

        /// <summary>
        /// The <see cref="Op1List" /> property's name.
        /// </summary>
        public const string Op1ListPropertyName = "Op1List";

        /// <summary>
        /// Sets and gets the Op1List property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<string> Op1List
        {
            get
            {
                if (Op2 == "Temperature" || Op2 == "Pressure")
                    return AvailiableOptions.Except(new[] { Op2 });
                else
                    return new[] { "Temperature", "Pressure" };
            }
        }

        /// <summary>
        /// The <see cref="Op2List" /> property's name.
        /// </summary>
        public const string Op2ListPropertyName = "Op2List";

        /// <summary>
        /// Sets and gets the Op2List property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public IEnumerable<string> Op2List
        {
            get
            {
                if (Op1 == "Temperature" || Op1 == "Pressure")
                    return AvailiableOptions.Except(new[] { Op1 });
                else
                    return new[] { "Temperature", "Pressure" };

            }
        }
    }
}
