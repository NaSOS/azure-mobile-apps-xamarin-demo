using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace GlobalAzureBootcamp2017.Helpers
{
    public static class FormatHelper
    {
        public static FormattedString FormatStringWithKeyPhrases(string text, IEnumerable<string> keyPhrases){
			var fs = new FormattedString();

            if(keyPhrases==null){
                fs.Spans.Add(new Span{Text=text});
                return fs;
            }

            var indexes = new Dictionary<int, int>();
			
            // sort keyphrases based on their index
			foreach (var keyPhrase in keyPhrases)
			{
				indexes.Add(text.IndexOf(keyPhrase, StringComparison.Ordinal), keyPhrase.Length);
			}

            // sort keys to itterate sequentially
			var keys = indexes.Keys.ToList();
			keys.Sort();
			
            var start = 0;
			foreach (var index in keys)
			{
				var prev = text.Substring(start, index - start);
				var span = text.Substring(index, indexes[index]);
				start = index + indexes[index];
				if (!string.IsNullOrEmpty(prev))
				{
					fs.Spans.Add(new Span { Text = prev });
				}
				if (!string.IsNullOrEmpty(span))
				{
					fs.Spans.Add(new Span { Text = span, FontAttributes = FontAttributes.Bold });
				}
			}

			var rest = text.Substring(start);
			if (!string.IsNullOrEmpty(rest))
			{
				fs.Spans.Add(new Span { Text = rest });
			}

			return fs;
        }
    }
}
