using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityFramework;

namespace ScT_LanSuite.Controllers
{
    public class SettingsManager
    {
        private UnitOfWork uow = new UnitOfWork();
        public async Task<string> getSettingAsync(string key)
        {
            try
            {
                string setting = (await uow.settingsRepository.FindAsync(x => x.Key == key)).Value;
                return setting;
            }
            catch
            {

                return "Error";
            }

        }
    }
}
