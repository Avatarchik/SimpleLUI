@echo off

echo "Clearing content..."
@RD /S /Q "Deploy"

call copy_deploy.bat