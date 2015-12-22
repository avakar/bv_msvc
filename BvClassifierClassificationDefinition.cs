using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace bv_msvc
{
    internal static class BvClassifierClassificationDefinition
    {
        [Export]
        [Name("better_verilog")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition ContentTypeDefinition { get; set; }

        [Export]
        [FileExtension(".bv")]
        [ContentType("better_verilog")]
        internal static FileExtensionToContentTypeDefinition FileExtensionDefinition { get; set; }
    }
}
