using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControls.Modal
{
    public class ModalClosedArgs
    {
        public ModalResult Result { get; private set; }
        public object Key { get; private set; }

        public ModalClosedArgs(ModalResult result, object key)
        {
            this.Result = result;
            this.Key = key;
        }
    }
}
