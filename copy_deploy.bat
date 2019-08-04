@echo off
echo "Moving build content..."
robocopy Assets\Plugins\NLua Deploy\NLua /S /xf *.meta
robocopy Library\ScriptAssemblies Deploy /S SimpleLUI.dll SimpleLUI.dll.pdb SimpleLUI.Editor.dll SimpleLUI.Editor.dll.pdb
robocopy Library\ScriptAssemblies Deploy /S SimpleLUI.JEM.dll SimpleLUI.JEM.dll.pdb SimpleLUI.Editor.JEM.dll SimpleLUI.JEM.Editor.dll.pdb
echo "Build done!"