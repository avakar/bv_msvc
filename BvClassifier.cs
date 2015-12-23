using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Language.StandardClassification;
using System.Text.RegularExpressions;

namespace bv_msvc
{
    internal class BvClassifier : IClassifier
    {
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (!span.Snapshot.Version.Equals(m_cachedVersion))
            {
                this.Reclassify();
            }

            var result = new List<ClassificationSpan>();
            foreach (var cachedSpan in m_spans)
            {
                if (cachedSpan.Span.OverlapsWith(span))
                    result.Add(cachedSpan);
            }

            return result;
        }

        internal BvClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            m_buffer = buffer;
            m_ct_keyword = registry.GetClassificationType(PredefinedClassificationTypeNames.Keyword);
            m_ct_comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            m_ct_number = registry.GetClassificationType(PredefinedClassificationTypeNames.Number);
            m_ct_atom = registry.GetClassificationType(PredefinedClassificationTypeNames.String);

            m_regex = new Regex(@"(\#[^\n]*)|\b(interface|struct|enum|module|def|use|inst|always|on|or|and|posedge|negedge|if|else|switch|wait|assert)\b|(\d+'[bodh][_0-9a-fA-Fxz?]+|\d+)|'[a-zA-Z_][a-zA-Z_0-9]*");
        }

        private void Reclassify()
        {
            ITextSnapshot ss = m_buffer.CurrentSnapshot;
            string text = ss.GetText();

            List<ClassificationSpan> spans = new List<ClassificationSpan>();
            foreach (var classifiedSpan in GetClassifiedSpans(text))
            {
                spans.Add(new ClassificationSpan(new SnapshotSpan(ss, classifiedSpan.start, classifiedSpan.length), classifiedSpan.ct));
            }

            m_cachedVersion = m_buffer.CurrentSnapshot.Version;
            m_spans = spans;
        }

        private class ClassifiedSpan
        {
            public int start;
            public int length;
            public IClassificationType ct;

            public ClassifiedSpan(int start, int length, IClassificationType ct)
            {
                this.start = start;
                this.length = length;
                this.ct = ct;
            }
        };

        private IEnumerable<ClassifiedSpan> GetClassifiedSpans(string text)
        {
            foreach (Match match in m_regex.Matches(text))
            {
                IClassificationType ct;
                if (match.Groups[1].Success)
                    ct = m_ct_comment;
                else if (match.Groups[2].Success)
                    ct = m_ct_keyword;
                else if (match.Groups[3].Success)
                    ct = m_ct_number;
                else
                    ct = m_ct_atom;
                yield return new ClassifiedSpan(match.Index, match.Length, ct);
            }
        }

        private readonly IClassificationType m_ct_keyword;
        private readonly IClassificationType m_ct_comment;
        private readonly IClassificationType m_ct_number;
        private readonly IClassificationType m_ct_atom;
        private readonly ITextBuffer m_buffer;

        private readonly Regex m_regex;

        private ITextVersion m_cachedVersion;
        private List<ClassificationSpan> m_spans;
    }
}
