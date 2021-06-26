using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Top50Trigrams
{
	public class ShakespeareTextAnalysis
	{
		Dictionary<string, Trigram> trigramDictionary = new Dictionary<string, Trigram>();

		class Trigram
		{
			public string[] words;
			public string wordsConcat;
			public int count;

			public Trigram(string[] words)
			{
				this.words = words;
				wordsConcat = string.Concat(words);
				count = 0;
			}
		}

		public void GenerateTopTrigramList(string input, string output)
		{
			var inputPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, $"{input}");
			string[] inputFileNames = Directory.GetFiles(inputPath);
			foreach (string fileName in inputFileNames)
			{
				string[] lines = File.ReadAllLines(fileName);
				RefineAndPrepareTrigramDictionary(lines);
			}

			var outputDataSource = trigramDictionary.OrderByDescending(x => x.Value.count).Select(x => x.Value).Take(50).ToList();

			var outputPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, $"{output}");
			using (var writer = new StreamWriter(outputPath))
			{
				writer.WriteLine("Trigram, Count");
				foreach (var item in outputDataSource)
					writer.WriteLine(string.Join(' ', item.words) + "," + item.count);
			}
		}

		private void RefineAndPrepareTrigramDictionary(string[] lines)
		{
			foreach (var line in lines)
			{
				//ignore XML tags, numbers, and punctuation marks

				//Assumption 1: If a line has XML tag then that is the only thing on that line, hence that line can be ignored.
				//Assumption 2: If < symbol is encountered then it is part of the XML tag, hence that line can be ignored.

				//Refine data

				if (line.Contains('<'))
					continue;

				var noSpecialCharacters = Regex.Replace(line, @"[^a-zA-Z\s]", "").Trim();
				var words = noSpecialCharacters.Split(' ');

				//prepare lists with 3 words
				PrepareTrigramDictionary(words);
			}
		}

		private void PrepareTrigramDictionary(string[] words)
		{
			int count = words.Length;
			string[] temp = new string[3];

			//start from zero index and read 3 words at a time and move to next
			for (int i = 0; i + 3 <= count; i++)
			{
				temp = words[i..(i + 3)];

				Trigram obj = new Trigram(temp);

				if (trigramDictionary.ContainsKey(obj.wordsConcat))
					trigramDictionary[obj.wordsConcat].count++;
				else
					trigramDictionary.Add(obj.wordsConcat, obj);
			}
		}
	}
}
