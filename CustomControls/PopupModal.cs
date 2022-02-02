using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace CustomControls
{
    public class PopupModal : Popup
    {
        private ICommand _submitCommand;
        public ICommand SubmitCommand
        {
            get { return _submitCommand ?? (_submitCommand = new RelayCommand(SubmitModal)); }
        }

        private ICommand _closeCommand;
        public ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(CloseModal)); }
        }

        private ICommand _deletingCommand;
        public ICommand DeletingCommand
        {
            get { return _deletingCommand ?? (_deletingCommand = new RelayCommand(DeletingModal)); }
        }

        public bool IsSubmitted { get; private set; }
        public bool IsDeleting { get; private set; }

        private DispatcherFrame frame;

        public PopupModal()
        {
            this.IsOpen = false;
            this.StaysOpen = false;
            this.AllowsTransparency = true;
        }

        private void OpenModal(object obj)
        {
            this.IsOpen = true;
        }

        private void SubmitModal(object obj)
        {
            CloseModal(obj);
            IsSubmitted = true;
        }

        private void DeletingModal(object obj)
        {
            CloseModal(obj);
            IsSubmitted = true;
            IsDeleting = true;
        }

        private void CloseModal(object obj)
        {
            this.IsOpen = false;
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            IsSubmitted = false;
            frame = new DispatcherFrame();
            Dispatcher.PushFrame(frame);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (frame != null) frame.Continue = false;
            base.OnClosed(e);
        }
    }
}
