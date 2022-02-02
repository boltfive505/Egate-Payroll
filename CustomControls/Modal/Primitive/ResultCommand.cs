using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomControls.Modal.Primitive
{
    public class ResultCommand
    {
        readonly Action<ModalResult, object> _execute;

        public ResultCommand(Action<ModalResult, object> execute)
        {
            if (execute == null)
                throw new ArgumentException("execute cannot be null");
            _execute = execute;
        }

        public void Execute(ModalResult result, object key)
        {
            _execute(result, key);
        }
    }
}
