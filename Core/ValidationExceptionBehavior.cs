using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Core.Interface;

namespace Core
{
    public class ValidationExceptionBehavior : Behavior<FrameworkElement>
    {

        private int validationExceptionCount = 0;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Validation.ErrorEvent,
                new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }

        private void OnValidationError(object sender, ValidationErrorEventArgs args)
        {
            var handler = AssociatedObject.DataContext as IValidationExceptionHandler;

            if (handler == null) return;

            var element = args.OriginalSource as UIElement;

            if (element == null) return;

            if (args.Action == ValidationErrorEventAction.Added)
            {
                validationExceptionCount++;
            }
            else if (args.Action == ValidationErrorEventAction.Removed)
            {
                validationExceptionCount--;
            }

            handler.IsValid = validationExceptionCount == 0;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Validation.ErrorEvent,
                new EventHandler<ValidationErrorEventArgs>(OnValidationError));
        }
    }
}
