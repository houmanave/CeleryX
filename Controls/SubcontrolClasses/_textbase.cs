using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Celery.Controls.SubcontrolClasses
{
    public abstract class _textbase : TextBlock
    {
        private bool _isRightOriented;
        private bool _isBottomOriented;
        private string _observableText;

        public bool IsRightOriented
        {
            get
            {
                return _isRightOriented;
            }

            set
            {
                _isRightOriented = value;
            }
        }

        public bool IsBottomOriented
        {
            get
            {
                return _isBottomOriented;
            }

            set
            {
                _isBottomOriented = value;
            }
        }

        public string ObservableText
        {
            get
            {
                return _observableText;
            }

            set
            {
                _observableText = value;
                this.Text = _observableText;
            }
        }

        public _textbase()
        {

        }

        public virtual void Regenerate(Point p)
        {
        }
    }
}
