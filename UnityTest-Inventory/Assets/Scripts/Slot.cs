using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public Item Item { get; set; }

    [SerializeField] private float tweenTime;

    private Image frame;

    private void Awake()
    {
        frame = GetComponent<Image>();
        Item = null;
    }

    public string Select()
    {
        frame.enabled = true;
        LeanTween.scale(gameObject, new Vector3(1.2f, 1.2f, 1.2f), tweenTime).setLoopPingPong();

        return (Item != null) ? Item.Name : "";
    }

    public string Reread()
    {
        return (Item != null) ? Item.Name : "";
    }

    public void Deselect()
    {
        if(Item == null && transform.childCount > 0) {
            Destroy(transform.GetChild(0).gameObject);
        }

        frame.enabled = false;
        LeanTween.scale(gameObject, new Vector3(1f, 1f, 1f), 0);
        LeanTween.cancel(gameObject);
    }
}