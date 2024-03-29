using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnitManagerSocketIO;

public class ItemDescriptionManager : MonoBehaviour
{
    public static ItemDescriptionManager instance { get; private set; }

    [SerializeField] private Transform _itemDescription;
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtStats;
    [SerializeField] private TextMeshProUGUI txtPassive;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        _itemDescription = transform.GetChild(0);
    }

    public Transform itemDescription
    {
        get { return _itemDescription; }
        set { _itemDescription = value; }
    }

    private void Start()
    {
        _itemDescription.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void ActiveItemDescription(bool isActive, Item item)
    {
        if (isActive)
        {
            SetImgIcon(item.icon);
            SetTxtName(item.name);
            SetTxtStats(item.descriptionStat);
            SetTxtPassive(item.descriptionPassive);
        }
        gameObject.SetActive(isActive);
    }


    public void SetImgIcon(string icon)
    {
        var sprite = Resources.Load<Sprite>("textures/items/" + icon);
        imgIcon.sprite = sprite;
    }

    public void SetTxtName(string name)
    {
        txtName.text = name;
    }

    public void SetTxtStats(string stats)
    {
        txtStats.text = stats;
    }
    public void SetTxtPassive(string descriptionPassive)
    {
        txtPassive.text = descriptionPassive;
    }
}
