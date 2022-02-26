using System;
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
