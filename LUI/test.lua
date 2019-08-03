import ('SimpleLUI', 'SimpleLUI.API.Core')
import ('SimpleLUI', 'SimpleLUI.API.Core.Math')

local obj1 = core:Create("Obj 1")
local obj2 = core:Create("Obj 2")

obj1.rectTransform:SetParent(obj2.rectTransform)
obj1.rectTransform.sizeDelta = SLUIVector2(3, 4)