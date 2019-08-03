import ('SimpleLUI', 'SimpleLUI.API.Core')
import ('SimpleLUI', 'SimpleLUI.API.Core.Math')

local obj1 = core:Create("Obj 1")
local obj2 = core:Create("Obj 2")

obj1.rectTransform:SetParent(obj2.rectTransform)
obj1.rectTransform.sizeDelta = SLUIVector2(128, 128)

local t = obj1:AddComponent('Image')
debug:Log(t.Hello)

obj2.rectTransform:SetAnchor('Stretch')
obj2.rectTransform.anchoredPosition = SLUIVector2(0, 0)
obj2.rectTransform.sizeDelta = SLUIVector2(0, 0)