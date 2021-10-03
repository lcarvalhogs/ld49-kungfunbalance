using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class BalanceRegion: MonoBehaviour
    {

        public int index;
        public int minValue;
        public int maxValue;
        public int damage;
        public int score;

        private void Awake()
        {
            Image img = gameObject.GetComponent<Image>();
            Color imgColor = img.color;
            imgColor.a = .5f;
            img.color = imgColor;
        }
    }
}
