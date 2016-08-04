Add Revit dll's to projects (see autodesk tutorials)

Update Revit Path on DISS_Addin.addin file

copy "$(ProjectDir)DISS_Addin.addin" "$(AppData)\Autodesk\REVIT\Addins\2016"
copy "$(ProjectDir)bin\Debug\DISS_Addin.dll" "$(AppData)\Autodesk\REVIT\Addins\2016"