using System;
using CasterFlash;
using CasterFlashGUI.Model;

namespace CasterFlashGUI.Design
{
    public class DesignDataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem(new CasterFlashUnitOp());
            callback(item, null);
        }
    }
}