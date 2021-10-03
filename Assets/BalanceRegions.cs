using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BalanceRegions : MonoBehaviour
{
    public Slider Slider;
    public List<BalanceRegion> Regions;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (BalanceRegion b in Regions)
        {
            RectTransform r = b.GetComponent<RectTransform>();
            r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, r.rect.width-.1f);
        }
    }

    private void OnDrawGizmos()
    {
        foreach(BalanceRegion b in Regions)
        {
            RectTransform r = b.GetComponent<RectTransform>();
            Gizmos.color = Color.red;
            Gizmos.DrawRay(r.position, Vector2.right*r.rect.width);
        }
    }
}
