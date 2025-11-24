using System.Collections.Generic;
using JetBrains.Annotations;
using MoreSpace.Domain;
using MoreSpace.InGame.Weapons;
using R3;
using UnityEngine;
using UnityEngine.UI;

public class SkillViewer : MonoBehaviour
{
    [SerializeField] private RectTransform selectFrame;
    [SerializeField] private Image[] baseImage = new Image[4];
    [SerializeField] private Image[] enableImage = new Image[4];
    [SerializeField] private Scrollbar[] recastTimeBar = new Scrollbar[4];

    private Dictionary<int, int> indexToViewIndex = new();
    
    public void VisualizeSkills(Skill[] skills)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            baseImage[i].sprite = skills[i].Icon;
            enableImage[i].sprite = skills[i].Icon;
            recastTimeBar[i].size = 0;
        }

        this.gameObject.GetComponent<ControlWeapon>().nowIndex.Skip(1).Subscribe(index =>
        {
            Debug.Log($"ChangeTo{index}:Dict{indexToViewIndex[index]}");
            selectFrame.SetParent(baseImage[indexToViewIndex[index]].rectTransform);
            selectFrame.localPosition = Vector3.zero;
        });
    }

    public void ActivateSkillUI(Skill target, [CanBeNull] Weapon weapon)
    {
        int index = (int)target.Level;
        recastTimeBar[index].size = 1;

        if (weapon != null)
        {
            weapon.nextFireTime.Subscribe(f => recastTimeBar[index].size = (1 - f / weapon.fireRate));
            Debug.Log($"{indexToViewIndex.Count},{index}");
            indexToViewIndex.Add(indexToViewIndex.Count,index);
        }
    }
}
