using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnitManagerSocketIO;

public class TraitDescriptionManager : MonoBehaviour
{
    public static TraitDescriptionManager instance { get; private set; }

    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtName;
    [SerializeField] private TextMeshProUGUI txtDescription;
    [SerializeField] private TextMeshProUGUI txtBreakpoint;
    [SerializeField] private Transform tfComposition;

    [SerializeField] private List<GameObject> lstComposition;
    [SerializeField] private GameObject pfComposition;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
        lstComposition = new List<GameObject>();
    }

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void SetImageIcon(string icon)
    {
        var sprite = Resources.Load<Sprite>("textures/traits/" + icon);
        imgIcon.sprite = sprite;
    }
    public void SetTextName(string traitName)
    {
        txtName.text = traitName;
    }
    public void SetTextDescription(string description)
    {
        txtDescription.text = description;
    }
    public void SetTextBreakpoint(int[] breakpoint, string[] detailBreakpoint, int currBreakpoint)
    {
        txtBreakpoint.text = "";
        for (int i = 0; i< breakpoint.Length; i++)
        {
            if (breakpoint[i] == currBreakpoint || (breakpoint[i] < currBreakpoint && currBreakpoint < breakpoint[i + 1]))
            {
                txtBreakpoint.text += "(" + breakpoint[i] + ") " + detailBreakpoint[i] + "\n";
            }
            else
            {
                txtBreakpoint.text += "<color=#808080>(" + breakpoint[i] + ") " + detailBreakpoint[i] + "</color>\n";
            }
        }
    }

    public void SetComposition(string[][] composition)
    {
        int i = 0;
        foreach (string[] compositionArray in composition)
        {
            if(i < lstComposition.Count && lstComposition[i] != null)
            {
                var iconSprite = Resources.Load<Sprite>("textures/avatar/avatar_" + compositionArray[0]);
                var borderSprite = Resources.Load<Sprite>("textures/border-unit/composition_border_" + compositionArray[1]);
                lstComposition[i].transform.GetChild(0).GetComponent<Image>().sprite = iconSprite;
                lstComposition[i].transform.GetChild(1).GetComponent<Image>().sprite = borderSprite;
            }
            else
            {
                GameObject obj = Instantiate(pfComposition, tfComposition);
                var iconSprite = Resources.Load<Sprite>("textures/avatar/" + compositionArray[0]);
                var borderSprite = Resources.Load<Sprite>("textures/border-unit/composition_border_" + compositionArray[1]);
                obj.transform.GetChild(0).GetComponent<Image>().sprite = iconSprite;
                obj.transform.GetChild(1).GetComponent<Image>().sprite = borderSprite;
                lstComposition.Add(obj);
            }
            i++;
        }
        for(int j = lstComposition.Count - 1; j >= i; j--)
        {
            GameObject temp = lstComposition[j];
            lstComposition.RemoveAt(j);
            Destroy(temp);
        }
    }
}
