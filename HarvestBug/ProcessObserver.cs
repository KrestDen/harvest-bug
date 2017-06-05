using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarvestBug
{
    interface ProcessObserver
    {
        void NewMessage(string msg);
        void OnFinish();
        void NeedCaptcha(string capchaUrl);
    }
}
