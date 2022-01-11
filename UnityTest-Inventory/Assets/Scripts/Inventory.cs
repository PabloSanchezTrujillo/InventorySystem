using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private Text itemText;
    [SerializeField] private Text resolutionText;
    [SerializeField] private float secondsToHide;
    [SerializeField] private GameObject[] items;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClips audioClips;
    [SerializeField] private float tweenTime;

    private GameObject selectedSlot;
    private int slotId;
    private bool itemPicked;
    private GameObject selectedItem;
    private int resolutionId;
    private Vector2 currentResolution;

    // Start is called before the first frame update
    private void Start()
    {
        itemPicked = false;

        foreach(GameObject slot in slots) {
            slot.GetComponent<Image>().enabled = false;
        }

        PlaceItems();
        selectedSlot = slots[0];
        slotId = 0;
        itemText.text = selectedSlot.GetComponent<Slot>().Select();
        LeanTween.scale(selectedSlot, new Vector3(1.2f, 1.2f, 1.2f), tweenTime).setLoopPingPong();

        resolutionId = 1;
        currentResolution = GetComponent<ResolutionSettings>().Resolutions[resolutionId];
        Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, FullScreenMode.Windowed);
    }

    private void PlaceItems()
    {
        for(int i = 0; i < 5; i++) {
            int randSlot = Random.Range(0, slots.Length);
            while(slots[randSlot].GetComponent<Slot>().Item != null) {
                randSlot = Random.Range(0, slots.Length);
            }

            int randItem = Random.Range(0, items.Length);
            Instantiate(items[randItem], slots[randSlot].transform);
            slots[randSlot].GetComponent<Slot>().Item = items[randItem].GetComponent<Item>();
        }
    }

    private void ClearAllItems()
    {
        foreach(GameObject slot in slots) {
            if(slot.GetComponent<Slot>().Item != null) {
                Destroy(slot.transform.GetChild(0).gameObject);
                slot.GetComponent<Slot>().Item = null;
            }
        }
    }

    public void ClearButton(InputAction.CallbackContext value)
    {
        if(value.started) {
            // NO item is picked
            if(!itemPicked) {
                ClearAllItems();
                PlaceItems();
                PlaySound(audioClips.resetItems);

                itemText.text = selectedSlot.GetComponent<Slot>().Select();
            }
            // An item is picked
            else {
                Destroy(selectedItem.gameObject);
                PlaySound(audioClips.deleteItem);
                itemPicked = false;

                if(selectedSlot.GetComponent<Slot>().Item == null) {
                    Destroy(selectedSlot.transform.GetChild(0).gameObject);
                }
            }
        }
    }

    public void PickItem(InputAction.CallbackContext value)
    {
        if(value.started) {
            // Has NO item picked
            if(!itemPicked) {
                // Selected slot has an item
                if(selectedSlot.GetComponent<Slot>().Item != null) {
                    itemPicked = true;

                    selectedItem = Instantiate(selectedSlot.transform.GetChild(0).gameObject);
                    selectedSlot.GetComponent<Slot>().Item = null;
                    Destroy(selectedSlot.transform.GetChild(0).gameObject);
                    itemText.text = selectedSlot.GetComponent<Slot>().Select();
                    PlaySound(audioClips.selectItem);

                    ObjectPreview();
                }
            }
            // Has an item picked
            else {
                // Selected slot has NO item
                if(selectedSlot.GetComponent<Slot>().Item == null) {
                    // Destroy preview object
                    Destroy(selectedSlot.transform.GetChild(0).gameObject);

                    GameObject objectInstace = Instantiate(selectedItem, selectedSlot.transform);
                    selectedSlot.GetComponent<Slot>().Item = objectInstace.GetComponent<Item>();
                    itemText.text = selectedSlot.GetComponent<Slot>().Select();
                    PlaySound(audioClips.selectItem);

                    Destroy(selectedItem.gameObject);

                    itemPicked = false;
                }
                // Selected slot has an item
                else {
                    GameObject aux = selectedSlot.transform.GetChild(0).gameObject;
                    GameObject objectInstance = Instantiate(selectedItem, selectedSlot.transform);
                    selectedSlot.GetComponent<Slot>().Item = objectInstance.GetComponent<Item>();
                    itemText.text = selectedSlot.GetComponent<Slot>().Reread();

                    Destroy(selectedItem.gameObject);
                    selectedItem = Instantiate(aux);
                    Destroy(aux.gameObject);

                    PlaySound(audioClips.selectItem);
                }
            }
        }
    }

    public void GetTopSlot(InputAction.CallbackContext value)
    {
        if(value.started) {
            selectedSlot.GetComponent<Slot>().Deselect();

            if(slotId > 5) {
                slotId -= 6;
            }
            selectedSlot = slots[slotId];
            itemText.text = selectedSlot.GetComponent<Slot>().Select();
            ObjectPreview();

            PlaySound(audioClips.blop);
        }
    }

    public void GetBottomSlot(InputAction.CallbackContext value)
    {
        if(value.started) {
            selectedSlot.GetComponent<Slot>().Deselect();

            if(slotId < 12) {
                slotId += 6;
            }
            selectedSlot = slots[slotId];
            itemText.text = selectedSlot.GetComponent<Slot>().Select();
            ObjectPreview();

            PlaySound(audioClips.blop);
        }
    }

    public void GetRightSlot(InputAction.CallbackContext value)
    {
        if(value.started) {
            selectedSlot.GetComponent<Slot>().Deselect();

            if(slotId < 17) {
                slotId++;
            }
            selectedSlot = slots[slotId];
            itemText.text = selectedSlot.GetComponent<Slot>().Select();
            ObjectPreview();

            PlaySound(audioClips.blop);
        }
    }

    public void GetLeftSlot(InputAction.CallbackContext value)
    {
        if(value.started) {
            selectedSlot.GetComponent<Slot>().Deselect();

            if(slotId > 0) {
                slotId--;
            }
            selectedSlot = slots[slotId];
            itemText.text = selectedSlot.GetComponent<Slot>().Select();
            ObjectPreview();

            PlaySound(audioClips.blop);
        }
    }

    public void NextResolution(InputAction.CallbackContext value)
    {
        if(value.started) {
            resolutionId++;

            if(resolutionId > 2) {
                resolutionId = 0;
            }

            currentResolution = GetComponent<ResolutionSettings>().Resolutions[resolutionId];
            Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, FullScreenMode.Windowed);

            resolutionText.text = currentResolution.x + "x" + currentResolution.y;
            PlaySound(audioClips.changeResolution);

            StopAllCoroutines();
            StartCoroutine(ShowHideText());
        }
    }

    public void PrevResolution(InputAction.CallbackContext value)
    {
        if(value.started) {
            resolutionId--;

            if(resolutionId < 0) {
                resolutionId = 2;
            }

            currentResolution = GetComponent<ResolutionSettings>().Resolutions[resolutionId];
            Screen.SetResolution((int)currentResolution.x, (int)currentResolution.y, FullScreenMode.Windowed);

            resolutionText.text = currentResolution.x + "x" + currentResolution.y;
            PlaySound(audioClips.changeResolution);

            StopAllCoroutines();
            StartCoroutine(ShowHideText());
        }
    }

    private IEnumerator ShowHideText()
    {
        resolutionText.gameObject.SetActive(true);

        yield return new WaitForSeconds(secondsToHide);

        resolutionText.gameObject.SetActive(false);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void ObjectPreview()
    {
        if(selectedItem != null && selectedSlot.GetComponent<Slot>().Item == null) {
            GameObject previewObjects = Instantiate(selectedItem, selectedSlot.transform);
            previewObjects.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
        }
    }
}