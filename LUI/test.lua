import ('SimpleLUI', 'SimpleLUI.API.Core')
import ('SimpleLUI', 'SimpleLUI.API.Core.Math')

-- Create new object of name `MyObject` and `MyObject1`
local obj1 = core:Create("MyObject1")
local obj2 = core:Create("MyObject2")

-- Set `MyObject1` anchor as 'Stretch'
obj1.rectTransform:SetAnchor('Stretch')
-- Restart transform position and size so the 'Stretch' transform will fill whole screen.
obj1.rectTransform.anchoredPosition = SLUIVector2.Zero
obj1.rectTransform.sizeDelta = SLUIVector2.Zero

-- Set `MyObject1` as a parent of `MyObject`
obj2.rectTransform:SetParent(obj1.rectTransform)
-- Set `MyObject2` anchor to 'Bottom'
obj2.rectTransform:SetAnchor('Bottom')
-- And update anchored position with size.
obj2.rectTransform.anchoredPosition = SLUIVector2(0, 30)
obj2.rectTransform.sizeDelta = SLUIVector2(200, 40)

local image1 = obj1:AddComponent('Image')
image1.sprite = "your_image.png"
image1.preserveAspect = true;
image1.raycastTarget = true;

local image2 = obj2:AddComponent('Image')
image2.sprite = "LUI//puti1.jpg"

local button2 = image2:AddComponent('Button')
button2:SetOnClick('myOnClick')

local switch = false
function myOnClick()
    if switch then
        image1.sprite = "your_image.jpg"
        switch = false
    else
        image1.sprite = "another_image.jpg"
        switch = true
    end
end