using System;
using CasterFlash;

namespace CasterFlashGUI.Model
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            //var item = new DataItem(null);
            //item.UnitOp = ViewModel.ViewModelLocator.UnitOp;
            var item = new DataItem(new CasterFlashUnitOp());
            callback(item, null);
        }
    }
}