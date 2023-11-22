using UnityEngine;
using UnityEngine.UI;
public class UIMapSelection: MonoBehaviour
{
    public bool MapLocked { get; set; }
    public Image Locker;
    public Image ImageMap;

    private void Start()
    {
        MapLocked = true;
    }
    public void SetImage(Image image)
    {
        ImageMap.sprite = image.sprite;
    }

    public void SetLocker(bool LockerStatus)
    {
        MapLocked = LockerStatus;
        Locker.gameObject.SetActive(LockerStatus);
    }

}