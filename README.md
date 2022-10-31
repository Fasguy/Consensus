# Consensus [obsolete]

An add-on library for Harmony, that allows patching of enumerator methods.

## This library is obsolete

Since Harmony v2.2.1.0, this library is no longer needed for patching `Enumerator`s.
You can now specify a new `MethodType` in the `HarmonyPatch`-Attribute constructor to patch `Enumerator`s

Example:
```cs
[HarmonyPatch(typeof(ClassWithEnumeratorMethod), nameof(SomeEnumeratorMethod), MethodType.Enumerator)]
```

### The problem

One annoying limitation of Harmony, is that it currently does not support patching `Enumerator` methods properly.
`Enumerator` methods are more complicated beneath the surface than they appear, as they just return an object of a hidden class which does all the work.


### The solution

Consensus redirects a Harmony patch from the normal enumerator method to its internal `Current` property or `MoveNext` method instead.

### Known limitations

* The `__instance` variable of `Prefix` and `Postfix` methods receives the underlying Enumerator object instead of the object where the Enumerator was defined.

### Build requirements

Add `0Harmony.dll` from [Harmony](https://github.com/pardeike/Harmony) or [HarmonyX](https://github.com/BepInEx/HarmonyX) to the `lib` directory to build it.

## Usage

### Using the attribute method

After you have created your Harmony instance, you need to pass it to `ConsensusPatcher.Patch` before patching anything.

```cs
Harmony harmonyInstance = new Harmony("your.harmony.id");
ConsensusPatcher.Patch(harmonyInstance);
harmonyInstance.PatchAll();
```

You can add the `[Consensus]` attribute to an existing harmony patch attribute.
The attribute takes an optional parameter, which chooses whether the patch should be redirected to the `Current` property or the `MoveNext` method.
The attribute without parameters will always redirect to the `MoveNext` method.

```cs
[Consensus(PatchEnumerator.MoveNext)]
[HarmonyPatch(typeof(ClassWithEnumeratorMethod), nameof(SomeEnumeratorMethod))]
internal class SomeEnumeratorMethodPatch
{
	...
}
```

Check the `ConsensusTest` project for a real example.

### Manually patching an enumerator

If you want to patch a enumerator method manually, then you can use the `GetCurrent` and `GetMoveNext` helper methods of the `ConsensusPatcher` class.

```cs
Harmony harmonyInstance = new Harmony("your.harmony.id");

MethodInfo original = AccessTools.Method(typeof(ClassWithEnumeratorMethod), nameof(SomeEnumeratorMethod));
MethodInfo prefix = AccessTools.Method(typeof(SomeEnumeratorMethodPatch), nameof(SomeEnumeratorMethodPrefix));
MethodInfo postfix = AccessTools.Method(typeof(SomeEnumeratorMethodPatch), nameof(SomeEnumeratorMethodPostfix));
MethodInfo transpiler = AccessTools.Method(typeof(SomeEnumeratorMethodPatch), nameof(SomeEnumeratorMethodTranspiler));

harmonyInstance.Patch(ConsensusPatcher.GetMoveNext(original), new HarmonyMethod(prefix), new HarmonyMethod(postfix), new HarmonyMethod(transpiler));
```

Check the `ConsensusTest` project for a real example.

## Additional Info

* .NET Standard 2.0
* Visual Studio 2019
* C# 9
