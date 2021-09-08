using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Consensus;
using HarmonyLib;

namespace ConsensusTest
{
	internal class TestCaseVariables
	{
		public static string[] TestValues { get; } =
		{
			"first",
			"second",
			"third"
		};

		public static string PrefixTemplate =>
			@"
Prefix called.
- Called from Instance: {0}";

		public static string PostfixTemplate =>
			@"
Postfix called.
- Called from Instance: {0}
- Received the result: {1}";
	}

	internal class TestCase
	{
		public IEnumerator EnumeratorTestNormal()
		{
			foreach (string testValue in TestCaseVariables.TestValues)
			{
				yield return $"This is the {testValue} standard value.";
			}
		}

		public IEnumerator EnumeratorTestConsensusCurrent()
		{
			foreach (string testValue in TestCaseVariables.TestValues)
			{
				yield return $"This is the {testValue} standard value.";
			}
		}

		public IEnumerator EnumeratorTestConsensusMoveNext()
		{
			foreach (string testValue in TestCaseVariables.TestValues)
			{
				yield return $"This is the {testValue} standard value.";
			}
		}
	}

	[HarmonyPatch(typeof(TestCase), nameof(TestCase.EnumeratorTestNormal))]
	internal class TestCaseNormalPatch
	{
		private static void Prefix(object __instance)
		{
			Console.WriteLine(TestCaseVariables.PrefixTemplate, __instance);
		}
		private static void Postfix(object __instance, ref object __result)
		{
			Console.WriteLine(TestCaseVariables.PostfixTemplate, __instance, __result);

		}
	}

	[Consensus(PatchEnumerator.Current)]
	[HarmonyPatch(typeof(TestCase), nameof(TestCase.EnumeratorTestConsensusCurrent))]
	internal class TestCaseConsensusPatchCurrent
	{
		private static void Prefix(object __instance)
		{
			Console.WriteLine(TestCaseVariables.PrefixTemplate, __instance);
		}
		private static void Postfix(object __instance, ref object __result)
		{
			Console.WriteLine(TestCaseVariables.PostfixTemplate, __instance, __result);
		}
	}

	[Consensus(PatchEnumerator.MoveNext)]
	[HarmonyPatch(typeof(TestCase), nameof(TestCase.EnumeratorTestConsensusMoveNext))]
	internal class TestCaseConsensusPatchMoveNext
	{
		private static void Prefix(object __instance)
		{
			Console.WriteLine(TestCaseVariables.PrefixTemplate, __instance);
		}
		private static void Postfix(object __instance, ref object __result)
		{
			Console.WriteLine(TestCaseVariables.PostfixTemplate, __instance, __result);
		}

		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Ldstr && instruction.operand is string text)
				{
					instruction.operand = text.Replace("standard", "transpile-manipulated");
				}
				yield return instruction;
			}
		}
	}
}