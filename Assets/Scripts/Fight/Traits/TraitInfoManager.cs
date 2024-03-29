using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnitManagerSocketIO;

public class TraitInfoManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TraitBase traitBase;
    [SerializeField] private int currBreakpoint;
    [SerializeField] private Image border;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI currentBreakpoint;
    [SerializeField] private TextMeshProUGUI nameTrait;
    [SerializeField] private TextMeshProUGUI breakpoint;

    public void SetTrait(TraitBase trait, int currentBreakpoint)
    {
        traitBase = trait;
        currBreakpoint = currentBreakpoint;
        SetBorder(currentBreakpoint, trait.breakpoint);
        SetIcon(trait.icon, currentBreakpoint, trait.breakpoint);
        SetCurrentBreakpoint(currentBreakpoint);
        SetNameTrait(trait.name);
        SetBreakpoint(trait.breakpoint);
    }

    public void SetBorder(int currentBreakpoint, int[] breakpoint)
    {
        if (currentBreakpoint < breakpoint[0])
        {
            this.border.sprite = Resources.Load<Sprite>("textures/traits/traits_border_none");
        }
        else if (currentBreakpoint >= breakpoint[breakpoint.Length - 1])
        {
            this.border.sprite = Resources.Load<Sprite>("textures/traits/traits_border_gold");
        }
        else if (currentBreakpoint >= breakpoint[0] && currentBreakpoint < breakpoint[1])
        {
            if(breakpoint.Length == 2)
            {
                this.border.sprite = Resources.Load<Sprite>("textures/traits/traits_border_silver");
            }
            else
            {
                this.border.sprite = Resources.Load<Sprite>("textures/traits/traits_border_bronze");
            }
        }
        else
        {
            this.border.sprite = Resources.Load<Sprite>("textures/traits/traits_border_silver");
        }
    }
    public void SetIcon(string icon, int currentBreakpoint, int[] breakpoint)
    {
        this.icon.sprite = Resources.Load<Sprite>("textures/traits/" + icon);
        if(currentBreakpoint < breakpoint[0])
        {
            this.icon.color = Color.gray;
        }
        else
        {
            this.icon.color = Color.black;
        }
    }
    public void SetCurrentBreakpoint(int currentBreakpoint)
    {
        this.currentBreakpoint.text = currentBreakpoint.ToString();
    }
    public void SetNameTrait(string nameTrait)
    {
        this.nameTrait.text = nameTrait;
    }
    public void SetBreakpoint(int[] breakpoint)
    {
        this.breakpoint.text = breakpoint[0].ToString();
        for (int i = 1; i < breakpoint.Length; i++)
        {
            this.breakpoint.text += " > " + breakpoint[i].ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TraitDescriptionManager.instance.SetComposition(traitBase.composition);
        TraitDescriptionManager.instance.SetImageIcon(traitBase.icon);
        TraitDescriptionManager.instance.SetTextName(traitBase.name);
        TraitDescriptionManager.instance.SetTextDescription(traitBase.description);
        TraitDescriptionManager.instance.SetTextBreakpoint(traitBase.breakpoint, traitBase.detailBreakpoint, currBreakpoint);
        TraitDescriptionManager.instance.gameObject.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        TraitDescriptionManager.instance.gameObject.SetActive(false);
    }
}
