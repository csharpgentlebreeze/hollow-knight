using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class KnightPanel : BasePanel
{
    private SoulOrb soulOrb;
    private GeoCollect geoCollect;
    private TextMeshProUGUI geoText;
    // Start is called before the first frame update
    void Start()
    {
        soulOrb = GetComponentInChildren<SoulOrb>();
        soulOrb.DelayShowOrb(1f);
        
        geoCollect = FindObjectOfType<GeoCollect>();
        geoText = GetComponentInChildren<TextMeshProUGUI>();
        geoCollect.geoText = geoText;
        geoText.text = geoCollect.geoCount.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
