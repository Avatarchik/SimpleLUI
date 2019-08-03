import ('SimpleLUI', 'SimpleLUI.API.Core')
import ('SimpleLUI', 'SimpleLUI.API.Core.Math')

local obj1 = core:Create("Obj 1")
local obj2 = core:Create("Obj 2")

obj1.rectTransform:SetAnchor('Stretch')
obj1.rectTransform.anchoredPosition = SLUIVector2.Zero
obj1.rectTransform.sizeDelta = SLUIVector2.Zero

obj2.rectTransform:SetParent(obj1.rectTransform)
obj2.rectTransform:SetAnchor('Bottom')
obj2.rectTransform.anchoredPosition = SLUIVector2(0, 30)
obj2.rectTransform.sizeDelta = SLUIVector2(200, 40)

local image1 = obj1:AddComponent('Image')
image1.sprite = "LUI//puti2.jpg";
image1.preserveAspect = true;
image1.raycastTarget = true;

local image2 = obj2:AddComponent('Image')
image2.sprite = "LUI//puti1.jpg"

local button2 = image2:AddComponent('Button')
button2:SetOnClick('onClick2')

local switch = false
function onClick2()
    if switch then
        image1.sprite = "LUI//puti2.jpg"
        switch = false
    else
        image1.sprite = "LUI//puti1.jpg"
        switch = true
    end
end