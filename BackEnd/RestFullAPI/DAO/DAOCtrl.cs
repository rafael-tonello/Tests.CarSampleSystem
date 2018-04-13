using System;
namespace RestFullAPI.DAO
{
    public class DAOCtrl
    {
        public DAOCtrl()
        {
            ConfsCtrl.Instance.ObservateChanges(OnConfsChange);
        }

        private void OnConfsChange(Confs conf, VariantVar var)
        {
            
        }
    }
}
