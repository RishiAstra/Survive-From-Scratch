#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class BuildWarner : IPreprocessBuildWithReport
{
	public int callbackOrder { get { return 0; } }
	public void OnPreprocessBuild(BuildReport report)
	{
		Debug.LogWarning(
			"Remember to rebuild AddressableAssets before build to prevent bugs from outdated data"
		);
	}
}
#endif