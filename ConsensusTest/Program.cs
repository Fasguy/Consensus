using System;
using System.Collections;
using Consensus;
using HarmonyLib;

namespace ConsensusTest
{
	internal class Program
	{
		private static void Main()
		{
			Console.WriteLine("Enumerator without patch:");

			TestCase testCaseInstance = new TestCase();

			IEnumerator enumerator = testCaseInstance.EnumeratorTestNormal();

			while (enumerator.MoveNext())
			{
				Console.WriteLine(enumerator.Current);
			}

			Harmony harmony = new Harmony("fasguy.consensus.test");
			ConsensusPatcher.Patch(harmony);
			harmony.PatchAll();

			//The commented-out section below is an example of a manual patch

			//MethodInfo normalOriginal = AccessTools.Method(typeof(TestCase), nameof(TestCase.EnumeratorTestNormal));
			//MethodInfo normalPrefix = AccessTools.Method(typeof(TestCaseNormalPatch), "Prefix");
			//MethodInfo normalPostfix = AccessTools.Method(typeof(TestCaseNormalPatch), "Postfix");

			//harmony.Patch(normalOriginal, new HarmonyMethod(normalPrefix), new HarmonyMethod(normalPostfix));

			//MethodInfo currentOriginal = AccessTools.Method(typeof(TestCase), nameof(TestCase.EnumeratorTestConsensusCurrent));
			//MethodInfo currentPrefix = AccessTools.Method(typeof(TestCaseConsensusPatchCurrent), "Prefix");
			//MethodInfo currentPostfix = AccessTools.Method(typeof(TestCaseConsensusPatchCurrent), "Postfix");

			//harmony.Patch(ConsensusPatcher.GetCurrent(currentOriginal), new HarmonyMethod(currentPrefix), new HarmonyMethod(currentPostfix));

			//MethodInfo moveNextOriginal = AccessTools.Method(typeof(TestCase), nameof(TestCase.EnumeratorTestConsensusMoveNext));
			//MethodInfo moveNextPrefix = AccessTools.Method(typeof(TestCaseConsensusPatchMoveNext), "Prefix");
			//MethodInfo moveNextPostfix = AccessTools.Method(typeof(TestCaseConsensusPatchMoveNext), "Postfix");
			//MethodInfo moveNextTranspiler = AccessTools.Method(typeof(TestCaseConsensusPatchMoveNext), "Transpiler");

			//harmony.Patch(ConsensusPatcher.GetMoveNext(moveNextOriginal), new HarmonyMethod(moveNextPrefix), new HarmonyMethod(moveNextPostfix), new HarmonyMethod(moveNextTranspiler));

			Header("Enumerator after patch without Consensus:");

			enumerator = testCaseInstance.EnumeratorTestNormal();

			while (enumerator.MoveNext())
			{
				Console.WriteLine(enumerator.Current);
			}

			Header("Enumerator after 'Current' patch with Consensus:");

			enumerator = testCaseInstance.EnumeratorTestConsensusCurrent();

			while (enumerator.MoveNext())
			{
				Console.WriteLine(enumerator.Current);
			}

			Header("Enumerator after 'MoveNext' patch with Consensus:");

			enumerator = testCaseInstance.EnumeratorTestConsensusMoveNext();

			while (enumerator.MoveNext())
			{
				Console.WriteLine(enumerator.Current);
			}

			Console.ReadKey();
		}

		private static void Header(string text)
		{
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("#######################################################################");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine(text);
		}
	}
}