using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MBODM.WPF
{
    public sealed class InvertBoolExtension : MarkupExtension
    {
        public InvertBoolExtension()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new InvertBoolConverter();
        }
    }
}
