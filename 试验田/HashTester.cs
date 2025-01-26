using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace 试验田;

public class HashTester
{
	public static void TestHash(string input)
	{
		Console.WriteLine($"Input: {input}");
		Console.WriteLine($"MD5:    {ComputeHash<MD5>(input)}");
		Console.WriteLine($"SHA1:   {ComputeHash<SHA1>(input)}");
		Console.WriteLine($"SHA256: {ComputeHash<SHA256>(input)}");
		Console.WriteLine($"SHA512: {ComputeHash<SHA512>(input)}");
	}

	private static string ComputeHash<T>(string input) where T : HashAlgorithm
	{
		using var algorithm = (T) Activator.CreateInstance(typeof(T));
		byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
		return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
	}
}
