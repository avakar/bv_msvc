using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace bv_msvc
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("better_verilog")]
    internal class BvClassifierProvider : IClassifierProvider
    {
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty<BvClassifier>(creator: () => new BvClassifier(buffer, this.classificationRegistry));
        }

        [Import]
        private IClassificationTypeRegistryService classificationRegistry;
    }
}
