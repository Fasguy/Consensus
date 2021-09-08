using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace Consensus
{
	public static class ConsensusPatcher
	{
		private static readonly string _enumeratorCurrent = $"{typeof(IEnumerator).FullName}.{nameof(IEnumerator.Current)}";

		private static readonly MethodInfo _patchClassTranspiler = AccessTools.Method(typeof(ConsensusPatcher), nameof(PatchClassTranspiler));
		private static readonly MethodInfo _replaceIEnumeratorPatch = AccessTools.Method(typeof(ConsensusPatcher), nameof(ReplaceIEnumeratorPatch));
		private static readonly MethodInfo _merge = AccessTools.Method(typeof(HarmonyMethod), nameof(HarmonyMethod.Merge));

		public static void Patch(Harmony instance)
		{
			ConstructorInfo patchClassConstructor = AccessTools.Constructor(typeof(PatchClassProcessor), new[] { typeof(Harmony), typeof(Type), typeof(bool) });
			instance.Patch(patchClassConstructor, transpiler: new HarmonyMethod(_patchClassTranspiler));
		}

		private static IEnumerable<CodeInstruction> PatchClassTranspiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				yield return instruction;
				if (!instruction.Calls(_merge)) continue;

				yield return new CodeInstruction(OpCodes.Ldarg_2);
				yield return new CodeInstruction(OpCodes.Call, _replaceIEnumeratorPatch);
			}
		}

		private static HarmonyMethod ReplaceIEnumeratorPatch(HarmonyMethod harmonyMethod, Type type)
		{
			ConsensusAttribute consensusAttribute = type.GetCustomAttributes().FirstOrDefault(attr => attr is ConsensusAttribute) as ConsensusAttribute;

			if (consensusAttribute == null) return harmonyMethod;
			
			MethodInfo methodInfo = AccessTools.Method(harmonyMethod.declaringType, harmonyMethod.methodName);
			
			harmonyMethod.declaringType = GetInternalType(methodInfo);
			harmonyMethod.methodName = nameof(IEnumerator.MoveNext);
			if (consensusAttribute.ToPatch == PatchEnumerator.Current)
			{
				harmonyMethod.methodName = _enumeratorCurrent;
				harmonyMethod.methodType = MethodType.Getter;
			}
			return harmonyMethod;
		}

		public static MethodBase GetMoveNext(MethodInfo methodInfo)
		{
			return AccessTools.Method(GetInternalType(methodInfo), nameof(IEnumerator.MoveNext));
		}

		public static MethodBase GetCurrent(MethodInfo methodInfo)
		{
			return AccessTools.PropertyGetter(GetInternalType(methodInfo), _enumeratorCurrent);
		}

		private static Type GetInternalType(MethodBase method)
		{
			MethodBody body = method.GetMethodBody();
			if (body == null) return null;
			byte[] cil = body.GetILAsByteArray();
			for (int idx = 0; idx < cil.Length; ++idx)
			{
				if (cil[idx] == 0x73) // newobj
				{
					int token = cil[++idx];
					token |= cil[++idx] << 8;
					token |= cil[++idx] << 16;
					token |= cil[++idx] << 24;
					return
						method.DeclaringType?.Module.ResolveMethod(token).DeclaringType
						?? throw new ConsensusTypeNotFoundException();
				}
			}

			throw new ConsensusTypeNotFoundException();
		}
	}
}