using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CustomControls.Modal.Primitive;

namespace CustomControls.Modal
{
    [TemplatePart(Name = "PART_ModalContainer", Type = typeof(Grid))]
    public class ModalForm : Control
    {
        private const string ELEMENT_MODALCONTAINER = "PART_ModalContainer";
        private Grid _modalContainer;

        private static ModalForm _instance;

        public ModalForm()
        {
            _instance = this;
        }

        static ModalForm()
        {
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ModalForm), new FrameworkPropertyMetadata(typeof(ModalForm)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _modalContainer = this.GetTemplateChild(ELEMENT_MODALCONTAINER) as Grid;
            this.Visibility = Visibility.Collapsed;
        }

        public static ModalResult ShowModal(object content, string title)
        {
            return ModalForm.ShowModal(content, title, ModalButtons.Ok);
        }

        public static ModalResult ShowModal(object content, string title, ModalButtons buttons)
        {
            ModalResult result;
            object key;
            DataTemplate buttonsTemplate = _instance.FindResource(buttons.ToString()) as DataTemplate;
            ModalForm.ShowCustomModal(content, title, buttonsTemplate, out result, out key);
            return result;
        }

        public static void ShowCustomModal(object content, string title, DataTemplate buttonsTemplate, out ModalResult result, out object key)
        {
            ModalForm.DoShowModal(content, title, buttonsTemplate, out result, out key);
        }

        private static void DoShowModal(object content, string title, DataTemplate buttonsTemplate, out ModalResult result, out object key)
        {
            //get previous modal, and set hit=false
            ModalItem previousModal = _instance._modalContainer.Children.OfType<ModalItem>().LastOrDefault();
            if (previousModal != null) previousModal.IsHitTestVisible = false;

            //add modal to queue
            ModalItem modal = new ModalItem();
            modal.ButtonsTemplate = buttonsTemplate;
            _instance._modalContainer.Children.Add(modal);
            modal.ApplyTemplate();

            //show this container
            _instance.Visibility = Visibility.Visible;

            //get result
            modal.DoShowModal(content, title);
            result = modal.Result;
            key = modal.Key;

            //remove modal from queue
            _instance._modalContainer.Children.Remove(modal);

            //reset hit of previous modal
            if (previousModal != null) previousModal.IsHitTestVisible = true;

            //if no more modals, hide this container
            if (_instance._modalContainer.Children.Count == 0)
                _instance.Visibility = Visibility.Collapsed;
        }
    }
}
