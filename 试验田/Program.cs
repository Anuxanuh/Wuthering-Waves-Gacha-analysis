namespace 试验田
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var finder = new GachaUrlFinder();
			var result = finder.FindUrl();

			Console.WriteLine("\nFinal Result:");
			Console.WriteLine($"Success: {result.Success}");
			Console.WriteLine($"URL: {result.Url}");
			Console.WriteLine($"Checked Paths: {string.Join("\n  ", result.CheckedPaths)}");
			Console.WriteLine($"Errors:\n{result.ErrorLog}");
		}
	}
}
