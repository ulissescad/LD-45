using UnityEngine;
using UnityEngine.UI;
public class UIMapSelection: MonoBehaviour
{
    public Image ImageMap;
    
    public void SetImage(Image image)
    {
        ImageMap.sprite = image.sprite;
    }
}