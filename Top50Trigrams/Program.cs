using System;
using System.Configuration;

namespace Top50Trigrams
{
	class Program
	{
		static void Main(string[] args)
		{
			ShakespeareTextAnalysis obj = new ShakespeareTextAnalysis();
			obj.GenerateTopTrigramList(
				input: ConfigurationManager.AppSettings.Get("TextCases"),
				output: ConfigurationManager.AppSettings.Get("OutputFolder")
			);
		}
	}
}
