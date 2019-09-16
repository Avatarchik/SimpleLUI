//
// JEM For Unity Source
//
// Copyright (c) 2019 ADAM MAJCHEREK ALL RIGHTS RESERVED
//

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace JEM.UnityEditor.VersionManagement
{
    /// <summary>
    ///     Simple class that will handle for us OnPreprocessBuild event to increase the BuildNumber.
    /// </summary>
    internal class JEMBuildPreprocess : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;
        public void OnPreprocessBuild(BuildReport report) => JEMBuildEditor.IncreaseBuildNumber();    
    }
}
